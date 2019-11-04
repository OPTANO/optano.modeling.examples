using OPTANO.Modeling.Optimization.Solver.GLPK;
using System;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Configuration;
using OPTANO.Modeling.Optimization.Enums;

namespace Demo_GLPK_framework
{
    class Program
    {


        /* TODO: Before you run this demo
        
            - Add the glpk_4_60.dll to the project ("Add Existing Item"). 
            - Set the glpk_4_60.dll, "copy to output directory" to "copy always" in the project. 
            
            - Add the libglpk-cli.dll 
            - Set the libglpk-cli.dll, "copy to output directory" to "copy always" in the project.
         
            - Run "Restore nuget package" from the solution's context menu

        */

        /* TODO if you'd like to create your own project
            This project has been created by the following steps:

            - Create new project
            - Project settings, build: disable prefer 32bit
            - Add this program.cs
            - Add Nuget OPTANO Modeling 3.0 package
            - Add the required glpk dlls and set them to "copy always" (see todo above)
           
        */

        static void Main(string[] args)
        {
            // Use long names for easier debugging/model understanding.
            var scopeConfig = new Configuration() { NameHandling = NameHandlingStyle.UniqueLongNames };
            using (var scope = new ModelScope(scopeConfig))
            {
                var model = new Model();

                var x = new Variable("x", 0, double.PositiveInfinity, VariableType.Integer);
                var y = new Variable("y", 0, double.PositiveInfinity, VariableType.Integer);

                model.AddObjective(new Objective(2 * x + y + 10, "goal", ObjectiveSense.Maximize), "goal");

                model.AddConstraint(x + y <= 100);

                using (var solver = new GLPKSolver())
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
