using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazoom
{
	public class Packet
	{
		public int Id
		{ get; }

		public Order Order
		{ get; }

		public Packet(int id, Order order,
			Tuple<Product, int> itemStockReq,
			Tuple<Product, int> lowStockAlert)
		{
			this.Id = id;
			this.Order = order;
		}
	}
}
