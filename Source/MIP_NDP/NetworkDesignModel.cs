using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NDP
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Operators;
    using OPTANO.Modeling.Optimization.Enums;

   /// <summary>
    /// A network design model
    /// </summary>
    public class NetworkDesignModel
    {
       
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkDesignModel"/> class and initializes all fields. 
        /// </summary>
        /// <param name="nodes">
        /// The network nodes of the model
        /// </param>
        /// <param name="edges">
        /// The edges of the model
        /// </param>
        public NetworkDesignModel(List<INode> nodes, List<IEdge> edges)
        {
            this.Nodes = nodes;
            this.Edges = edges;
            this.Model = new Model();
               
            // Flow-Variables
            this.x = new VariableCollection<IEdge>(
                // register with this.Model
                this.Model,
                // Index is just the edge (might have time period in a more advanced problem)
                this.Edges,
                // the name of the variable collection
                "x",
                // Label-Generator
                edge => $"Flow {edge.FromNode} to {edge.ToNode}",
                // Lower Bound, 0 is default 
                edge => 0,
                // if Capacity is set, use as upper bound (otherwise double.PositiveInfinity for unbounded)
                edge => edge.Capacity ?? double.PositiveInfinity,
                // Continuous Variable (is default)
                VariableType.Continuous); 

            // Design-Variables
            this.y = new VariableCollection<IEdge>(
               this.Model, // register this.Model
               this.Edges, // Index is just the edge (might have time period in a more advanced problem)
               "y", // the name of the variable collection
               edge => $"Design {edge.FromNode} to {edge.ToNode}", // Label-Generator
               edge => 0, // Lower Bound, 0 is default 
               edge => (edge.Capacity ?? double.PositiveInfinity) > 0 ? 1d : 0d, // if capacity is set and greater than 0, set 1 as bound for binary, otherwise 0. e
               VariableType.Binary); // it is a binary! only bounds of {0;1} are valid.

            // Create Constraints

            // Add flow-balance for every node
            foreach (var node in this.Nodes)
            {
                // Add Constraint to model
                this.Model.AddConstraint(
                    // equal flow of every edge arriving at the node
                    Expression.Sum(this.Edges.Where(e => e.ToNode == node).Select(edge => this.x[edge]))

                    // flows of every edge departing from the node
                    == Expression.Sum(this.Edges.Where(e => e.FromNode == node).Select(edge => this.x[edge]))
                    
                    // plus the demand to fulfill
                    + node.Demand, 
                    $"FlowBalance, {node}"); // name of the constraint for debug use.
            }

            // if any edge is unbounded, take the sum of all demands as Big M
            var bigM = this.Nodes.Where(node => node.Demand > 0).Sum(node => node.Demand);

            foreach (var edge in this.Edges)
            {
                // Add Constraint to model
                this.Model.AddConstraint(

                    // Set y greater or equal to edge's usage ratio, use Big M is capacity is unbounded
                    this.y[edge] >= this.x[edge] / (edge.Capacity ?? bigM),
                    // name of the constraint for debug use.
                    $"Desing_LowerBound {edge}"); 

                this.Model.AddConstraint(
                    // Bound y to zero, if edge is not used at all (only required if design cost may be negative).
                    this.y[edge] <= this.x[edge],
                    // name of the constraint for debug use.
                    $"Desing_UpperBound {edge}"); 
            }

            // Add the objective
            // Sum of all flows times the flow-unit-cost plus all design decisions and their respective costs.
            // \sum_{edge \in Edges} \{ x_{edge} * costPerFlowUnit_{edge} + y_{edge} * designCost_{edge} \}
            this.Model.AddObjective(
                new Objective(Expression.Sum(this.Edges.Select(edge => (x[edge] * edge.CostPerFlowUnit) + (y[edge] * edge.DesignCost))),
                "sum of all cost", // name
                ObjectiveSense.Minimize)
            ); // minimize

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
        public VariableCollection<IEdge> y { get; }
    }
}
