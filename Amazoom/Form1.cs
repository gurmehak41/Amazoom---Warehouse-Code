using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amazoom
{
    public partial class FormAmazoom : System.Windows.Forms.Form
    {
        public class RobotInfo
        {
            public int Id
            { get; }

            public string Product
            { get; }

            public int Quantity
            { get; }

            public RobotStatus Status  // housekeeping
            { get; }

            public RobotInfo(int id, string product, int quantity,
                RobotStatus status)
            {
                this.Id = id;
                this.Product = product;
                this.Quantity = quantity;
                this.Status = status;
            }
        }

        public class InventoryInfo
        {
            public string Product
            { get; }

            public int EstimatedQty
            { get; }

            public double ActualQty
            { get; }

            public InventoryInfo(string product, int estimated, int actual)
            {
                this.Product = product;
                this.EstimatedQty = estimated;
                this.ActualQty = actual;
            }
        }

        private CentralComputer computer;
        private List<RobotInfo> robotsInfo;
        private List<InventoryInfo> inventoryInfo;

        private int sizeX = 8;
        private int sizeY = 4;
        private int numDock = 2;
        private int numRobot = 4;
        private const double maxShelfCap = 100;
        private const int maxShelfNum = 6;
        private const double maxTruckVolume = 10;
        private const double maxTruckWeight = 400;
        private const double maxRobotCap = 50;
        private bool isComputerStarted = false;

        public FormAmazoom()
        {
            InitializeComponent();
        }


        private void UpdateRobotsInfo(List<Robot> robots)
        {
            robotsInfo.Clear();
            foreach (var robot in robots)
            {
                if (robot.Status != RobotStatus.Null)
                {
                    if (robot.Product != null)
                    {
                        robotsInfo.Add(new RobotInfo(
                        robot.Id,
                        robot.Product.Item1.Name,
                        robot.Product.Item2,
                        robot.Status));
                    }
                    else
                    {
                        robotsInfo.Add(new RobotInfo(
                        robot.Id, "Empty", 0, robot.Status));
                    }
                }
                else
                {
                    robotsInfo.Add(new RobotInfo(
                        robot.Id, "Null", 0, robot.Status));
                }
            }
            // Prevent scrolling reset
            int currentRow = 0;
            if (dataGridViewRobots.CurrentCell != null)
            {
                currentRow =
                    dataGridViewRobots.FirstDisplayedScrollingRowIndex;
            }
            dataGridViewRobots.DataSource = null;  // need this to work
            dataGridViewRobots.DataSource = this.robotsInfo;
            if (dataGridViewRobots.CurrentCell != null)
            {
                if (currentRow < 0)
                {
                    currentRow = 0;
                }
                dataGridViewRobots.FirstDisplayedScrollingRowIndex
                    = currentRow;
            }
            dataGridViewRobots.Columns["status"].Visible = false;

            for (int i = 0; i < this.robotsInfo.Count; i++)
            {
                DataGridViewRow row = dataGridViewRobots.Rows[i];
                switch (this.robotsInfo[i].Status)
                {
                    case RobotStatus.Null:
                        row.DefaultCellStyle.BackColor = Color.White;
                        break;
                    case RobotStatus.PickUp:
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        break;
                    case RobotStatus.DropOff:
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        break;
                    case RobotStatus.GoHome:
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateInventoryInfo(
            Dictionary<Product, int> inventoryDatabase,
            Dictionary<Product, int> shelfDatabase)
        {
            inventoryInfo.Clear();

            foreach (var productData in inventoryDatabase)
            {
                this.inventoryInfo.Add(new InventoryInfo(
                        productData.Key.Name, productData.Value,
                        shelfDatabase[productData.Key]));
            }
            // Prevent scrolling reset
            int currentRow = 0;
            if (dataGridViewInventory.CurrentCell != null)
            {
                currentRow =
                    dataGridViewInventory.FirstDisplayedScrollingRowIndex;
            }
            dataGridViewInventory.DataSource = null;  // need this to work
            dataGridViewInventory.DataSource = this.inventoryInfo;
            if (dataGridViewRobots.CurrentCell != null)
            {
                if (currentRow < 0)
                {
                    currentRow = 0;
                }
                dataGridViewInventory.FirstDisplayedScrollingRowIndex
                    = currentRow;
            }

            for (int i = 0; i < this.inventoryInfo.Count; i++)
            {
                DataGridViewRow row = dataGridViewInventory.Rows[i];
                if (this.inventoryInfo[i].EstimatedQty >  // Restocking
                    this.inventoryInfo[i].ActualQty)
                {
                    row.DefaultCellStyle.BackColor = Color.LightSalmon;
                }
                else if (this.inventoryInfo[i].EstimatedQty <  // Delivery
                         this.inventoryInfo[i].ActualQty)
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }

        private void UpdateOrdersInfo(List<Order> orders)
        {
            foreach (var order in orders)
            {
                string orderHeader = "Order: " + order.Id.ToString();
                string orderInfo = "Status: " + order.Status.ToString();
                TreeNode orderNode;
                if (treeViewOrders.Nodes.ContainsKey(orderHeader))
                {
                    orderNode = treeViewOrders.Nodes[orderHeader];
                    // Assume order product never changes
                    if (orderNode.Nodes[0].Text != orderInfo)
                    {
                        orderNode.Nodes[0].Remove();
                        orderNode.Nodes.Insert(0, orderInfo);
                    }
                }
                else
                {
                    orderNode = new TreeNode(orderHeader);
                    orderNode.Name = orderHeader;
                    orderNode.Nodes.Add(orderInfo);
                    foreach (var productData in order.Products)
                    {
                        string productInfo = productData.Key.Name + ": "
                            + productData.Value.ToString();
                        orderNode.Nodes.Add(productInfo);
                    }
                    treeViewOrders.Nodes.Add(orderNode);
                }
            }
        }

        private void InitializeMap()
        {
            // Add cells
            bool add_columns = true;
            for (int r = 0; r < sizeY - 1; r++)
            {
                for (int c = 0; c < sizeX; c++)
                {
                    if (add_columns)
                    {
                        DataGridViewImageColumn col =
                            new DataGridViewImageColumn();
                        col.ImageLayout =
                            DataGridViewImageCellLayout.Stretch;
                        dataGridViewMap.Columns.Add(col);  // this adds one row
                    }
                }
                add_columns = false;
                dataGridViewMap.Rows.Add();
            }
        }

        private void UpdateMap(List<Robot> robots, List<Dock> docks)
        {
            // Fill cells
            for (int r = 0; r < dataGridViewMap.Rows.Count; r++)
            {
                for (int c = 0; c < dataGridViewMap.Columns.Count; c++)
                {
                    DataGridViewImageCell cell = (DataGridViewImageCell)
                        dataGridViewMap.Rows[r].Cells[c];
                    bool has_robot = false;
                    bool is_delivery = false;
                    bool is_dock = false;
                    foreach (var robot in robots)
                    {
                        if ((robot.Location != null) &&
                            (robot.Location.Row == r) &&
                            (robot.Location.Column == c))
                        {
                            has_robot = true;
                            if (robot.Type == RobotType.Delivery)
                            {
                                is_delivery = true;
                            }
                            break;
                        }
                    }
                    foreach (var dock in docks)
                    {
                        if ((dock.Location.Row == r) &&
                            (dock.Location.Column == c))
                        {
                            is_dock = true;
                            break;
                        }
                    }
                    if (has_robot && is_dock)
                    {
                        if (is_delivery)
                        {
                            cell.Value =
                                (System.Drawing.Image)
                                Properties.Resources.RobotDockDelivery;
                        }
                        else
                        {
                            cell.Value =
                                (System.Drawing.Image)
                                Properties.Resources.RobotDockRestocking;
                        }
                    }
                    else if (has_robot)
                    {
                        if (is_delivery)
                        {
                            cell.Value =
                                (System.Drawing.Image)
                                Properties.Resources.RobotDelivery;
                        }
                        else
                        {
                            cell.Value =
                                (System.Drawing.Image)
                                Properties.Resources.RobotRestocking;
                        }
                    }
                    else if (is_dock)
                    {
                        cell.Value =
                            (System.Drawing.Image)Properties.Resources.Dock;
                    }
                    else
                    {
                        cell.Value =
                            (System.Drawing.Image)Properties.Resources.Tile;
                    }
                }
            }
        }

        public void buttonStartComputer_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            form2.VisibleChanged += form2VisibleChanged;
        }

        private void form2VisibleChanged(object sender, EventArgs e)
        {
            Form2 form2 = (Form2)sender;

            if (!form2.Visible)
            {
                this.sizeX = form2.sizeX;
                this.sizeY = form2.sizeY;
                this.numDock = form2.numDock;
                this.numRobot = form2.numRobot;
                string filename = form2.filename;

                computer = new CentralComputer(
                    sizeX, sizeY, numDock, numRobot,
                    maxShelfCap, maxShelfNum, maxTruckVolume,
                    maxTruckWeight, maxRobotCap, filename);

                robotsInfo = new List<RobotInfo>();
                inventoryInfo = new List<InventoryInfo>();

                InitializeMap();
                computer.Start();

                isComputerStarted = true;

                form2.Dispose();
                buttonStartComputer.Enabled = false;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (isComputerStarted)
            {
                UpdateRobotsInfo(computer.Robots);
                UpdateInventoryInfo(computer.InventoryDatabase,
                    computer.ShelfDatabase);
                UpdateOrdersInfo(computer.Orders);
                UpdateMap(computer.Robots, computer.WarehouseDock);
            }
        }

        private void dataGridViewMap_SelectionChanged(object sender,
            EventArgs e)
        {
            // Just to remove selection highlight
            dataGridViewMap.ClearSelection();
        }

        private void buttonOrder_Click(object sender, EventArgs e)
        {
            // Just to remove selection highlight
            computer.AddOrders();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxWarehouseID.Text) ||
                string.IsNullOrEmpty(textBoxServerIP.Text) ||
                string.IsNullOrEmpty(textBoxServerPort.Text))
            {
                MessageBox.Show("Please fill in infomation!");
                return;
            }

            if (isComputerStarted)
            {
                int warehouseId = Convert.ToInt32(textBoxWarehouseID.Text);
                string serverIp = textBoxServerIP.Text;
                int serverPort = Convert.ToInt32(textBoxServerPort.Text);

                if (!computer.ConnectToServer(warehouseId, serverIp, serverPort))
                    buttonConnect.Text = "Failed to Connect";
                else
                {
                    buttonConnect.Text = "Connected!";
                    buttonConnect.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Please start the computer!");
                return;
            }
        }

        private void FormAmazoom_FormClosing(object sender,
            FormClosingEventArgs e)
        {
            if(this.isComputerStarted)
                computer.DisconnectFromServer();
        }

        private void buttonAddRemoveItem_Click(object sender, EventArgs e)
        {
            if (isComputerStarted)
            {
                Form3 form3 = new Form3(this.computer.InventoryDatabase);
                form3.Show();
                form3.VisibleChanged += form3VisibleChanged;
            }
        }

        private void form3VisibleChanged(object sender, EventArgs e)
        {
            Form3 form3 = (Form3)sender;

            if (!form3.Visible)
            {
                string name = form3.name;

                if (form3.isAdd)
                {
                    double weight = form3.weight;
                    double volume = form3.volume;

                    if (!this.computer.AddToInventory(name, weight, volume))
                    {
                        string message = $"Failed to add new product.";
                        MessageBox.Show(message);

                        return;
                    }
                }
                else if (form3.isRemove)
                {
                    if(!this.computer.RemoveFromInventory(name))
                    {
                        string message = $"Failed to remove product.";
                        MessageBox.Show(message);

                        return;
                    }
                }

                form3.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            computer.AddRobot();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            computer.RemoveRobot();
        }

        private void FormAmazoom_Load(object sender, EventArgs e)
        {

        }
    }
}
