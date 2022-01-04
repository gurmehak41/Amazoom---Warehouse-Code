using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Timers;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Amazoom
{
    public class CentralComputer
    {
        private readonly int maxShelfNum;
        private readonly double maxShelfCap;
        private readonly double maxRobotCap;
        private readonly double maxTruckVolume;
        private readonly double maxTruckWeight;

        private readonly ConcurrentQueue<Order> impendingOrders;
        private readonly List<Order> processedOrders;

        private readonly Map map;
        private readonly Comm comm;

        private Shelf shelf;
        private Inventory inventory;

        private List<Dock> docks;
        private List<Thread> dockTruckProcs;
        private readonly ConcurrentQueue<Truck> trucks;

        private List<Robot> robots;
        private List<Thread> robotProcs;
        private readonly ConcurrentQueue<ComputerToRobotCommand> robotCommands;

        private readonly Thread restocking;
        private readonly Thread delivering;
        private readonly Thread cleaningRobots;

        public int totalRobots;

        private int warehouseId;
        private bool isServerConnected;
        private SimpleTCP.SimpleTcpClient tcpClient;

        private const int delay = 1000;  // milliseconds

        // These properties are used by Warehouse UI only
        public List<Robot> Robots
        {
            get
            {
                return this.robots;
            }
        }

        public Dictionary<Product, int> InventoryDatabase
        {
            get
            {
                return new Dictionary<Product, int>(
                    this.inventory.productKeyDatabase);
            }
        }

        public Dictionary<Product, int> ShelfDatabase
        {
            get
            {
                // Change dictionary type
                Dictionary<Product, ProductKeyDatabaseValue> before =
                    new Dictionary<Product, ProductKeyDatabaseValue>(
                        this.shelf.ProductKeyDatabase);
                Dictionary<Product, int> after =
                    new Dictionary<Product, int>();
                foreach (var productData in before)
                {
                    after.Add(productData.Key, productData.Value.quantity);
                }
                return after;
            }
        }

        public List<Order> Orders
        {
            get
            {
                return this.processedOrders;
            }
        }

        public List<Dock> WarehouseDock
        {
            get
            {
                return this.docks;
            }
        }

        public CentralComputer(int sizeX, int sizeY, int numDock, int numRobot,
            double maxShelfCap, int maxShelfNum, double maxTruckVolume,
            double maxTruckWeight, double maxRobotCap, string filename)
        {
            this.maxShelfCap = maxShelfCap;
            this.maxShelfNum = maxShelfNum;
            this.maxTruckVolume = maxTruckVolume;
            this.maxTruckWeight = maxTruckWeight;
            this.maxRobotCap = maxRobotCap;
            this.totalRobots = numRobot;

            this.impendingOrders = new ConcurrentQueue<Order>();
            this.processedOrders = new List<Order>();

            this.map = new Map(sizeX, sizeY, new Point(0, 0));
            this.comm = new Comm(numDock, 1000 * numRobot);

            this.trucks = new ConcurrentQueue<Truck>();
            this.robotCommands = new ConcurrentQueue<ComputerToRobotCommand>();

            RegisterShelves(this.map, this.maxShelfNum, this.maxShelfCap);
            RegisterProducts(filename);
            RegisterDocks(this.map, numDock);
            RegisterRobots(numRobot, maxRobotCap, this.robotCommands,
                this.map, this.shelf, this.comm);

            this.isServerConnected = false;

            this.restocking = new Thread(() => RestockingProc());
            this.delivering = new Thread(() => DeliveringProc());
            this.cleaningRobots = new Thread(() => cleanRobotList());

        }

        public void cleanRobotList()
        {
            while (true)
            {
                for (int i = 0; i < robots.Count; i++)
                {
                    if (robotProcs[i].IsAlive == false)
                    {
                        robotProcs.RemoveAt(i);
                        robots.RemoveAt(i);
                    }
                }
                Thread.Sleep(500);
            }
        }

        private void RegisterShelves(Map map, int maxShelfNum,
            double maxShelfCap)
        {
            this.shelf = new Shelf(maxShelfCap);

            for (int i = 1; i < map.NumRows - 1; i++)
            {
                for (int j = 0; j < map.NumColumns; j++)
                {
                    for (int k = 0; k < maxShelfNum; k++)
                    {
                        if (j == 0)
                        {
                            Point pointRight = new Point(
                                i, j, ShelfSide.Right, k);
                            this.shelf.AddNewPoint(pointRight);
                        }
                        else if (j == map.NumColumns - 1)
                        {
                            Point pointLeft = new Point(
                                i, j, ShelfSide.Left, k);
                            this.shelf.AddNewPoint(pointLeft);
                        }
                        else
                        {
                            Point pointLeft = new Point(
                                i, j, ShelfSide.Left, k);
                            Point pointRight = new Point(
                                i, j, ShelfSide.Right, k);
                            this.shelf.AddNewPoint(pointLeft);
                            this.shelf.AddNewPoint(pointRight);
                        }
                    }
                }
            }
        }

        private void RegisterProducts(string filename)
        {
            this.inventory = new Inventory();

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            string name = "null";
            double volume = 1.0;
            double weight = 1.0;

            while ((line = file.ReadLine()) != null)
            {
                string[] words = line.Split(' ');

                if (words[0] == "Name:")
                    name = words[1];
                else if (words[0] == "Weight:")
                    weight = Convert.ToDouble(words[1]);
                else if (words[0] == "Volume:")
                    volume = Convert.ToDouble(words[1]);
                else if (words[0] == "End")
                {
                    Product product = new Product(name, volume, weight);
                    // Check product is not already added
                    if (!this.shelf.GetPoint(product, out Point _))
                    {
                        if (this.shelf.GetEmptyPoint(out Point point))
                        {
                            this.inventory.Add(product, 0);
                            this.shelf.Add(point, product, 0);
                        }
                    }
                }
            }
        }

        public void RemoveRobot()
        {
            if (this.totalRobots > 1)
            {
                Robot temp;
                for (int i = robots.Count - 1; i >= 0; i--)
                {
                    temp = this.robots[i];
                    if (temp.active)
                    {
                        temp.active = false;
                        break;
                    }
                }
                //this.robots.RemoveAt(robots.Count - 1);
                this.totalRobots--;
            }
        }


        public void AddRobot()
        {
            Robot robot = new Robot(this.totalRobots, this.maxRobotCap, this.robotCommands,
                    this.map, this.shelf, this.comm);
            this.robots.Add(robot);
            Thread thread = new Thread(() => robot.ThreadProc());
            this.robotProcs.Add(thread);
            thread.Start();
            this.totalRobots++;
        }


        private void RegisterDocks(Map map, int numDock)
        {
            this.docks = new List<Dock>();
            this.dockTruckProcs = new List<Thread>();

            for (int col = 1; col < numDock + 1 && col < map.NumColumns; col++)
            {
                Point point = new Point(map.NumRows - 1, col);
                Dock dock = new Dock(col, point);
                dock.Busy.Release();
                this.docks.Add(dock);

                Thread thread = new Thread(() => DockTruckProc(dock));
                thread.Start();
                this.dockTruckProcs.Add(thread);
            }
        }

        private void RegisterRobots(int numRobot, double maxRobotCap,
            ConcurrentQueue<ComputerToRobotCommand> commands, Map map,
            Shelf shelf, Comm comm)
        {
            this.robots = new List<Robot>();
            this.robotProcs = new List<Thread>();

            for (int i = 0; i < numRobot; i++)
            {
                Robot robot = new Robot(i, maxRobotCap, commands,
                    map, shelf, comm);
                this.robots.Add(robot);

                Thread thread = new Thread(() => robot.ThreadProc());
                this.robotProcs.Add(thread);
                thread.Start();
            }
        }

        private void RequestRestockTruck(Truck truck)
        {
            this.trucks.Enqueue(truck);

            foreach (KeyValuePair<Product, int> productData
                in truck.CurrentProducts)
            {
                this.inventory.Add(productData.Key, productData.Value);
            }
        }

        private void RequestDeliveryTruck(Truck truck)
        {
            this.trucks.Enqueue(truck);

            foreach (Order order in truck.RequestedOrders)
            {
                foreach (KeyValuePair<Product, int> productData
                    in order.Products)
                {
                    this.inventory.Remove(productData.Key, productData.Value);
                }
            }
        }

        private void EnqueueCommandToRobot(KeyValuePair<Product, int>
            productData, Dock dock)
        {
            Product product = productData.Key;
            int quantity = productData.Value;

            int maxQty = Convert.ToInt32(Math.Floor(
                this.maxRobotCap / product.Weight));
            int remainingQty = productData.Value;
            while (remainingQty > 0)
            {
                int currentQty = maxQty;
                if (remainingQty < currentQty)
                {
                    currentQty = remainingQty;
                }

                Tuple<Product, int> commandProduct =
                    new Tuple<Product, int>(product, currentQty);
                Point point;
                if (this.shelf.GetPoint(product, out point))
                {
                    //this.comm.RobotCommandsProducerSemaphore.Wait();
                    ComputerToRobotCommand newCommand =
                        new ComputerToRobotCommand(dock, point,
                        commandProduct);
                    this.robotCommands.Enqueue(newCommand);
                    //this.comm.RobotCommandsConsumerSemaphore.Release();
                }
                else if (this.shelf.GetEmptyPoint(out point))
                {
                    //this.comm.RobotCommandsProducerSemaphore.Wait();
                    ComputerToRobotCommand newCommand =
                        new ComputerToRobotCommand(dock, point,
                        commandProduct);
                    this.robotCommands.Enqueue(newCommand);
                    //this.comm.RobotCommandsConsumerSemaphore.Release();
                }
                remainingQty -= currentQty;
            }
        }

        private void SetOrderStatus(Order order)
        {
            if (processedOrders.Contains(order))
            {
                var index = processedOrders.IndexOf(order);
                processedOrders[index].Status = order.Status;
            }
            else
            {
                processedOrders.Add(order);
            }

            if (this.isServerConnected)
            {
                string command = "OrderFromWarehouse";
                string sender = "-";
                string receiver = order.Id.ToString();
                string content = order.Status.ToString();
                string message = command + "/" + sender + "/" + receiver + "/"
                    + content;
                this.tcpClient.WriteLine(message);
            }
        }

        private void RestockingProc()
        {
            Truck restockingTruck = new Truck(TruckType.Restocking,
                this.maxTruckVolume, this.maxTruckWeight);

            while (true)
            {
                foreach (KeyValuePair<Product, int> productData
                    in this.inventory.productKeyDatabase)
                {
                    Product product = productData.Key;
                    int currentQty = productData.Value;

                    int maxQty = Convert.ToInt32(Math.Floor(
                        this.maxShelfCap / product.Weight));
                    int orderQty = maxQty - currentQty;
                    if (orderQty > 0)
                    {
                        // If truck is full, send truck away, request new truck
                        if (!restockingTruck.AddProduct(product, orderQty))
                        {
                            RequestRestockTruck(restockingTruck);
                            restockingTruck = new Truck(TruckType.Restocking,
                                this.maxTruckVolume, this.maxTruckWeight);

                            restockingTruck.AddProduct(product, orderQty);
                        }
                    }
                }

                if (!restockingTruck.IsTruckEmpty())
                {
                    RequestRestockTruck(restockingTruck);
                    restockingTruck = new Truck(TruckType.Restocking,
                                this.maxTruckVolume, this.maxTruckWeight);
                }
                Thread.Sleep(delay);
            }
        }

        private void DeliveringProc()
        {
            Truck deliveryTruck = new Truck(TruckType.Delivery,
                maxTruckVolume, maxTruckWeight);

            while (true)
            {
                while (impendingOrders.TryDequeue(out Order order))
                {
                    double orderedVolume = 0.0;
                    double orderedCapacity = 0.0;

                    foreach (KeyValuePair<Product, int> orderData in order.Products)
                    {
                        Product product = orderData.Key;
                        int quantity = orderData.Value;

                        if (!this.inventory.CheckProductQty(product, quantity))
                        {
                            order.Status = OrderStatus.Denied;
                            SetOrderStatus(order);
                            break;
                        }
                        orderedVolume += product.Volume * quantity;
                        orderedCapacity += product.Weight * quantity;
                    }

                    if (order.Status != OrderStatus.Denied && (
                        orderedVolume > deliveryTruck.MaxVolume ||
                        orderedCapacity > deliveryTruck.MaxWeight))
                    {
                        order.Status = OrderStatus.Denied;
                        SetOrderStatus(order);
                        continue;
                    }

                    if (order.Status != OrderStatus.Denied)
                    {
                        order.Status = OrderStatus.Accepted;
                        SetOrderStatus(order);

                        // If truck is full, send truck away, request new truck
                        if (!deliveryTruck.AddRequestedOrder(order))
                        {
                            RequestDeliveryTruck(deliveryTruck);
                            deliveryTruck = new Truck(TruckType.Delivery,
                                maxTruckVolume, maxTruckWeight);

                            deliveryTruck.AddRequestedOrder(order);
                        }
                    }
                }

                if (!deliveryTruck.IsOrdersEmpty())
                {
                    RequestDeliveryTruck(deliveryTruck);
                    deliveryTruck = new Truck(TruckType.Delivery,
                                maxTruckVolume, maxTruckWeight);
                }
                Thread.Sleep(delay);
            }
        }

        private void DockTruckProc(Dock dock)
        {
            while (true)
            {
                if (trucks.TryDequeue(out Truck truck))
                {
                    dock.Busy.Wait();
                    dock.Truck = truck;
                    if (truck.Type == TruckType.Restocking)
                    {
                        foreach (KeyValuePair<Product, int> productData
                            in truck.CurrentProducts)
                        {
                            EnqueueCommandToRobot(productData, dock);
                        }
                    }
                    else
                    {
                        foreach (Order order in truck.RequestedOrders)
                        {
                            order.Status = OrderStatus.InProgress;
                            SetOrderStatus(order);
                            foreach (KeyValuePair<Product, int> productData
                                in order.Products)
                            {
                                EnqueueCommandToRobot(productData, dock);
                            }
                        }
                    }
                    dock.Busy.Wait();
                    if (truck.Type == TruckType.Delivery)
                    {
                        foreach (Order order in truck.RequestedOrders)
                        {
                            order.Status = OrderStatus.Delivered;
                            SetOrderStatus(order);
                        }
                    }
                    dock.Busy.Release();
                }
                Thread.Sleep(delay);
            }
        }

        private void DataReceived(object sender, SimpleTCP.Message e)
        {
            string[] words = e.MessageString.Split('/');

            // Ensure this message is for warehouse
            if (words[0] == "OrderToWarehouse")
            {
                // Check this message is for this warehouse
                int warehouseId = Convert.ToInt32(words[2]);
                if (warehouseId == this.warehouseId)
                {
                    // Create new order
                    int customerId = Convert.ToInt32(words[1]);
                    Order orderReceived = new Order(
                        Convert.ToInt32(customerId));
                    // Place products of specified quantity into order
                    string[] order = words[3].Split(',');
                    foreach (string orderContent in order)
                    {
                        if (!String.IsNullOrEmpty(orderContent))
                        {
                            string[] nameAndQty = orderContent.Split('*');
                            string productName = nameAndQty[0];
                            int quantity = Convert.ToInt32(nameAndQty[1]);

                            this.inventory.GetProduct(productName,
                                out Product product);

                            orderReceived.Add(product, quantity);
                        }
                    }
                    this.impendingOrders.Enqueue(orderReceived);
                }
            }
        }

        /**
         * \brief Start the computer.
         */
        public void Start()
        {
            this.restocking.Start();
            this.delivering.Start();
            this.cleaningRobots.Start();
        }

        /**
         * \brief Connect to the server.
         * 
         * \param warehouseId Id for the warehouse
         * \param serverIP IP address for the server
         * \param serverPort Port number for the server
         * 
         * \returns true if connected, false otherwise
         */
        public bool ConnectToServer(int warehouseId, string serverIP,
            int serverPort)
        {
            this.warehouseId = warehouseId;

            try
            {
                this.tcpClient = new SimpleTCP.SimpleTcpClient().Connect(
                    serverIP, serverPort);
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Server cannot be connected");
                return false;
            }

            this.isServerConnected = true;
            this.tcpClient.Delimiter = 0x13;  // enter
            this.tcpClient.StringEncoder = Encoding.UTF8;
            this.tcpClient.DelimiterDataReceived += DataReceived;

            string command = "WarehouseStart";
            string sender = this.warehouseId.ToString();
            string receiver = "-";
            string content = "";

            foreach (Product product in this.inventory.productKeyDatabase.Keys)
            {
                string name = product.Name;
                int maxQty = Convert.ToInt32(Math.Floor(
                        this.maxShelfCap / product.Weight));

                content = content + name + "*" + maxQty.ToString() + ",";
            }

            string message = command + "/" + sender + "/" + receiver + "/" + content;

            this.tcpClient.WriteLine(message);

            return true;
        }

        /**
         * \brief Disconnects from server.
         */
        public void DisconnectFromServer()
        {
            if (this.isServerConnected)
            {
                string command = "WarehouseStop";
                string sender = this.warehouseId.ToString();
                string receiver = "-";
                string content = "-";
                string message = command + "/" + sender + "/" + receiver + "/" + content;

                this.tcpClient.WriteLine(message);
            }
        }

        /**
         * \brief Adds new product to the inventory.
         * ONLY to be used by the manager UI.
         **/
        public bool AddToInventory(string name, double weight, double volume)
        {
            Product product = new Product(name, volume, weight);

            if (!this.shelf.GetPoint(product, out Point _))
            {
                if (this.shelf.GetEmptyPoint(out Point point))
                {
                    this.inventory.Add(product, 0);
                    this.shelf.Add(point, product, 0);

                    if (this.isServerConnected)
                    {
                        string command = "WarehouseNewProduct";
                        string sender = this.warehouseId.ToString();
                        string receiver = "-";

                        int maxQty = Convert.ToInt32(Math.Floor(
                                this.maxShelfCap / weight));

                        string content = name + "*" + maxQty.ToString();

                        string message = command + "/" + sender + "/" + receiver + "/" + content;

                        this.tcpClient.WriteLine(message);
                    }

                    return true;
                }
            }

            return false;
        }

        /**
         * \brief Removes product of specified name from the inventory.
         * ONLY to be used by the manager UI.
         **/
        public bool RemoveFromInventory(string name)
        {
            if (this.inventory.GetProduct(name, out Product product))
            {
                double weight = product.Weight;

                if (this.inventory.DeleteProductFromWarehouse(name))
                {
                    if (this.isServerConnected)
                    {
                        string command = "WarehouseDeleteProduct";
                        string sender = this.warehouseId.ToString();
                        string receiver = "-";

                        int maxQty = Convert.ToInt32(Math.Floor(
                                    this.maxShelfCap / weight));

                        string content = name + "*" + maxQty.ToString();

                        string message = command + "/" + sender + "/" + receiver + "/" + content;

                        this.tcpClient.WriteLine(message);
                    }

                    return true;
                }
            }

            return false;
        }

        /**
         * \brief: ONLY FOR TESTING PURPOSES
         **/
        public void ComputerTest()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 5000;  // milliseconds
            timer.AutoReset = true;  // repeating
            timer.Elapsed += new ElapsedEventHandler(ShowProducts);
            timer.Start();
        }

        /**
         * \brief: ONLY FOR TESTING PURPOSES
         **/
        public void ShowProducts(object sender, ElapsedEventArgs e)
        {
            this.shelf.ShowProducts();
            this.inventory.ShowProducts();
        }

        /**
         * \brief: ONLY FOR TESTING PURPOSES
         */
        public void AddOrders()
        {
            Random random = new Random();
            Order order = new Order(random.Next(10000, 100000));
            // Get random number of products
            int numProducts = this.inventory.productKeyDatabase.Count;
            if (numProducts > 0)
            {
                int reqProducts = random.Next(1,
                    Math.Min(numProducts + 1, 100));  // limit for capacity
                int productIdx = 0;
                foreach (var productData in this.inventory.productKeyDatabase)
                {
                    if (productIdx >= numProducts)
                    {
                        break;
                    }
                    // Get random quantity of product
                    Product product = productData.Key;
                    int quantity = productData.Value;
                    if (quantity > 0)
                    {
                        int reqQuantity = random.Next(1,
                            Math.Min(quantity + 1, 100));  // limit for capacity
                        order.Add(product, reqQuantity);
                    }
                    productIdx++;
                }
                if (order.Products.Count > 0)
                {
                    this.impendingOrders.Enqueue(order);
                }
            }
        }
    }
}
