using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLP
{
    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi752;

    /// <summary>
    /// Demo program solving a Warehouse Location Problem
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

            INode pb = new Node("Paderborn", -100, 1000000);
            INode ny = new Node("New York", -200, 1750000);
            INode b = new Node("Beijing", -100, 750000);
            INode sp = new Node("São Paulo", 100, 0);
            INode sf = new Node("San Francisco", 100, 0);
            INode mo = new Node("Moscow", 100, 0);
            var nodes = new List<INode> { pb, ny, b, sp, sf, mo };

            var edges = new List<IEdge>
                            {
                                new Edge(pb, sp, 100, 9900),
                                new Edge(pb, sf, 100, 9500),
                                new Edge(pb, mo, 100, 1900),
                                new Edge(ny, sp, 100, 7600),
                                new Edge(ny, sf, 100, 4100),
                                new Edge(ny, mo, 100, 7500),
                                new Edge(b, sp, 100, 17500),
                                new Edge(b, sf, 100, 9500),
                                new Edge(b, mo, 100, 5800)
                            };

            // Use long names for easier debugging/model understanding.
            var config = new Configuration();
            config.NameHandling = NameHandlingStyle.UniqueLongNames;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {
                // create a model, based on given data and the model scope
                var warehouseModel = new WarehouseLocationModel(nodes, edges);

                // Get a solver instance, change your solver
                using (var solver = new GurobiSolver())
                {
                    // solve the model
                    var solution = solver.Solve(warehouseModel.Model);

                    // import the results back into the model 
                    warehouseModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                    // print objective and variable decisions
                    Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                    warehouseModel.x.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));
                    warehouseModel.y.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

                    warehouseModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
                    Console.ReadLine();
                }
            }
        }
    }
}
