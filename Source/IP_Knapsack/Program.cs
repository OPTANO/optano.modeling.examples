using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack
{
    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi75x;

    /// <summary>
    /// Demo program solving a Knapsack Problem
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

            // create example Items
            var items = new List<IKnapsackItem>
            {
                new KnapsackItem("Item A",1,8),
                new KnapsackItem("Item B",3,21),
                new KnapsackItem("Item C",1.5,17),
                new KnapsackItem("Item D",2.5,12),
                new KnapsackItem("Item E",4,13),
                new KnapsackItem("Item F",2,11),
                new KnapsackItem("Item G",2,17),
                new KnapsackItem("Item H",1,9)
            };

            // maximum weight of all the items
            double maxWeight = 10.8;

            // use default settings
            var config = new Configuration();
            config.NameHandling = NameHandlingStyle.UniqueLongNames;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var knapsackModel = new KnapsackModel(items, maxWeight);

                // Get a solver instance, change your solver
                var solver = new GurobiSolver();

                // solve the model
                var solution = solver.Solve(knapsackModel.Model);

                // import the results back into the model 
                knapsackModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                knapsackModel.y.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

                knapsackModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
            }


        }
    }
}
