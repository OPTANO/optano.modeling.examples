using OPTANO.Modeling.Optimization.Solver.MipCL213;
using System;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Enums;

namespace Demo_Mipcl
{
    class Program
    {


        /* TODO: Before you start
        
            - Add the mipcl.dll to the project ("Add Existing Item"). The DLL usually is in C:\Program Files\PNN\MIPCL\mip\bin\mipcl.dll
            - Set the mipcl.dll, "copy to output directory" to "copy always" in the project. 
            
            if you are create a netcore project: 
            - Add the MipCL172WrapperCpp.dll it usually is in %homepath%\.nuget\packages\optano.modeling\2.12.0.415\content\MipCL172WrapperCpp.dll. 
            - Set the MipCL172WrapperCpp.dll, "copy to output directory" to "copy always" in the project.
         
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

                using (var solver = new MipCLSolver())
                {
                    solver.Solve(model);
                }
            }
        }
    }
}
