using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace Knapsack
{
    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi752;

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
            var csv = new CsvReader(File.OpenText("knapsackItems.csv"));
            csv.Configuration.Delimiter = ";";
            csv.Configuration.CultureInfo = new CultureInfo("en-US");
            csv.Configuration.RegisterClassMap<KnapsackItemMap>();
            var items = csv.GetRecords<KnapsackItem>().ToList();

            // maximum weight of all the items
            var maxWeight = 10.8;

            // use default settings
            var config = new Configuration
            {
                NameHandling = NameHandlingStyle.UniqueLongNames,
                ComputeRemovedVariables = true
            };
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

            Console.ReadLine();
        }
    }
}
