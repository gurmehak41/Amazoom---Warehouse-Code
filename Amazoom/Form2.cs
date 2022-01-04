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
    public partial class Form2 : Form
    {
        private readonly int minX = 3;
        private readonly int minY = 3;
        private readonly int minDock = 1;
        private readonly int minRobot = 1;

        public int sizeX { get; set; }
        public int sizeY { get; set; }
        public int numDock { get; set; }
        public int numRobot { get; set; }
        public string filename { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if (isInputValid())
            {
                OpenFileDialog openFiledialog = new OpenFileDialog();
                openFiledialog.Filter = "txt files (*.txt)|*.txt";
                openFiledialog.Title = "Select inventory text file";
                openFiledialog.RestoreDirectory = true;

                if (openFiledialog.ShowDialog() == DialogResult.OK)
                {
                    this.sizeX = Convert.ToInt32(textBoxX.Text);
                    this.sizeY = Convert.ToInt32(textBoxY.Text);
                    this.numDock = Convert.ToInt32(textBoxNumDock.Text);
                    this.numRobot = Convert.ToInt32(textBoxNumRobot.Text);
                    this.filename = openFiledialog.FileName;

                    this.Visible = false;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool isInputValid()
        {
            if (Convert.ToInt32(textBoxX.Text) < minX)
            {
                string message = $"Warehouse size in X must not be smaller " +
                    $"than {minX.ToString()}";
                MessageBox.Show(message);

                textBoxX.Text = minX.ToString();
                return false;
            }
            else if (Convert.ToInt32(textBoxY.Text) < minY)
            {
                string message = $"Warehouse size in Y must not be smaller " +
                    $"than {minY.ToString()}";
                MessageBox.Show(message);

                textBoxY.Text = minY.ToString();
                return false;
            }
            else if (Convert.ToInt32(textBoxNumDock.Text) < minDock)
            {
                string message = $"Number of docks must not be smaller " +
                    $"than {minDock.ToString()}";
                MessageBox.Show(message);

                textBoxNumDock.Text = minDock.ToString();
                return false;
            }
            else if (Convert.ToInt32(textBoxNumRobot.Text) < minRobot)
            {
                string message = $"Number of robots must not be smaller " +
                    $"than {minRobot.ToString()}";
                MessageBox.Show(message);

                textBoxNumRobot.Text = minRobot.ToString();
                return false;
            }

            if (!isDockNumValid())
            {
                return false;
            }

            if (!isRobotNumValid())
            {
                return false;
            }

            return true;
        }

        private bool isDockNumValid()
        {
            if (Convert.ToInt32(textBoxNumDock.Text) >= 
                Convert.ToInt32(textBoxX.Text))
            {
                string message = $"Number of dock must be smaller than" +
                    $" warehouse size in X";
                MessageBox.Show(message);

                textBoxNumDock.Text = 
                    (Convert.ToInt32(textBoxY.Text) - 1).ToString();

                return false;
            }

            return true;
        }

        private bool isRobotNumValid()
        {
            if (Convert.ToInt32(textBoxNumRobot.Text) >= 
                (Convert.ToInt32(textBoxX.Text) *
                (Convert.ToInt32(textBoxY.Text))))
            {
                string message = $"Number of robots must be smaller than " +
                    $"warehouse area size";
                MessageBox.Show(message);

                textBoxNumRobot.Text = 
                    (Convert.ToInt32(textBoxY.Text) * 2 - 1).ToString();

                return false;
            }

            return true;
        }
    }
}
