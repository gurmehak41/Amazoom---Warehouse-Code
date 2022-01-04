using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amazoom
{
    public partial class Form3 : Form
    {
        private const double minWeight = 0.005;
        private const double minVolume = 0.001;
        public string name { get; set; }
        public double weight { get; set; }
        public double volume { get; set; }
        public bool isAdd { get; set; }
        public bool isRemove { get; set; }

        private Dictionary<Product, int> inventoryDatabase;

        public Form3(Dictionary<Product, int> inventoryDatabase)
        {
            InitializeComponent();
            this.isAdd = false;
            this.isRemove = false;
            this.inventoryDatabase = inventoryDatabase;

            comboBoxRemove.Items.Clear();

            foreach (Product product in this.inventoryDatabase.Keys)
            {
                comboBoxRemove.Items.Add(product.Name);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (isAddInputValid())
            {
                this.name = textBoxName.Text;
                this.weight = Convert.ToDouble(textBoxWeight.Text);
                this.volume = Convert.ToDouble(textBoxVolume.Text);

                this.isAdd = true;
                this.Visible = false;
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (comboBoxRemove.SelectedItem == null)
            {
                string message = $"Please select the product to remove.";
                MessageBox.Show(message);

                return;
            }
            else
            {
                string message = $"This action will permanently remove the product " +
                    $"from warehouse.\n Do you wish to proceed?";
                string caption = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.Yes)
                {
                    this.name = comboBoxRemove.SelectedItem.ToString();
                    this.isRemove = true;
                    this.Visible = false;
                }
            }
        }

        private bool isAddInputValid()
        {
            if (String.IsNullOrEmpty(textBoxName.Text))
            {
                string message = $"Product name cannot be null or empty.";
                MessageBox.Show(message);

                return false;
            }
            else if (Convert.ToDouble(textBoxWeight.Text) < minWeight)
            {
                string message = $"Product mass cannot be smaller than {minWeight}kg.";
                MessageBox.Show(message);

                textBoxWeight.Text = minWeight.ToString();
                return false;
            }
            else if (Convert.ToDouble(textBoxVolume.Text) < minVolume)
            {
                string message = $"Product volume cannot be smaller than {minVolume}m^3";
                MessageBox.Show(message);

                textBoxVolume.Text = minVolume.ToString();
                return false;
            }

            foreach (Product product in this.inventoryDatabase.Keys)
            {
                if (textBoxName.Text == product.Name)
                {
                    string message = $"A product of same name exists. " +
                        $"Please enter a different name.";
                    MessageBox.Show(message);

                    return false;
                }
            }

            return true;
        }
    }
}
