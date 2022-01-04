using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Amazoom
{
    class Inventory
    {
        public ConcurrentDictionary<Product, int> productKeyDatabase;

        public Inventory()
        {
            productKeyDatabase = new ConcurrentDictionary<Product, int>();
        }

        /**
         * \brief  Stocks the specified quantity of product to the inventory.
         * 
         * \param  product  Product to be stored
         * \param  quantity Number of product to be stored
         * 
         * \return bool     True if successful
         *                  false otherwise
         **/
        public bool Add(Product product, int quantity)
        {
            if (productKeyDatabase.ContainsKey(product))
                productKeyDatabase[product] += quantity;
            else
                productKeyDatabase.TryAdd(product, quantity);

            return true;
        }

        /**
         * \brief  Removes the specified quantity of product from the
         *         inventory.
         * 
         * \param  product  Product to be removed
         * \param  quantity Number of product to be removed
         * 
         * \return bool     True if successful
         *                  false otherwise
         **/
        public bool Remove(Product product, int quantity)
        {
            if (productKeyDatabase.ContainsKey(product))
            {
                if (productKeyDatabase[product] < quantity)
                {
                    return false;
                }
                productKeyDatabase[product] -= quantity;
                return true;
            }

            return false;
        }

        /**
         * \brief  Checks whether the product has the required quantity.
         * 
         * \param  product  Product to be checked
         * \param  quantity Required quantity of product
         * 
         * \return bool     True if product has the required quantity
         *                  false otherwise
         **/
        public bool CheckProductQty(Product product, int quantity)
        {
            if (productKeyDatabase.ContainsKey(product))
            {
                if (productKeyDatabase[product] >= quantity)
                    return true;
            }

            return false;
        }

        /**
         * \brief  Checks for the current quantity of the product.
         * 
         * \param  product  Product name
         * \param  quantity Current quantity of product
         * 
         * \return bool     True if product quantity is non-zero
         *                  False otherwise
         **/
        public bool CheckProductQty(string itemName, out int quantity)
        {
            int q = 0;

            foreach (KeyValuePair<Product,int> product in productKeyDatabase)
            {
                if (itemName == product.Key.Name)
                    q = product.Value;
            }

            if (q == 0)
            {
                quantity = q;
                return false;
            }
            else
            {
                quantity = q;
                return true;
            }
        }

        /**
         * \brief  Checks for the current quantity of the product.
         * 
         * \param  product  Product name
         * \param  quantity Current quantity of product
         * 
         * \return bool     True if product quantity is non-zero
         *                  False otherwise
         **/
        public bool GetProduct(string itemName, out Product product)
        {
            foreach (KeyValuePair<Product, int> productData
                in this.productKeyDatabase)
            {
                if (itemName == productData.Key.Name)
                {
                    product = productData.Key;
                    return true;
                }
            }

            product = new Product("null", 0.0, 0.0);
            return false;
        }

        /**
         * \brief  Deletes the product of given name from the warehouse.
         *         ONLY to be used via admin UI.
         *         
         * \param  product  Product name
         * 
         * \return bool     True if product is successfully removed
         *                  False otherwise
         **/
        public bool DeleteProductFromWarehouse(string itemName)
        {
            foreach (Product product in this.productKeyDatabase.Keys)
            {
                if (itemName == product.Name)
                {
                    if(this.productKeyDatabase.TryRemove(product, out _))
                        return true;
                }
            }

            return false;
        }

        /**
         * \brief: ONLY FOR TESTING PURPOSES
         **/
        public void ShowProducts()
        {
            Debug.WriteLine("Inventory Product Database:");

            foreach(KeyValuePair<Product, int> data in productKeyDatabase)
            {
                Debug.WriteLine($"{data.Value} {data.Key.Name} found.");
            }
        }
    }
}
