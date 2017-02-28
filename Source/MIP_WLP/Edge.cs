using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLP
{
    /// <summary>
    /// An edge between two nodes of the network. 
    /// </summary>
    public class Edge : IEdge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="fromNode">
        /// The departing node
        /// </param>
        /// <param name="toNode">
        /// The arrival node
        /// </param>
        /// <param name="capacity">
        /// The capacity of the edge
        /// </param>
        /// <param name="costPerFlowUnit">
        /// The cost per flow unit
        /// </param>
        public Edge(INode fromNode, INode toNode, double capacity, double costPerFlowUnit)
        {
            // set the parameter information
            this.FromNode = fromNode;
            this.ToNode = toNode;
            this.Capacity = capacity;
            this.CostPerFlowUnit = costPerFlowUnit;
        }

        /// <summary>
        /// Gets the cost per flow unit on this edge
        /// </summary>
        public double CostPerFlowUnit { get; }

        /// <summary>
        /// Gets the capacity of the edge
        /// </summary>
        public double Capacity { get; }

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
        /// Gets or sets the flow of the optimal solution
        /// </summary>
        public double Flow { get; set; }

        /// <summary>
        /// Name of the edge
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString() => $"Edge {this.FromNode} to {this.ToNode}";
       
    }
}
