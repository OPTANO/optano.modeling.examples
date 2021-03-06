﻿namespace NDP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi900;

    /// <summary>
    /// Demo program solving a network design problem
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

            // create example Nodes
            INode pb = new Node("Paderborn", 50);
            INode ny = new Node("New York", -100);
            INode b = new Node("Beijing", 50);
            INode sp = new Node("São Paulo", 50);
            INode sf = new Node("San Francisco", -50);
            // assign these nodes to a list of INodes
            var nodes = new List<INode> { pb, ny, b, sp, sf };

            // create example Edges
            IEdge one = new Edge(ny, pb, null, 3, 6100);
            IEdge two = new Edge(b, ny, null, 2, 11000);
            IEdge three = new Edge(sp, b, 75, 1, 17600);
            IEdge four = new Edge(sf, sp, null, 4, 10500);
            IEdge five = new Edge(sp, pb, null, 5, 9900);
            IEdge six = new Edge(pb, b, 50, 1, 7600);

            // assign these edges to a list of IEdges
            var edges = new List<IEdge> { one, two, three, four, five, six };

            // Use long names for easier debugging/model understanding.
            var config = new Configuration();
            config.NameHandling = NameHandlingStyle.UniqueLongNames;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {
                // create a model, based on given data and the model scope
                var designModel = new NetworkDesignModel(nodes, edges);

                // Get a solver instance, change your solver
                using (var solver = new GurobiSolver())
                {
                    // solve the model
                    var solution = solver.Solve(designModel.Model);

                    // import the results back into the model
                    designModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                    // print objective and variable decisions
                    Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                    designModel.x.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));
                    designModel.y.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

                    designModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
                }
            }
        }
    }
}
