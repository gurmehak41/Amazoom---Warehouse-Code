using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amazoom
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormAmazoom());
            

            // For testing CentralComputer, Inventory, and Shelf
            //CentralComputer centralComputer = new CentralComputer(5, 5, 2, 4, 200, 20, 3000, 100, "../../testProduct.txt");
            //centralComputer.SystemIntegrationTesting();

            //For debugging of generatePath function - please ignore in final code
            /*
            Point myHome = new Point();
            Point s = new Point();
            Point e = new Point();

            List<Point> myPath = new List<Point>();

            s.row = 3;
            s.column = 0;

            e.row = 3;
            e.column = 2;

            Map myMap = new Map(6, 6, myHome);

            myMap.generatePath(s, e,out myPath);

            int es = 0;
            */
        }
    }
}
