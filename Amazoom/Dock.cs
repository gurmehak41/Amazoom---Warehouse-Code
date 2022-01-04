using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Amazoom
{
	public class Dock
	{
		public int Id
		{ get;}

		public Point Location
		{ get; }

		public SemaphoreSlim Busy
		{ get; }

		public Truck Truck
		{ get; set; }

		public Dock(int id, Point location)
		{
			this.Id = id;
			this.Location = location;
			this.Busy = new SemaphoreSlim(0, 1);
		}
	}
}
