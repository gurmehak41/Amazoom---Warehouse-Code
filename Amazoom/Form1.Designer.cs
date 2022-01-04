
namespace Amazoom
{
    partial class FormAmazoom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonStartComputer = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewRobots = new System.Windows.Forms.DataGridView();
            this.labelRobots = new System.Windows.Forms.Label();
            this.labelInventory = new System.Windows.Forms.Label();
            this.dataGridViewInventory = new System.Windows.Forms.DataGridView();
            this.labelOrders = new System.Windows.Forms.Label();
            this.treeViewOrders = new System.Windows.Forms.TreeView();
            this.labelCredit = new System.Windows.Forms.Label();
            this.dataGridViewMap = new System.Windows.Forms.DataGridView();
            this.buttonOrder = new System.Windows.Forms.Button();
            this.labelWarehouseID = new System.Windows.Forms.Label();
            this.textBoxWarehouseID = new System.Windows.Forms.TextBox();
            this.textBoxServerIP = new System.Windows.Forms.TextBox();
            this.labelServerIP = new System.Windows.Forms.Label();
            this.textBoxServerPort = new System.Windows.Forms.TextBox();
            this.labelServerPort = new System.Windows.Forms.Label();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonAddRemoveItem = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRobots)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMap)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStartComputer
            // 
            this.buttonStartComputer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartComputer.Location = new System.Drawing.Point(14, 61);
            this.buttonStartComputer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonStartComputer.Name = "buttonStartComputer";
            this.buttonStartComputer.Size = new System.Drawing.Size(250, 28);
            this.buttonStartComputer.TabIndex = 13;
            this.buttonStartComputer.Text = "Start Warehouse";
            this.buttonStartComputer.UseVisualStyleBackColor = true;
            this.buttonStartComputer.Click += new System.EventHandler(this.buttonStartComputer_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // dataGridViewRobots
            // 
            this.dataGridViewRobots.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRobots.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRobots.Location = new System.Drawing.Point(14, 125);
            this.dataGridViewRobots.Name = "dataGridViewRobots";
            this.dataGridViewRobots.RowHeadersVisible = false;
            this.dataGridViewRobots.RowHeadersWidth = 82;
            this.dataGridViewRobots.Size = new System.Drawing.Size(250, 150);
            this.dataGridViewRobots.TabIndex = 16;
            // 
            // labelRobots
            // 
            this.labelRobots.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRobots.Location = new System.Drawing.Point(14, 95);
            this.labelRobots.Name = "labelRobots";
            this.labelRobots.Size = new System.Drawing.Size(250, 28);
            this.labelRobots.TabIndex = 17;
            this.labelRobots.Text = "Robots";
            this.labelRobots.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelInventory
            // 
            this.labelInventory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInventory.Location = new System.Drawing.Point(270, 95);
            this.labelInventory.Name = "labelInventory";
            this.labelInventory.Size = new System.Drawing.Size(96, 28);
            this.labelInventory.TabIndex = 18;
            this.labelInventory.Text = "Inventory";
            this.labelInventory.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dataGridViewInventory
            // 
            this.dataGridViewInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewInventory.Location = new System.Drawing.Point(274, 125);
            this.dataGridViewInventory.Name = "dataGridViewInventory";
            this.dataGridViewInventory.RowHeadersVisible = false;
            this.dataGridViewInventory.RowHeadersWidth = 82;
            this.dataGridViewInventory.Size = new System.Drawing.Size(250, 150);
            this.dataGridViewInventory.TabIndex = 19;
            // 
            // labelOrders
            // 
            this.labelOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrders.Location = new System.Drawing.Point(530, 95);
            this.labelOrders.Name = "labelOrders";
            this.labelOrders.Size = new System.Drawing.Size(250, 28);
            this.labelOrders.TabIndex = 20;
            this.labelOrders.Text = "Orders";
            this.labelOrders.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // treeViewOrders
            // 
            this.treeViewOrders.Location = new System.Drawing.Point(534, 125);
            this.treeViewOrders.Name = "treeViewOrders";
            this.treeViewOrders.Size = new System.Drawing.Size(251, 150);
            this.treeViewOrders.TabIndex = 21;
            // 
            // labelCredit
            // 
            this.labelCredit.Location = new System.Drawing.Point(15, 684);
            this.labelCredit.Name = "labelCredit";
            this.labelCredit.Size = new System.Drawing.Size(291, 13);
            this.labelCredit.TabIndex = 22;
            this.labelCredit.Text = "All images are obtained under Creative Commons License";
            this.labelCredit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridViewMap
            // 
            this.dataGridViewMap.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewMap.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewMap.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMap.ColumnHeadersVisible = false;
            this.dataGridViewMap.Location = new System.Drawing.Point(14, 281);
            this.dataGridViewMap.Name = "dataGridViewMap";
            this.dataGridViewMap.RowHeadersVisible = false;
            this.dataGridViewMap.RowHeadersWidth = 82;
            this.dataGridViewMap.Size = new System.Drawing.Size(772, 400);
            this.dataGridViewMap.TabIndex = 23;
            this.dataGridViewMap.SelectionChanged += new System.EventHandler(this.dataGridViewMap_SelectionChanged);
            // 
            // buttonOrder
            // 
            this.buttonOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOrder.Location = new System.Drawing.Point(534, 61);
            this.buttonOrder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOrder.Name = "buttonOrder";
            this.buttonOrder.Size = new System.Drawing.Size(251, 28);
            this.buttonOrder.TabIndex = 24;
            this.buttonOrder.Text = "Test Random Order";
            this.buttonOrder.UseVisualStyleBackColor = true;
            this.buttonOrder.Click += new System.EventHandler(this.buttonOrder_Click);
            // 
            // labelWarehouseID
            // 
            this.labelWarehouseID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWarehouseID.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.labelWarehouseID.Location = new System.Drawing.Point(10, 11);
            this.labelWarehouseID.Name = "labelWarehouseID";
            this.labelWarehouseID.Size = new System.Drawing.Size(99, 28);
            this.labelWarehouseID.TabIndex = 25;
            this.labelWarehouseID.Text = "Warehouse ID";
            this.labelWarehouseID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxWarehouseID
            // 
            this.textBoxWarehouseID.Location = new System.Drawing.Point(114, 11);
            this.textBoxWarehouseID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxWarehouseID.Multiline = true;
            this.textBoxWarehouseID.Name = "textBoxWarehouseID";
            this.textBoxWarehouseID.Size = new System.Drawing.Size(150, 28);
            this.textBoxWarehouseID.TabIndex = 26;
            this.textBoxWarehouseID.Text = "1";
            // 
            // textBoxServerIP
            // 
            this.textBoxServerIP.Location = new System.Drawing.Point(374, 11);
            this.textBoxServerIP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxServerIP.Multiline = true;
            this.textBoxServerIP.Name = "textBoxServerIP";
            this.textBoxServerIP.Size = new System.Drawing.Size(150, 28);
            this.textBoxServerIP.TabIndex = 28;
            this.textBoxServerIP.Text = "127.0.0.1";
            // 
            // labelServerIP
            // 
            this.labelServerIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServerIP.Location = new System.Drawing.Point(270, 11);
            this.labelServerIP.Name = "labelServerIP";
            this.labelServerIP.Size = new System.Drawing.Size(99, 28);
            this.labelServerIP.TabIndex = 27;
            this.labelServerIP.Text = "Server IP";
            this.labelServerIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxServerPort
            // 
            this.textBoxServerPort.Location = new System.Drawing.Point(635, 11);
            this.textBoxServerPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxServerPort.Multiline = true;
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Size = new System.Drawing.Size(150, 28);
            this.textBoxServerPort.TabIndex = 30;
            this.textBoxServerPort.Text = "8911";
            // 
            // labelServerPort
            // 
            this.labelServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelServerPort.Location = new System.Drawing.Point(530, 11);
            this.labelServerPort.Name = "labelServerPort";
            this.labelServerPort.Size = new System.Drawing.Size(100, 28);
            this.labelServerPort.TabIndex = 29;
            this.labelServerPort.Text = "Server Port";
            this.labelServerPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Location = new System.Drawing.Point(274, 61);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(250, 28);
            this.buttonConnect.TabIndex = 31;
            this.buttonConnect.Text = "Connect to Server";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonAddRemoveItem
            // 
            this.buttonAddRemoveItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAddRemoveItem.Location = new System.Drawing.Point(370, 95);
            this.buttonAddRemoveItem.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonAddRemoveItem.Name = "buttonAddRemoveItem";
            this.buttonAddRemoveItem.Size = new System.Drawing.Size(152, 29);
            this.buttonAddRemoveItem.TabIndex = 32;
            this.buttonAddRemoveItem.Text = "Add/Remove Item";
            this.buttonAddRemoveItem.UseVisualStyleBackColor = true;
            this.buttonAddRemoveItem.Click += new System.EventHandler(this.buttonAddRemoveItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(176, 95);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 25);
            this.button1.TabIndex = 33;
            this.button1.Text = "Remove Robot";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(78, 95);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 26);
            this.button2.TabIndex = 34;
            this.button2.Text = "Add Robot";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormAmazoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 701);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonAddRemoveItem);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.textBoxServerPort);
            this.Controls.Add(this.labelServerPort);
            this.Controls.Add(this.textBoxServerIP);
            this.Controls.Add(this.labelServerIP);
            this.Controls.Add(this.textBoxWarehouseID);
            this.Controls.Add(this.labelWarehouseID);
            this.Controls.Add(this.buttonOrder);
            this.Controls.Add(this.dataGridViewMap);
            this.Controls.Add(this.labelCredit);
            this.Controls.Add(this.treeViewOrders);
            this.Controls.Add(this.labelOrders);
            this.Controls.Add(this.dataGridViewInventory);
            this.Controls.Add(this.labelInventory);
            this.Controls.Add(this.labelRobots);
            this.Controls.Add(this.dataGridViewRobots);
            this.Controls.Add(this.buttonStartComputer);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(810, 1485);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(647, 413);
            this.Name = "FormAmazoom";
            this.Text = "Amazoom Warehouse Portal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAmazoom_FormClosing);
            this.Load += new System.EventHandler(this.FormAmazoom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRobots)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

		#endregion
		private System.Windows.Forms.Button buttonStartComputer;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.DataGridView dataGridViewRobots;
        private System.Windows.Forms.Label labelRobots;
        private System.Windows.Forms.Label labelInventory;
        private System.Windows.Forms.DataGridView dataGridViewInventory;
        private System.Windows.Forms.Label labelOrders;
        private System.Windows.Forms.TreeView treeViewOrders;
        private System.Windows.Forms.Label labelCredit;
        private System.Windows.Forms.DataGridView dataGridViewMap;
        private System.Windows.Forms.Button buttonOrder;
        private System.Windows.Forms.Label labelWarehouseID;
        private System.Windows.Forms.TextBox textBoxWarehouseID;
        private System.Windows.Forms.TextBox textBoxServerIP;
        private System.Windows.Forms.Label labelServerIP;
        private System.Windows.Forms.TextBox textBoxServerPort;
        private System.Windows.Forms.Label labelServerPort;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonAddRemoveItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

