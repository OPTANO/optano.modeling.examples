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
    public class KnapsackItem
    {
        /// <summary>
        /// the weight of the item
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// the value of the item
        /// </summary>
        public double Value { get; set; }

        public string Name { get; set; }

        public override string ToString() => Name;
    }
}
