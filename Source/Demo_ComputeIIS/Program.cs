using System;
using System.Collections.Generic;
using System.Linq;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Configuration;

namespace Demo_ComputeIIS
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using OPTANO.Modeling.Optimization.Solver.Gurobi900;

    class Program
    {
        static void Main(string[] args)
        {
            /*
             Maximize
                 obj: x1 + 2 x2 + 3 x3
              Subject To
               c1:  x2 + x3 <= 20
               c2: x1 - 3 x2 + x3 <= 30
               c3: x1<= 20
               c4: x1>=40
              Bounds
               0 <= x1 <= 40
              Generals
               x1 x2 x3
              End

             */
            var scopeConfig = new Configuration() { NameHandling = NameHandlingStyle.Manual };
            using (var scope = new ModelScope(scopeConfig))
            {
                var model = new Model { Name = "model" };
                var x1 = new Variable("x1", 0, 40);
                var x2 = new Variable("x2");
                var x3 = new Variable("x3");

                model.AddConstraint(x2 + x3 <= 20, "c1");
                model.AddConstraint(x1 - 2 * x2 + x3 <= 30, "c2");

                var brokenConstraint2 = new Constraint(x1, "c3", upperBound: 20);
                model.AddConstraint(brokenConstraint2);

                var brokenConstraint1 = new Constraint(x1, "c4", 40);
                model.AddConstraint(brokenConstraint1);
                model.AddObjective(new Objective(x1 + 2 * x2 + 3 * x3, "obj"));

                var solverConfig = new GurobiSolverConfiguration() { ComputeIIS = true };
                using (var solver = new GurobiSolver(solverConfig))
                {
                    var solution = solver.Solve(model);
                    Assert.IsTrue(solution.ConflictingSet.ConstraintsLB.Contains(brokenConstraint1));
                    Assert.IsTrue(solution.ConflictingSet.ConstraintsUB.Contains(brokenConstraint2));
                }
            }
        }
    }
}
