using System;
using System.IO;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Configuration;
using OPTANO.Modeling.Optimization.Enums;

namespace Demo_GLPK
{
    using OPTANO.Modeling.GLPK;

    class Program
    {


        /* TODO: Before you start
            - install the Optano.Modeling.GLPK NuGet package
            - Add the glpk_4_65.dll to the project ("Add Existing Item").
            - Set the glpk_4_65.dll, "copy to output directory" to "copy always" in the project.

            - Add the libglpk-cli.dll as a reference (shall appear under Dependencies / Assemblies)
            - Set the libglpk-cli reference to "copy local" = yes
        */

        static void Main(string[] args)
        {
            var glpkConfig = new GLPKSolverConfiguration()
            {
                LibraryPaths = { new DirectoryInfo(@"..\..\..\..\..\Tools\GLPK\glpk-4.65\w64\") }
            };
            // Use long names for easier debugging/model understanding.
            var scopeConfig = new Configuration() { NameHandling = NameHandlingStyle.UniqueLongNames };
            using (var scope = new ModelScope(scopeConfig))
            {
                var model = new Model();

                var x = new Variable("x", 0, double.PositiveInfinity, VariableType.Integer);
                var y = new Variable("y", 0, double.PositiveInfinity, VariableType.Integer);

                model.AddObjective(new Objective(2 * x + y + 10, "goal", ObjectiveSense.Maximize), "goal");
                model.AddConstraint(x + y <= 100);

                using (var solver = new GLPKSolver(glpkConfig))
                {
                    var solution = solver.Solve(model);

                    Console.WriteLine("Solution:");
                    Console.WriteLine($"x = {x.Value}");
                    Console.WriteLine($"y = {y.Value}");

                    var objVal = solution.GetObjectiveValue("goal") ?? double.NaN;
                    Console.WriteLine($"goal = {objVal:N2}");
                }
            }
        }
    }
}
