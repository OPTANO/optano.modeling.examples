
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    /// <summary>
    /// A node of the traveling salesman problem network
    /// </summary>
    public class Node : INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class, representing a node of the traveling salesman problem
        /// </summary>
        /// <param name="name">
        /// Name of the node
        /// </param>
        /// <param name="isStartingNode">
        /// Is this node a starting node?
        /// </param>


        public Node(string name, bool isStartingNode)
        {
            this.Name = name;
            this.IsStartingNode = isStartingNode;
        }

        /// <summary>
        /// Gets the name of the node
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Indicates whether this node is a starting node (true) or not (false)
        /// </summary>
        public bool IsStartingNode { get; set; }

        /// <summary>
        /// Name of the node
        /// </summary>
        /// <returns>
        /// The name of this node (<see cref="string"/>).
        /// </returns>
        public override string ToString() => this.Name;

    }
}
