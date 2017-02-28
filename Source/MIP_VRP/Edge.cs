using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP
{
    /// <summary>
    /// An edge between two nodes of the Multi Vehicle Routing Problem /w Capacity Constraints
    /// </summary>
    public class Edge : IEdge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class
        /// </summary>
        /// <param name="fromNode">
        /// The departing node
        /// </param>
        /// <param name="toNode">
        /// The arrival node
        /// </param>
        /// <param name="transportationCost">
        /// The length of the edge
        /// </param>
        public Edge(INode fromNode, INode toNode, double? transportationCost)
        {
            // set the parameter information
            this.FromNode = fromNode;
            this.ToNode = toNode;
            this.transportationCost = transportationCost;
        }

        /// <summary>
        /// Gets the capacity of the edge
        /// </summary>
        public double? transportationCost { get; }

        /// <summary>
        /// Gets the end node of the edge
        /// </summary>
        public INode ToNode { get; }

        /// <summary>
        /// Gets the start node of the edge
        /// </summary>
        public INode FromNode { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the edge is used in the solution
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Name of the edge
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString() => $"Edge {this.FromNode} to {this.ToNode}";
       
    }
}
