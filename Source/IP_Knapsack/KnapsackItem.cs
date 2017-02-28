using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack
{   
    /// <summary>
    /// creates an item for the Knapsack Problem
    /// </summary>
    public class KnapsackItem : IKnapsackItem
    {

        public KnapsackItem (string name, double weight, double value)
        {
            this.Name = name;
            this.Weight = weight;
            this.Value = value;
        }
        /// <summary>
        /// the weight of the item
        /// </summary>
        public double Weight { get; }

        /// <summary>
        /// the value of the item
        /// </summary>
        public double Value { get; }

        public string Name { get; }

        public override string ToString() => this.Name;
    }
}
