using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    using System.Security.Cryptography.X509Certificates;

    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Enums;
    using OPTANO.Modeling.Optimization.Solver.Cplex127;

    /// <summary>
    /// program solving a Sudoku
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
            
            var size = Enumerable.Range(0, 9).ToList();
            var section = size.GroupBy(s => s / 3);

            // you may manipulate the following game, which is the initial state of the Sudoku to your liking
            // note: obviously not observing the rules of Sudoku here makes the problem infeasible
            var game = new int?[,] {
                    //  0    1      2     3     4    5     6     7     8 
                    { null,    3, null, null, null, null, null, null, null },
                    { null, null, null,    1,    9,    5, null, null, null },
                    { null, null,    8, null, null, null, null,    6, null },

                    {    8, null, null, null,    6, null, null, null, null },
                    {    4, null, null,    8, null, null, null, null,    1 },
                    { null, null, null, null,    2, null, null, null, null },

                    { null,    6, null, null, null, null,    2,    8, null },
                    { null, null, null,    4,    1,    9, null, null,    5 },
                    { null, null, null, null, null, null, null,    7, null },
                };

            // use default settings
            var config = new Configuration();
            config.EnableFullNames = true;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {
                // create a model, based on given data and the model scope
                var sudokuModel = new SudokuModel(size, section, game);
                
                // get a solver instance, change your solver
                var solver = new CplexSolver();

                // solve the model
                var solution = solver.Solve(sudokuModel.Model);

                // print objective and variable decisions
                Console.WriteLine("Result: ");
                foreach (var row in size)
                {
                    foreach (var col in size)
                    {
                        foreach (var value in size)
                        {
                            if (sudokuModel.field[col, row, value].Value > 0)
                            {
                                Console.Write(string.Format("   {0}", value + 1));
                            }
                        }
                        if ((col + 1) % 3 == 0) Console.Write("  ");
                    }
                    if ((row + 1 ) % 3 == 0) Console.WriteLine();
                    Console.WriteLine();
                }


            }
        }
    }
}
