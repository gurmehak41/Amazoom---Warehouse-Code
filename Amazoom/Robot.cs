using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Amazoom
{
    public enum RobotStatus  // for UI only
    {
        Null,
        PickUp,
        DropOff,
        GoHome
    }

    public enum RobotType  // for UI only
    {
        Null,
        Restocking,
        Delivery
    }

    public class ComputerToRobotCommand
    {
        public Dock Dock
        { get; }
        public Point Shelf
        { get; }
        public Tuple<Product, int> Product
        { get; }

        public ComputerToRobotCommand(Dock dock, Point shelf,
            Tuple<Product, int> product)
        {
            this.Dock = dock;
            this.Shelf = shelf;
            this.Product = product;
        }
    }

    public class Robot
    {

        public int Id
        { get; }
        public double MaxCapacity
        { get; }
        public Point Location
        { get; private set; }
        public RobotStatus Status
        { get; private set; }
        public RobotType Type
        { get; private set; }
        public Tuple<Product, int> Product
        { get; private set; }

        private readonly ConcurrentQueue<ComputerToRobotCommand> commands;
        private readonly Map map;
        private readonly Shelf shelf;
        private readonly Comm comm;
        public bool active;
        private const int delay = 250;  // milliseconds

        public Robot(int id, double maxCapacity,
            ConcurrentQueue<ComputerToRobotCommand> commands, Map map,
            Shelf shelf, Comm comm)
        {
            this.Id = id;
            this.MaxCapacity = maxCapacity;
            this.Location = null;  // Not in warehouse, start from home
            this.Status = RobotStatus.Null;
            this.Type = RobotType.Null;
            this.Product = null;
            this.active = true;

            this.commands = commands;
            this.map = map;
            this.shelf = shelf;
            this.comm = comm;
        }

        private ComputerToRobotCommand GetCommand()
        {
            ComputerToRobotCommand command;
            while (!this.commands.TryDequeue(out command))
            {
                Thread.Sleep(delay);
            }
            return command;
        }

        private List<Point> GetPath(Point dest)
        {
            List<Point> path;
            while (!this.map.GeneratePath(this.Location, dest, out path))
            {
                Thread.Sleep(delay);
            }
            return path;
        }

        private void MoveRobot(List<Point> path)
        {
            while (path.Count > 0)
            {
                if (!this.map.Grid[path[0].Row, path[0].Column])
                {
                    this.comm.MapMutex.WaitOne();
                    // Release current location
                    if (this.Location != null)
                    {
                        this.map.Grid[this.Location.Row, this.Location.Column]
                            = false;
                    }
                    // Occupy next location
                    this.Location = path[0];
                    if ((this.Status == RobotStatus.GoHome) &&
                        (this.Location.Row == this.map.Home.Row) &&
                        (this.Location.Column == this.map.Home.Column))
                    {
                        this.Location = null;
                    }
                    else
                    {
                        this.map.Grid[this.Location.Row, this.Location.Column]
                            = true;
                    }
                    this.comm.MapMutex.ReleaseMutex();
                    path.RemoveAt(0);
                }
                Thread.Sleep(delay);
            }
        }

        private void StockRobotFromTruck(ComputerToRobotCommand command)
        {
            Product product = command.Product.Item1;
            int quantity = command.Product.Item2;

            // Remove from truck
            if (!command.Dock.Truck.RemoveProduct(product, quantity))
            {
                Debug.WriteLine($"Error: Robot {this.Id} [Restocking] "
                    + "cannot remove product from truck.");
            }

            // Check if truck is empty
            if (command.Dock.Truck.IsTruckEmpty())
            {
                command.Dock.Busy.Release();
            }

            // Add to robot
            if ((product.Weight * quantity) > this.MaxCapacity)
            {
                Debug.WriteLine($"Error: Robot {this.Id} [Restocking] "
                    + "cannot add product to robot.");
            }
            this.Product = command.Product;
        }

        private void StockTruckFromRobot(ComputerToRobotCommand command)
        {
            Product product = command.Product.Item1;
            int quantity = command.Product.Item2;

            // Remove from robot
            this.Product = null;

            // Add to truck
            if (!command.Dock.Truck.AddProduct(product, quantity))
            {
                Debug.WriteLine($"Error: Robot {this.Id} [Delivery] "
                    + "cannot add product to truck.");
            }

            // Check if truck is full
            if (command.Dock.Truck.IsOrdersFulfilled())
            {
                command.Dock.Busy.Release();
            }
        }

        private void StockRobotFromShelf(ComputerToRobotCommand command)
        {
            Product product = command.Product.Item1;
            int quantity = command.Product.Item2;

            // Remove from shelf
            //this.comm.ShelfMutex.WaitOne();
            if (!this.shelf.Remove(command.Shelf, product, quantity))
            {
                Debug.WriteLine($"Error: Robot {this.Id} [Delivery] "
                    + "cannot remove product from shelf.");
            }
            //this.comm.ShelfMutex.ReleaseMutex();

            // Add to robot
            if ((product.Weight * quantity) > this.MaxCapacity)
            {
                Debug.WriteLine($"Error: Robot {this.Id} [Delivery] "
                    + "cannot add product to robot.");
            }
            this.Product = command.Product;
        }

        private void StockShelfFromRobot(ComputerToRobotCommand command)
        {
            Product product = command.Product.Item1;
            int quantity = command.Product.Item2;

            // Remove from robot
            this.Product = null;

            // Add to shelf
            //this.comm.ShelfMutex.WaitOne();
            if (!this.shelf.Add(command.Shelf, product, quantity))
            {
                Debug.WriteLine($"Error: Robot {this.Id} [Restocking] "
                    + " cannot add product to shelf.");
            }
            //this.comm.ShelfMutex.ReleaseMutex();
        }

        /**
         * \brief Start new thead with this class method to represent a working
         *        robot.
         */
        public void ThreadProc()
        {
            ComputerToRobotCommand command;
            List<Point> path;

            // Forever loop
            while (this.active)
            {
                //this.comm.RobotCommandsConsumerSemaphore.Wait();
                Debug.WriteLine($"Robot {this.Id} is waiting for command.");
                while (true)
                {
                    if(this.commands.TryDequeue(out command))
                    {
                        break;
                    }
                    else if (this.active == false)
                    {
                        break;
                    }
                    Thread.Sleep(delay);

                }
                if (this.active == false)
                {
                    break;
                }

                Debug.WriteLine($"Robot {this.Id} receive command.");

                switch (command.Dock.Truck.Type)
                {
                    case TruckType.Restocking:
                        this.Type = RobotType.Restocking;
                        path = GetPath(command.Dock.Location);
                        this.Status = RobotStatus.PickUp;
                        Debug.WriteLine($"Robot {this.Id} [Restocking] receive path to dock.");
                        MoveRobot(path);
                        Debug.WriteLine($"Robot {this.Id} [Restocking] moved to dock.");
                        StockRobotFromTruck(command);
                        Debug.WriteLine($"Robot {this.Id} [Restocking] stocked from truck.");
                        path = GetPath(command.Shelf);
                        this.Status = RobotStatus.DropOff;
                        Debug.WriteLine($"Robot {this.Id} [Restocking] receive path to shelf.");
                        MoveRobot(path);
                        StockShelfFromRobot(command);
                        Debug.WriteLine($"Robot {this.Id} [Restocking] stocked to shelf.");
                        path = GetPath(this.map.Home);
                        this.Status = RobotStatus.GoHome;
                        Debug.WriteLine($"Robot {this.Id} [Restocking] receive path to home.");
                        MoveRobot(path);
                        Debug.WriteLine($"Robot {this.Id} [Restocking] went to home.");
                        break;
                    case TruckType.Delivery:
                        this.Type = RobotType.Delivery;
                        path = GetPath(command.Shelf);
                        this.Status = RobotStatus.PickUp;
                        Debug.WriteLine($"Robot {this.Id} [Delivery] receive path to shelf.");
                        MoveRobot(path);
                        Debug.WriteLine($"Robot {this.Id} [Delivery] moved to shelf.");
                        StockRobotFromShelf(command);
                        path = GetPath(command.Dock.Location);
                        this.Status = RobotStatus.DropOff;
                        Debug.WriteLine($"Robot {this.Id} [Delivery] receive path to dock.");
                        MoveRobot(path);
                        Debug.WriteLine($"Robot {this.Id} [Delivery] moved to dock.");
                        StockTruckFromRobot(command);
                        path = GetPath(this.map.Home);
                        this.Status = RobotStatus.GoHome;
                        Debug.WriteLine($"Robot {this.Id} [Delivery] receive path to home.");
                        MoveRobot(path);
                        Debug.WriteLine($"Robot {this.Id} [Delivery] went to home.");
                        break;
                }
                this.Status = RobotStatus.Null;
                this.Type = RobotType.Null;
            }
        }
    }
}
