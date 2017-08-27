using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ITimestep one =   new Timestep(1, 0, 90, 10, 10, 20);
            ITimestep two =   new Timestep(2, 10, 20, 10, 20, 20);
            ITimestep three = new Timestep(3, 25, 20, 30, 30, 30);
            ITimestep four =  new Timestep(4, 15, 20, 20, 40, 20);
            ITimestep five =  new Timestep(5, 10, 20, 30, 30, 25);
            ITimestep six =   new Timestep(6, 5, 20, 40, 20, 20);
            ITimestep seven = new Timestep(7, 50, 5, 20, 30, 15);
            ITimestep eight = new Timestep(8, 10, 50, 20, 30, 15);

            var timesteps = new List<ITimestep> { one, two, three, four, five,
                                                  six, seven, eight };



            // use default settings
            var config = new Configuration();
            config.NameHandling = NameHandlingStyle.UniqueLongNames;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var clspModel = new CapacitatedLotsizingModel(timesteps);
                
                // Get a solver instance, change your solver
                var solver = new GurobiSolver();

                // solve the model
                var solution = solver.Solve(clspModel.Model);
                
                // import the results back into the model 
                clspModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                clspModel.y.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));
                clspModel.x.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));
                clspModel.s.Variables.ForEach(s => Console.WriteLine($"{s.ToString().PadRight(36)}: {s.Value}"));

                clspModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
            }


        }
    }
}
