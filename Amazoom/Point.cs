using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazoom
{
	public enum ShelfSide
    {
		Left,
		Right
    }

	public class Point
	{
		public int Row
		{ get; }

		public int Column
		{ get; }

		public ShelfSide ShelfSide
		{ get; }
		
		public int ShelfNum
		{ get; }

		public Point(int row, int col)
        {
			this.Row = row;
			this.Column = col;
        }

		public Point(int row, int col, ShelfSide shelfSide, int shelfNum)
        {
			this.Row = row;
			this.Column = col;
			this.ShelfSide = shelfSide;
			this.ShelfNum = shelfNum;
		}
	}
}
