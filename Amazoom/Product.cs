using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amazoom
{
    public class Product
    {
        public string Name
        { get; }

        public double Volume
        { get; }

        public double Weight
        { get; }

        public Product(string name, double volume, double weight)
        {
            this.Name = name;
            this.Volume = volume;
            this.Weight = weight;
        }
    }
}
