﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLP
{
    /// <summary>
    /// A node of the Warehouse Location Problem
    /// </summary>
    public class Node : INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class, 
        /// representing a node of the Warehouse Location Problem
        /// </summary>
        /// <param name="name">
        /// Name of the node
        /// </param>
        /// <param name="demand">
        /// The demand of this node. A negative demand is supply. 
        /// </param>
        /// <param name="creationCost">
        /// The cost for opening this facility
        /// </param>
        public Node(string name, double demand, double creationCost)
        {
            this.Name = name;
            this.Demand = demand;
            this.CreationCost = creationCost;
        }

        /// <summary>
        /// Gets the creationCost of this node. 0 means it is not a facility.
        /// </summary>
        public double CreationCost { get; }
        /// <summary>
        /// Gets the demand of this node. A negative demand is supply. 
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
