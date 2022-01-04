using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazoom
{
    public class Map
    {
        private enum GeneratePathState
        {
            MoveUp,
            MoveLeft,
            MoveRight,
            MoveDown
        }

        public int NumRows
        { get; }

        public int NumColumns
        { get; }

        public Point Home
        { get; }

        public bool[,] Grid  // false represent no robot
        { get; set; }

        public Map(int cols, int rows, Point home)
        {
            this.NumColumns = cols;
            this.NumRows = rows;
            this.Home = home;

            this.Grid = new bool[this.NumRows, this.NumColumns];
        }

        private int GetNumRobots()
        {
            int numRobots = 0;
            for (int row = 0; row < this.NumRows; row++)
            {
                for (int col = 0; col < this.NumColumns; col++)
                {
                    if (Grid[row, col])
                    {
                        numRobots++;
                    }
                }
            }
            return numRobots;
        }

        private bool CheckIfArrive(Point current, Point goal)
        {
            return ((current.Row == goal.Row) &&
                    (current.Column == goal.Column));
        }

        /**
		 * \brief Generates clockwise path from start to end (non-inclusive of start).
		 * 
		 * Assume map has shelves on the left and right of each cells except cells
		 * on the four outer edges.
		 * 
		 * \param start Current location of robot. If null, assume home.
		 * \param end Destination location of robot.
		 * \param path Clockwise path.
		 * 
		 * \returns true if path is generated, false if there are more
		 *          than 2 * NumRows robot on the map due to possible deadlock.
		 */
        public bool GeneratePath(Point start, Point end, out List<Point> path)
        {
            path = new List<Point>();

            // Do not generate path if there is possible deadlock
/*            if (GetNumRobots() >= (2 * this.NumRows))
            {
                return false;
            }*/

            // Assume home starting point if start is null
            Point current;
            if (start == null)
            {
                path.Add(this.Home);
                current = new Point(this.Home.Row, this.Home.Column);
            }
            else
            {
                current = new Point(start.Row, start.Column);
            }

            // Determine starting state
            GeneratePathState currentState = GeneratePathState.MoveUp;
            if ((current.Row <= 0) &&
                (current.Column < this.NumColumns - 1))
            {
                currentState = GeneratePathState.MoveRight;
            }
            else if ((current.Row < this.NumRows - 1) &&
                     (current.Column == this.NumColumns - 1))
            {
                currentState = GeneratePathState.MoveDown;
            }
            else if ((current.Row == this.NumRows - 1) &&
                     (current.Column < end.Column))
            {
                currentState = GeneratePathState.MoveLeft;
            }

            while (true)
            {
                if (CheckIfArrive(current, end))
                {
                    return true;
                }

                switch (currentState)
                {
                    case GeneratePathState.MoveUp:
                        current = new Point(current.Row - 1, current.Column);
                        break;
                    case GeneratePathState.MoveLeft:
                        current = new Point(current.Row, current.Column - 1);
                        break;
                    case GeneratePathState.MoveRight:
                        current = new Point(current.Row, current.Column + 1);
                        break;
                    case GeneratePathState.MoveDown:
                        current = new Point(current.Row + 1, current.Column);
                        break;
                }

                path.Add(current);

                switch (currentState)
                {
                    case GeneratePathState.MoveUp:
                        if (current.Row <= 0)
                        {
                            currentState = GeneratePathState.MoveRight;
                        }
                        break;
                    case GeneratePathState.MoveLeft:
                        if ((current.Column <= 0) ||
                            (current.Column == end.Column))
                        {
                            currentState = GeneratePathState.MoveUp;
                        }       
                        break;
                    case GeneratePathState.MoveRight:
                        if (current.Column >= this.NumColumns - 1)
                        {
                            currentState = GeneratePathState.MoveDown;
                        }
                        break;
                    case GeneratePathState.MoveDown:
                        if (current.Row >= this.NumRows - 1)
                        {
                            currentState = GeneratePathState.MoveLeft;
                        }
                        break;
                }
            }
        }
    }
}
