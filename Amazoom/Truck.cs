using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amazoom
{
    public enum TruckType
    {
        Restocking,
        Delivery
    }

    public class Truck
    {

        public TruckType Type
        { get; }

        public double MaxVolume
        { get; }

        public double MaxWeight
        { get; }

        public List<Order> RequestedOrders
        { get; }

        public Dictionary<Product, int> CurrentProducts  // type, quantity
        { get; }

        public double RemainingVolumeRequestedOrders
        {
            get
            {
                double remainingVolume = this.MaxVolume;
                foreach (var order in this.RequestedOrders)
                {
                    foreach (var productEntry in order.Products)
                    {
                        Product product = productEntry.Key;
                        int quantity = productEntry.Value;
                        remainingVolume -= (product.Volume * quantity);
                    }
                }
                return remainingVolume;  // negative to signify overfilled
            }
        }

        public double RemainingVolumeCurrentProducts
        {
            get
            {
                double remainingVolume = this.MaxVolume;
                foreach (var productEntry in this.CurrentProducts)
                {
                    Product product = productEntry.Key;
                    int quantity = productEntry.Value;
                    remainingVolume -= (product.Volume * quantity);
                }
                return remainingVolume;  // negative to signify overfilled
            }
        }

        public double RemainingWeightRequestedOrders
        {
            get
            {
                double remainingWeight = this.MaxWeight;
                foreach (var order in this.RequestedOrders)
                {
                    foreach (var productEntry in order.Products)
                    {
                        Product product = productEntry.Key;
                        int quantity = productEntry.Value;
                        remainingWeight -= (product.Weight * quantity);
                    }
                }
                return remainingWeight;  // negative to signify overfilled
            }
        }

        public double RemainingWeightCurrentProducts
        {
            get
            {
                double remainingWeight = this.MaxWeight;
                foreach (var productEntry in this.CurrentProducts)
                {
                    Product product = productEntry.Key;
                    int quantity = productEntry.Value;
                    remainingWeight -= (product.Weight * quantity);
                }
                return remainingWeight;  // negative to signify overweighted
            }
        }

        public Truck(TruckType type, double maxVolume, double maxWeight)
        {
            this.Type = type;
            this.MaxVolume = maxVolume;
            this.MaxWeight = maxWeight;

            this.RequestedOrders = new List<Order>();
            this.CurrentProducts = new Dictionary<Product, int>();
        }

        /**
         * \brief Checks whether restocking truck is empty.
         * 
         * \returns true if empty, false otherwise
         */
        public bool IsTruckEmpty()
        {
            return this.CurrentProducts.Count == 0;
        }

        /**
         * \brief Checks whether requested orders is empty.
         * 
         * \returns true if empty, false otherwise
         */
        public bool IsOrdersEmpty()
        {
            foreach (var order in this.RequestedOrders)
            {
                if (order.Products.Count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * \brief Checks whether delivery truck's requested order is fulfilled.
         * NOTE: Requested orders must be fulfilled exactly by current products.
         * 
         * \returns true if fulfilled, false otherwise
         */
        public bool IsOrdersFulfilled()
        {
            Dictionary<Product, int> currentProducts = this.CurrentProducts;
            foreach (var order in this.RequestedOrders)
            {
                foreach (var productEntry in order.Products)
                {
                    Product product = productEntry.Key;
                    int quantity = productEntry.Value;
                    if (currentProducts.TryGetValue(product, out int current)
                        && (current >= quantity))
                    {
                        currentProducts[product] = current - quantity;
                        if (current - quantity == 0)
                        {
                            currentProducts.Remove(product);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return currentProducts.Count == 0;
        }

        /**
         * \brief Adds order to delivery truck. This order should be
         *        fulfilled when this truck docked.
         * 
         * \param order order to be fulfilled
         * 
         * \returns true if truck is not overfilled, false otherwise
         */
        public bool AddRequestedOrder(Order order)
        {
            // Check whether order can be placed into truck
            double orderVolume = 0, orderWeight = 0;
            foreach (var productEntry in order.Products)
            {
                Product product = productEntry.Key;
                int quantity = productEntry.Value;
                orderVolume += (product.Volume * quantity);
                orderWeight += (product.Weight * quantity);
            }
            if ((this.RemainingVolumeRequestedOrders < orderVolume) ||
                (this.RemainingWeightRequestedOrders < orderWeight))
            {
                return false;
            }

            this.RequestedOrders.Add(order);
            return true;
        }

        /**
        * \brief Adds specified quantity of product to restocking truck.
        * 
        * \param product product to be added
        * \param quantity quantity to be added
        * 
        * \returns true if truck is not overfilled, false otherwise
        */
        public bool AddProduct(Product product, int quantity)
        {
            // Check whether product can be placed into truck
            if ((this.RemainingVolumeCurrentProducts < product.Volume * quantity) ||
                (this.RemainingWeightCurrentProducts < product.Weight * quantity))
            {
                return false;
            }

            if (this.CurrentProducts.TryGetValue(product, out int current))
            {
                this.CurrentProducts[product] = current + quantity;
            }
            else
            {
                this.CurrentProducts[product] = quantity;
            }
            return true;
        }

        /**
         * \brief Removes specified quantity of product from restocking truck.
         * 
         * \param product product to be removed
         * \param quantity quantity to be removed
         * 
         * \returns true if product of specified quantity exists,
         *          false otherwise
         */
        public bool RemoveProduct(Product product, int quantity)
        {
            if (this.CurrentProducts.TryGetValue(product, out int current) &&
                (current >= quantity))
            {
                this.CurrentProducts[product] = current - quantity;
                if (current - quantity == 0)
                {
                    this.CurrentProducts.Remove(product);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
