using OPTANO.Modeling.Optimization.Solver.GLPK;
using System;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Enums;

namespace Demo_GLPK
{
    class Program
    {


        /* TODO: Before you start
        
            - Add the glpk_4_60.dll to the project ("Add Existing Item"). 
            - Set the glpk_4_60.dll, "copy to output directory" to "copy always" in the project. 
            
            - Add the libglpk-cli.dll as a reference (shall appear under Dependencies / Assemblies)
            - Set the libglpk-cli reference to "copy local" = yes
        */

        static void Main(string[] args)
        {

            // Use long names for easier debugging/model understanding.
            using (var scope = new ModelScope())
            {
                var model = new Model();

                var x = new Variable("x", 0, double.PositiveInfinity, VariableType.Integer);
                var y = new Variable("y", 0, double.PositiveInfinity, VariableType.Integer);

                model.AddObjective(new Objective(2 * x + y + 10, "goal", ObjectiveSense.Maximize), "goal");

                model.AddConstraint(x + y <= 100);

                using (var solver = new GLPKSolver())
                {
                    solver.Solve(model);
                }
            }
        }
    }
}
