using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetProblems
{
    using System.Configuration;

    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi752;

    /// <summary>
    /// Demo program solving Set-problem(s)
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

            // create example elements
            IElement one = new Element("one", 1);
            IElement two = new Element("two", 2);
            IElement three = new Element("three", 3);
            IElement four = new Element("four", 4);
            IElement five = new Element("five", 5);

            // create full set
            ISet_E fullSet = new Set_E("full_set", 
                             new List<IElement> { one, two, three, four, five} , 0, true);

            // create subsets
            ISet_E subset_1 = new Set_E("subset_1", 
                              new List<IElement> { one, three }, 8, false);
            ISet_E subset_2 = new Set_E("subset_2", 
                              new List<IElement> { three, five }, 16, false);
            ISet_E subset_3 = new Set_E("subset_3", 
                              new List<IElement> { three }, 6, false);
            ISet_E subset_4 = new Set_E("subset_4", 
                              new List<IElement> { one, two, four }, 15, false);
            ISet_E subset_5 = new Set_E("subset_5",
                              new List<IElement> { one, five }, 7, false);

            var sets = new List<ISet_E>{subset_1, subset_2, subset_3, subset_4, subset_5};

            // use default settings
            var config = new Configuration();
            config.NameHandling = NameHandlingStyle.UniqueLongNames;
            config.ComputeRemovedVariables = true;
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var setProblemModel = new SetProblemModel(sets, fullSet);

                // Get a solver instance, change your solver
                var solver = new GurobiSolver();

                // solve the model
                var solution = solver.Solve(setProblemModel.Model);

                // import the results back into the model 
                setProblemModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));

                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                setProblemModel.y.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));

                setProblemModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);
            }

            Console.ReadLine();
        }
    }
}
