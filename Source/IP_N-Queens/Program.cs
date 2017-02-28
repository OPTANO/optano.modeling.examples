using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueensProblem
{
    using System.Configuration;

    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi652;
    using System.Collections;

    /// <summary>
    /// Demo program solving the n-Queens Problem
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
            // set size of the board - board is always a square
            int dimension = 8;

            // use default settings
            var scopeSettings = new OptimizationConfigSection();
            scopeSettings.ModelElement.EnableFullNames = true;
            scopeSettings.ModelElement.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(scopeSettings))
            {

                // create a model, based on given data and the model scope
                var queensModel = new QueensModel(dimension);

                // Get a solver instance, change your solver
                var solver = new GurobiSolver();

                // solve the model
                var solution = solver.Solve(queensModel.Model);

                // import the results back into the model 
                queensModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                //queensModel.y.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

                Console.WriteLine("Result: ");

                foreach (var row in Enumerable.Range(0, dimension))
                {
                    foreach (var col in Enumerable.Range(0, dimension))
                    {

                        Console.Write(string.Format(queensModel.y[row, col].Value + "  "));
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
