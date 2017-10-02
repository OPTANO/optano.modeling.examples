using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MIP_CLSP;
using OPTANO.Modeling.Optimization.Solver;

namespace CLSP
{
    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi75x;

    /// <summary>
    /// Demo program solving a Capacitated Lot-Sizing Problem
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
            // create time steps with their "name", demand, setup cost,
            // production cost per unit, inventory cost per unit
            var csv = new CsvReader(File.OpenText("timesteps.csv"));
            csv.Configuration.Delimiter = ";";
            csv.Configuration.CultureInfo = new CultureInfo("en-US");
            var periodInformation = csv.GetRecords<PeriodInformation>();

            // use default settings
            var config = new Configuration
            {
                NameHandling = NameHandlingStyle.Manual,
                ComputeRemovedVariables = true
            };
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var clspModel = new CapacitatedLotsizingModel(periodInformation);

                var solverCfg = new GurobiSolverConfiguration()
                {
                    ModelOutputFile = new FileInfo("clsp.lp"),
                };

                // Get a solver instance, change your solver
                var solver = new GurobiSolver(solverCfg);

                // solve the model
                var solution = solver.Solve(clspModel.Model);
                
                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                clspModel.y.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));
                clspModel.x.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));
                clspModel.s.Variables.ForEach(s => Console.WriteLine($"{s.ToString().PadRight(36)}: {s.Value}"));

                clspModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
            }

            Console.ReadLine();
        }
    }
}
