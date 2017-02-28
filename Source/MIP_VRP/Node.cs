using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP
{
    /// <summary>
    /// A node of the Multi Vehicle Routing Problem /w Capacity Constraints
    /// </summary>
    public class Node : INode
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Node"/> class, representing a node of the tsp
        /// </summary>
        /// <param name="name">
        /// Name of the node
        /// </param>
        /// <param name="demand">
        /// The demand of this node; A negative demand is supply
        /// </param>
        /// <param name"isDepot">
        /// Indicator if this node is a depot or not
        /// </param>
        public Node(string name, double demand, bool isDepot)
        {
            this.Name = name;
            this.Demand = demand;
            this.IsDepot = isDepot;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is a depot
        /// </summary>
        public bool IsDepot { get; set; }

        /// <summary>
        /// Gets the demand of this node; The depot has no demand but "infinite" supply
        /// </summary>
        public double Demand { get; }

        /// <summary>
        /// Gets the name of the node
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Name of the node
        /// </summary>
        /// <returns>
        /// The name of this node (<see cref="string"/>).
        /// </returns>
        public override string ToString() => this.Name;

    }
}
