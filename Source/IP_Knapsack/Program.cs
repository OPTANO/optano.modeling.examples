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
    using OPTANO.Modeling.Optimization.Solver.Gurobi810;

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

            // Use long names for easier debugging/model understanding.
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
                using (var solver = new GurobiSolver())
                {
                    // solve the model
                    var solution = solver.Solve(knapsackModel.Model);
                    ExportSolution(solution, knapsackModel);
                    // results from solution are already "synced" with variables in knapsack model.
                    Program.AllowActivationOfAtMost4ActiveVariables(knapsackModel);

                    // re-solve 1
                    var solutionB = solver.Solve(knapsackModel.Model);
                    ExportSolution(solutionB, knapsackModel);
                    Program.AllowActivationOfAtMost4ActiveVariables(knapsackModel);

                    // re-solve 2
                    var solutionC = solver.Solve(knapsackModel.Model);
                    ExportSolution(solutionC, knapsackModel);
                }
            }
        }

        /// <summary>
        /// Adds a constraint to the model that prevents the solver from simply re-using the old solution.
        /// At most 4 variables that were active in the current solution are allowed to be active in the next solution.
        /// Serves only to demonstrate the modification + resolve functionality of the framework.
        /// </summary>
        /// <param name="knapsackModel">The current (solved) model</param>
        private static void AllowActivationOfAtMost4ActiveVariables(KnapsackModel knapsackModel)
        {
            var activeVariables = knapsackModel.y.Variables.Where(v => Math.Round(v.Value) >= 1).ToList();
            // only add constraint if it will "do something"
            if (!activeVariables.Any() || activeVariables.Count <= 4)
            {
                return;
            }
            var info = $"Limiting the usage of the following variables to at most 4:{Environment.NewLine.PadRight(9)}{string.Join($",{Environment.NewLine}".PadRight(10), activeVariables.OrderBy(v => v.Name).Select(v => v.Name))}";
            Console.WriteLine(info);
            var limitUsageOfItems = Expression.Sum(activeVariables) <= 4;
            knapsackModel.Model.AddConstraint(limitUsageOfItems);
        }

        private static void ExportSolution(Solution solution, KnapsackModel knapsackModel)
        {
            // print objective and variable decisions
            Console.WriteLine($"{solution.ObjectiveValues.Single()}");
            knapsackModel.y.Variables.OrderBy(y => y.Name).ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

            knapsackModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
