using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TSP
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Enums;

    /// <summary>
    /// A traveling salesman model
    /// </summary>
    public class TravelingSalesmanModel
    {
       
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TravelingSalesmanModel"/> class and initializes all fields.
        /// </summary>
        /// <param name="nodes">
        /// The nodes of the model
        /// </param>
        /// <param name="edges">
        /// The edges of the model
        /// </param>
        public TravelingSalesmanModel(List<INode> nodes, List<IEdge> edges)
        {
            this.Nodes = nodes;
            this.Edges = edges;
         
            this.Model = new Model();

            // Edge-activation Variables
            this.y = new VariableCollection<IEdge>(
                this.Model,
                this.Edges,
                "y",
                edge => $"{edge}",
                edge => 0, // edge is not used
                edge => 1, // edge is used in the route
                edge => VariableType.Binary); // indicates whether the edge is used "1" or not "0"


            // Edge-activation Variables
            this.z = new VariableCollection<INode>(
                this.Model,
                this.Nodes,
                "z",
                node => $"{node.Name} ",
                node => 0, // edge is not used
                node => this.Nodes.Count + 1, // edge is used in the route
                node => VariableType.Integer); // indicates whether the edge is used "1" or not "0"


            // leave the starting node exactly once
            this.Model.AddConstraint(
                Expression.Sum(this.Edges.Where(edge => edge.FromNode.IsStartingNode == true)
                .Select(edge => this.y[edge]))
                == 1,
                $"leave starting node");

            // Exactly one incoming edge
            foreach (var node in this.Nodes)
            {
                // Add the Constraint to the model:
                this.Model.AddConstraint(
                    Expression.Sum(this.Edges.Where(edge => edge.ToNode == node).Select(edge => this.y[edge]))
                    == 1,
                    $"Exactly one incoming edge for node {node}");
            }
      
            // Exactly one outgoing edge
            foreach (var node in this.Nodes)
            {
                // Add the Constraint to the model:
                this.Model.AddConstraint(
                    Expression.Sum(this.Edges.Where(edge => edge.FromNode == node).Select(edge => this.y[edge]))
                    == 1,
                    $"Exactly one outgoing edge for node {node}");
            }

            // To eliminate Sub-tours in our TSP we need to add Sub tour elimination constraints

            // Iterate each Node as starting node
            foreach (var nodeI in this.Nodes.Where(node => !node.IsStartingNode))
            {
                // take all nodes, which can be travled to from nodeI
                foreach (var edge in this.Edges.Where(e => e.FromNode == nodeI))
                {
                    var nodeJ = edge.ToNode;
                    this.Model.AddConstraint(
                        z[nodeI] - z[nodeJ] + (this.Nodes.Count) * y[edge] <= this.Nodes.Count - 1, 
                        string.Format("Subtour Elemination for {0} {1}", nodeI.Name, nodeJ.Name)
                        );
                }
            }

          

            // Add the objective:
            // Sum of the distances between all used edges
            // \sum_{edge \in Edges} \{ x_{edge} * Distance_{edge} \}
            this.Model.AddObjective(
                new Objective(
                Expression.Sum(this.Edges.Select(edge => (y[edge] * edge.Distance))),
                "sum of all distances",
                ObjectiveSense.Minimize) // Minimize the sum of all distances.
            );
        }

        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the edges of this network
        /// </summary>
        public List<IEdge> Edges { get; }

        /// <summary>
        /// Gets the nodes of this network
        /// </summary>
        public List<INode> Nodes { get; }

        /// <summary>
        /// Gets the Collection of all edge activation variables
        /// </summary>
        public VariableCollection<IEdge> y { get; }

        /// <summary>
        /// Gets the collection of all node tour position variables
        /// </summary>
        public VariableCollection<INode> z { get; }
    }
}
