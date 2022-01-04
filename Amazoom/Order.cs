using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazoom
{
    public enum OrderStatus
    {
        Denied,
        Waiting,
        Accepted,
        InProgress,
        Delivered
    }

    public class Order
    {
        public int Id
        { get; }

        public Dictionary<Product, int> Products  // type, number
        { get; }

        public OrderStatus Status
        { get; set; }

        public Order(int id)
        {
            this.Id = id;
            this.Products = new Dictionary<Product, int>();
            this.Status = OrderStatus.Waiting;
        }

        /**
         * \brief Add product of specified quantity to order.
         * 
         * \param product product to be added
         * \quantity quantity of product to be added
         */
        public void Add(Product product, int quantity)
        {
            if (this.Products.TryGetValue(product, out int current))
            {
                this.Products[product] = current + quantity;
            }
            else
            {
                this.Products[product] = quantity;
            }
        }
    }
}
