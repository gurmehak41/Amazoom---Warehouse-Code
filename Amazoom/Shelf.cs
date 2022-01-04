using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Amazoom
{
    public struct ProductKeyDatabaseValue
    {
        public Point location;
        public int quantity;
    }

    public struct PointKeyDatabaseValue
    {
        public Product product;
        public int quantity;
        public double occupiedCapacity;
    }

    public class Shelf
    {
        public readonly ConcurrentDictionary<Product, ProductKeyDatabaseValue>
            ProductKeyDatabase;
        public readonly ConcurrentDictionary<Point, PointKeyDatabaseValue>
            PointKeyDatabase;

        private double maximumCapacity;  // kg

        public Shelf(double maximumCapacity)
        {
            this.maximumCapacity = maximumCapacity;

            this.ProductKeyDatabase = new ConcurrentDictionary<
                Product, ProductKeyDatabaseValue>();
            this.PointKeyDatabase = new ConcurrentDictionary<
                Point, PointKeyDatabaseValue>();
        }

        /**
         * \brief Add new location for shelf.
         * 
         * \param point Location to be added
         */
        public void AddNewPoint(Point point)
        {
            this.PointKeyDatabase.TryAdd(point, new PointKeyDatabaseValue());
        }

        /**
         * \brief  Stocks the specified quantity of product to the shelf.
         * 
         * \param  point    Location of the shelf to be modified, must exist
         * \param  product  Product to be stored
         * \param  quantity Number of product to be stored
         * 
         * \return bool     True if successful
         *                  false otherwise
         **/
        public bool Add(Point point, Product product, int quantity)
        {
            PointKeyDatabaseValue newPointData = PointKeyDatabase[point];

            if (newPointData.product == product)
            {
                newPointData.quantity += quantity;
                newPointData.occupiedCapacity += quantity * product.Weight;
                if (newPointData.occupiedCapacity > this.maximumCapacity)
                {
                    return false;
                }
                PointKeyDatabase[point] = newPointData;
            }
            else
            {
                newPointData.product = product;
                newPointData.quantity = quantity;
                newPointData.occupiedCapacity = quantity * product.Weight;
                if (newPointData.occupiedCapacity > this.maximumCapacity)
                {
                    return false;
                }
                PointKeyDatabase[point] = newPointData;
            }

            if (ProductKeyDatabase.ContainsKey(product))
            {
                ProductKeyDatabaseValue newProductData
                    = ProductKeyDatabase[product];
                newProductData.quantity += quantity;
                ProductKeyDatabase[product] = newProductData;
            }
            else
            {
                ProductKeyDatabaseValue newProductData
                    = new ProductKeyDatabaseValue();
                newProductData.location = point;
                newProductData.quantity = quantity;
                ProductKeyDatabase.TryAdd(product, newProductData);
            }

            return true;
        }

        /**
         * \brief  Removes the specified quantity of product from the shelf.
         * 
         * \param  point    Location of the shelf to be modified, must exist
         * \param  product  Product to be removed, must exist
         * \param  quantity Number of product to be removed
         * 
         * \return bool     True if successful
         *                  false otherwise
         **/
        public bool Remove(Point point, Product product, int quantity)
        {
            PointKeyDatabaseValue newPointData = PointKeyDatabase[point];
            newPointData.quantity -= quantity;
            newPointData.occupiedCapacity -= quantity * product.Weight;
            if (newPointData.occupiedCapacity < 0)
            {
                return false;
            }

            ProductKeyDatabaseValue newProductData
                = ProductKeyDatabase[product];
            newProductData.quantity -= quantity;

            PointKeyDatabase[point] = newPointData;
            ProductKeyDatabase[product] = newProductData;

            return true;
        }

        /**
         * \brief  Gets the location of where the product is stored.
         * 
         * \param  product      Product to be searched
         * \param  out point    Location of the product
         * 
         * \return bool     True if product is successfully found
         *                  false otherwise
         **/
        public bool GetPoint(Product product, out Point point)
        {
            if (!ProductKeyDatabase.ContainsKey(product))
            {
                point = null;
                return false;
            }
            else
            {
                point = this.ProductKeyDatabase[product].location;
                return true;
            }
        }

        /**
         * \brief  Gets the location of an empty shelf.
         * 
         * \param  out point    Location of the shelf
         * 
         * \return bool     True if empty shelf is found
         *                  false otherwise
         **/
        public bool GetEmptyPoint(out Point point)
        {
            foreach (KeyValuePair<Point, PointKeyDatabaseValue> data in
                PointKeyDatabase)
            {
                if (data.Value.product == null)
                {
                    point = data.Key;
                    return true;
                }
            }

            point = null;
            return false;
        }

        /**
         * \brief: ONLY FOR TESTING PURPOSES
         **/
        public void ShowProducts()
        {
            Debug.WriteLine("Shelf Point Key Database:");

            foreach (KeyValuePair<Point, PointKeyDatabaseValue> data in
                PointKeyDatabase)
            {
                if (data.Value.product != null)
                {
                    Debug.WriteLine($"{data.Value.quantity} " +
                        $"{data.Value.product.Name} found at [r: {data.Key.Row}" +
                        $", c: {data.Key.Column}, side: {data.Key.ShelfSide}" +
                        $", height: {data.Key.ShelfNum}.");
                }
            }
        }
    }
}
