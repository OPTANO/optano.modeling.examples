using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WLP
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Operators;
    using OPTANO.Modeling.Optimization.Enums;

   /// <summary>
    /// A warehouse location model
    /// </summary>
    public class WarehouseLocationModel
    {
       
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="WarehouseLocationModel"/> class and initializes all fields. 
        /// </summary>
        /// <param name="nodes">
        /// The network nodes of the model
        /// </param>
        /// <param name="edges">
        /// The edges of the model
        /// </param>
        public WarehouseLocationModel(List<INode> nodes, List<IEdge> edges)
        {
            this.Nodes = nodes;
            this.Edges = edges;
         
            this.Model = new Model();
               
            // Supply amount variable
            this.x = new VariableCollection<IEdge>(
                this.Model,
                this.Edges,
                "x", // the name of the variable collection
                edge => $"Supply flowing from {edge.FromNode} to {edge.ToNode}",
                edge => 0, 
                edge => edge.Capacity,
                edge => VariableType.Continuous);

            // Depot activation variable
            this.y = new VariableCollection<INode>(
               this.Model, 
               this.Nodes, 
               "y",
               node => $"{node.Name} status:",
               node => 0, // Warehouse is not used
               node => 1, // Warehouse is used
               node => VariableType.Binary); 
                
            // Create Constraints

            // if any edge is unbounded, take the highest of all demands as Big M
            // var bigM = nodes.Where(node => node.Demand > 0).Max(node => node.Demand);


            // fulfill all demands
            foreach (var node in this.Nodes)
            {
                if (node.Demand > 0)
                {
                    // Add Constraint to model
                    this.Model.AddConstraint(
                        // sum of transported goods to customers
                        Expression.Sum(this.Edges.Where(edge => edge.ToNode == node).Select(edge => this.x[edge]))
                        == node.Demand,
                        $"Fulfill the demand of node, {node}");
                }
            }

            // open facility if edge used
            foreach (var node in this.Nodes)
            {
                if (node.CreationCost > 0)
                {
                    // Add Constraint to model
                    this.Model.AddConstraint(
                        // edge activation for outgoing warehouse edges
                        Expression.Sum(this.Edges.Where(edge => edge.FromNode == node) .Select(edge => this.x[edge]))
                        <= this.y[node] * Math.Abs(node.Demand), // take the absolute value
                        $"Force facility {node} to open if outgoing edge is used");
                }
            }

            // Add the objective
            // Sum of all flows times the flow-unit-cost
            // + all warehouse opening decisions and their respective costs.
            // \sum_{edge \in Edges} \{ x_{edge} * costPerFlowUnit_{edge} 
            // + y_{node} * creationCost_{node} \}
            this.Model.AddObjective(
                new Objective(Expression.Sum(this.Edges.Select(edge => x[edge] * edge.CostPerFlowUnit)) +
                              Expression.Sum(this.Nodes.Select(node => y[node] * node.CreationCost)),
                "sum of all cost",
                ObjectiveSense.Minimize) // minimize
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
        /// Gets the Collection of all flow variables
        /// </summary>
        public VariableCollection<IEdge> x { get; }

        /// <summary>
        /// Gets the Collection of all design variables
        /// </summary>
        public VariableCollection<INode> y { get; }
    }
}
