using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VRP
{

    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Enums;
    using OPTANO.Modeling.Optimization.Operators;

    /// <summary>
    /// A Multi Vehicle Routing Problem /w Capacity Constraints
    /// </summary>
    public class VehicleRoutingModel
    {

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="VehicleRoutingModel"/> class and initializes all fields
        /// </summary>
        /// <param name="nodes">
        /// The network nodes of the model
        /// </param>
        /// <param name="edges">
        /// The edges of the model
        /// </param>
        public VehicleRoutingModel(List<INode> nodes, List<IEdge> edges,
                                   int vehicles, int capacity)
        {
            this.Nodes = nodes;
            this.Edges = edges;
            // create an Enumerable based on the given Integer for the VariableCollections
            this.Vehicles = Enumerable.Range(1,vehicles).ToList();                                                      
            this.capacity = capacity;

            this.Model = new Model();

            // Indicates whether Vehicle k travels over this edge
            this.x = new VariableCollection<IEdge, int>( // This VariableCollection is based
                this.Model,                              // on two index Sets forming a
                this.Edges,                              // variable: x_(edge,time)
                this.Vehicles,
                "x",
                (edge, k) => $"Edge from {edge.FromNode} to {edge.ToNode} with vehicle {k}",
                (edge, k) => 0, // edge is not used by vehicle k
                (edge, k) => 1, // edge is used in the route by vehicle k
                (edge, k) => VariableType.Binary); // indicates whether the edge is used "1" or not "0"

            // Indicates whether Vehicle k is assigned to this customer
            this.y = new VariableCollection<INode, int>( // same as above but this time
                this.Model,                              // the variable is: y_(node,time)
                this.Nodes,     
                this.Vehicles,
                "y",
                (node, k) => $"Customer {node.Name} is served by vehicle {k}",
                (node, k) => 0, // vehicle k does not serve customer k
                (node, k) => 1, // vehicle k does serve customer 
                (node, k) => VariableType.Binary); // indicates if the customer is served by vehicle k.

            // we create a tuple (via cross join) based of edges
            // and vehicles to work on our x-variable
            var tupleEdgeVehicle = (from edge in this.Edges 
                          from vehicle in this.Vehicles //.AsParallel() invocation possible
                          select new { edge, vehicle }).ToList();

            var tupleNodeVehicle = (from node in this.Nodes
                                    from vehicle in this.Vehicles
                                    select new { node, vehicle }).ToList();


            // each customer is assigned to one vehicle - select only non-depots
            foreach (var node in this.Nodes.Where(node => node.IsDepot == false))
            {
                    this.Model.AddConstraint(
                    Expression.Sum(tupleNodeVehicle.Where(tuple => tuple.node == node).Select(tuple => y[node, tuple.vehicle]))
                    == 1,
                    $"Incoming {node}");
            }


            // all vehicles are assigned to the depot
            this.Model.AddConstraint(
            Expression.Sum(tupleNodeVehicle.Where(tuple => tuple.node.IsDepot == true).Select(tuple => y[tuple.node, tuple.vehicle]))
            == vehicles,
            $"Outgoing");


            //A vehicle that enters a node must leave the same node afterwards
            foreach (var vehicle in this.Vehicles)
            {
                // you can add one or more constraints in the same loop
                foreach (var node in this.Nodes)
                {
                    this.Model.AddConstraint(
                        // vehicle arriving at a node ...
                        Expression.Sum(this.Edges.Where(edge => edge.FromNode == node).Select(edge => this.x[edge, vehicle]))
                        // ... must leave the same node
                        == Expression.Sum(this.Edges.Where(edge => edge.ToNode == node).Select(edge => this.x[edge, vehicle])),
                        $"Flow Balance1 for {node} with vehicle {vehicle}");

                    this.Model.AddConstraint(
                        Expression.Sum(this.Edges.Where(edge => edge.FromNode == node).Select(edge => this.x[edge, vehicle])) // number of incoming edges
                        == y[node, vehicle], // is equal to the number of assigned vehicles
                        $"Flow Balance2 for {node} with vehicle {vehicle}");
                }
            }


            // Vehicle capacity can not be exceeded
            foreach (var vehicle in this.Vehicles)
            {
                this.Model.AddConstraint(
                    // Sum of the demands for vehicle k
                    Expression.Sum(this.Edges.Where(edge => edge.FromNode.Demand > 0).Select(edge => this.x[edge, vehicle]))
                    // has to be less, than the vehicle capacity
                    <= this.capacity,
                    $"Capacity of vehicle {vehicle}");
            }

            // Add the objective:
            // Sum of the distances between all used edges results in the cost
            // \sum_{edge \in Edges} \{ x_{edge} * transportationCost_{edge} \}

            this.Model.AddObjective(
                new Objective(
                // You have the choice of two different implementations:

                // 1: Concatenation of Sums
                //Expression.Sum(Enumerable.Range(1,this.vehicles)
                // .Select(vehicle => Expression.Sum(this.Edges
                // .Select(edge => (x[edge,vehicle] * edge.transportationCost))))),

                // 2: Using the tuple
                Expression.Sum(tupleEdgeVehicle.Select(tuple => x[tuple.edge, tuple.vehicle] * tuple.edge.transportationCost)),
                "sum of all distances", 
                ObjectiveSense.Minimize) // minimize the sum of all distances
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
        /// Gets the number of available vehicles
        /// </summary>
        public List<int> Vehicles { get; }

        /// <summary>
        /// Gets the capacity of each vehicle
        /// </summary>
        public int capacity { get; }

        /// <summary>
        /// Gets the Collection of all edge, vehicle assignments
        /// </summary>
        public VariableCollection<IEdge,int> x { get; }

        /// <summary>
        /// Gets the Collection of all node, vehicle assignments
        /// </summary>
        public VariableCollection<INode,int> y { get; }
    }
}
