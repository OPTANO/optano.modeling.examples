using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Knapsack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OPTANO.Modeling.Common;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Configuration;
using OPTANO.Modeling.Optimization.Solver.Gurobi900;

namespace IP_Knapsack_Tests
{
    [TestClass]
    public class KnapsackModelTests
    {
        private List<KnapsackItem> _items;
        private double _maxWeight;

        [TestInitialize]
        public void Initialize()
        {
            // create example Items
            var csv = new CsvReader(File.OpenText("knapsackItems.csv"));
            csv.Configuration.Delimiter = ";";
            csv.Configuration.CultureInfo = new CultureInfo("en-US");
            csv.Configuration.RegisterClassMap<KnapsackItemMap>();
            _items = csv.GetRecords<KnapsackItem>().ToList();

            // maximum weight of all the items
            _maxWeight = 10.8;

            // Use long names for easier debugging/model understanding.
            var config = new Configuration
            {
                NameHandling = NameHandlingStyle.UniqueLongNames,
                ComputeRemovedVariables = true
            };
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var knapsackModel = new KnapsackModel(_items, _maxWeight);

                // Get a solver instance, change your solver
                using (var solver = new GurobiSolver())
                {
                    // solve the model
                    var solution = solver.Solve(knapsackModel.Model);

                    // import the results back into the model
                    knapsackModel.Model.VariableCollections.ForEach(vc => vc.SetVariableValues(solution.VariableValues));
                    foreach (var knapsackItem in _items)
                    {
                        knapsackItem.IsPacked = Math.Abs(knapsackModel.y[knapsackItem].Value - 1) < scope.EPSILON;
                    }
                }
            }
        }

        [TestMethod]
        public void WeightConstraintTest()
        {
            Assert.IsTrue(_items.Where(item => item.IsPacked).Sum(item => item.Weight) <= _maxWeight);
        }
    }
}
