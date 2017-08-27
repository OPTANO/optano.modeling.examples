using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRP
{
    using System.Configuration;

    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi75x;

    /// <summary>
    /// Demo program solving a vehicle routing problem
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">
        /// no arguments required
        /// </param>
        static void Main(string[] args)
        {

            INode pad = new Node("Paderborn", 0, true); // Starting node (depot) 
                                                       // of our vehicle routing problem
            INode nyc = new Node("New York", 1500, false);
            INode bjs = new Node("Beijing", 2000, false);
            INode sao = new Node("São Paulo", 2000, false);
            INode sfo = new Node("San Francisco", 2500, false);

            var nodes = new List<INode> { pad, nyc, bjs, sao, sfo };

            var edges = new List<IEdge>
                            {   
                                //Paderborn outgoing
                                new Edge(pad, nyc, 6130),
                                new Edge(pad, bjs, 7660),
                                new Edge(pad, sao, 9950),
                                new Edge(pad, sfo, 9000),

                                // from Beijing
                                new Edge(bjs, sfo, 9510),

                                // from New York
                                new Edge(nyc, bjs, 11000),
                                new Edge(nyc, sfo, 4140),

                                // from San Francisco
                                new Edge(sfo, sao, 10400),

                                // from Sao Paulo
                                new Edge(sao, nyc, 7680),

                                //Paderborn incoming
                                new Edge(nyc, pad, 6130),
                                new Edge(bjs, pad, 7660),
                                new Edge(sao, pad, 9950),
                                new Edge(sfo, pad, 9000),
                            };

            var vehicles = 3;   // initialize 3 vehicles

            var capacity = 4000;    // each vehicle has a capacity of 4000 units

            // use default settings
            var config = new Configuration();
            config.NameHandling = NameHandlingStyle.UniqueLongNames;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the modelscope
                var VRPModel = new VehicleRoutingModel(nodes, edges, vehicles, capacity);

                // Get a solver instance, change your solver
                using (var solver = new GurobiSolver())
                {
                    try
                    {
                        // troubleshooting

                        solver.Configuration.TimeLimit = 1;
                        (solver.Configuration as GurobiSolverConfiguration).ComputeIIS = true;

                        // troubleshooting

                        // solve the model
                        var solution = solver.Solve(VRPModel.Model);
                        if (solution.ConflictingSet != null)
                        {
                            var conflicts = solution.ConflictingSet.ToString();
                            Console.WriteLine(conflicts);
                        }

                        // import the results back into the model 
                        VRPModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                        // print objective and variable decisions
                        Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                        VRPModel.x.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));
                        VRPModel.y.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

                        VRPModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    catch (Exception) { }
                   
                }
            }

        }
    }
}
