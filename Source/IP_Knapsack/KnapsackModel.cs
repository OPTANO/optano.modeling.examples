using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Knapsack
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Operators;
    using OPTANO.Modeling.Optimization.Enums;

    /// <summary>
    /// A Knapsack Model
    /// </summary>
    public class KnapsackModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="KnapsackModel"/> class and initializes all fields. 
        /// </summary>
        /// <param name="items">
        /// The items of the model
        /// </param>
        /// <param name="maxWeight">
        /// The maximum weight of all items taken
        /// </param>
        public KnapsackModel(IEnumerable<KnapsackItem> items, double maxWeight)
        {
            this.Items = items;
            this.MaxWeight = maxWeight;
            this.Model = new Model();

            // Choose Item Variable
            this.y = new VariableCollection<KnapsackItem>(
               this.Model,
               this.Items,
               "y", 
               item => $"Status of item {item.Name}",
               item => 0,
               item => 1,
               item => VariableType.Binary); // it is a binary! only bounds of {0;1} are valid.

            // Create Constraints

            // the sum of the weight of the items selected cannot exceed the maximum weight
            this.Model.AddConstraint(
                Expression.Sum(this.Items.Select(item => this.y[item] * item.Weight))
                <= MaxWeight,
                $"Sum of all weights"
                );


            // Add the objective
            // Sum of all item values.
            // \sum_{item \in Items} \{ y_{item} * value_{item}\}
            this.Model.AddObjective(
            new Objective(Expression.Sum(this.Items.Select(item => (y[item] * item.Value))),
            "Sum of all item-values",
            ObjectiveSense.Maximize)
            );
        }

        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the edges of this network
        /// </summary>
        public IEnumerable<KnapsackItem> Items { get; }

        /// <summary>
        /// Maximum weight the knapsack can take
        /// </summary>
        public double MaxWeight { get; }

        /// <summary>
        /// Gets the Collection of all design variables
        /// </summary>
        public VariableCollection<KnapsackItem> y { get; }
    }
}
