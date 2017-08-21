using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    using System.Configuration;

    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi75x;
    using System.IO;

    /// <summary>
    /// Demo program solving a traveling salesman problem
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

            INode pb = new Node("Paderborn",true);
            INode ny = new Node("New York",false);
            INode b = new Node("Beijing",false);
            INode sp = new Node("São Paulo",false);
            INode sf = new Node("San Francisco",false);
            var nodes = new List<INode> { pb, ny, b, sp, sf };

            var edges = new List<IEdge>
                            {
                                new Edge(pb, ny, 6100),
                                new Edge(pb, b, 7600),
                                new Edge(pb, sp, 9900),
                                new Edge(ny, sp, 7600),
                                new Edge(ny, b, 11000),
                                new Edge(b, sf, 9500),
                                new Edge(b, ny, 11000),
                                new Edge(sp, pb, 9900),
                                new Edge(sf, sp, 10500),
                                new Edge(sf, ny, 4100)
                            };

            // use default settings
            var config = new Configuration();
            config.EnableFullNames = true;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var travelingSalesmanModel = new TravelingSalesmanModel(nodes, edges);
                
                // Get a solver instance, change your solver
                var solver = new GurobiSolver();

                // solve the model
                var solution = solver.Solve(travelingSalesmanModel.Model);

                // import the results back into the model 
                travelingSalesmanModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                travelingSalesmanModel.y.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));

                travelingSalesmanModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
            }


        }
    }
}
