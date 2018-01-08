using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SetProblems
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Operators;
    using OPTANO.Modeling.Optimization.Enums;

    /// <summary>
    /// A Set Problem Model
    /// </summary>
    public class SetProblemModel
    {

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SetProblemModel"/> class and initializes all fields. 
        /// </summary>
        /// <param name="nodes">
        /// The network nodes of the model
        /// </param>
        /// <param name="edges">
        /// The edges of the model
        /// </param>
        public SetProblemModel(List<ISet_E> sets, ISet_E fullSet)
        {
            this.Sets = sets;
            this.Model = new Model();
            this.FullSet = fullSet;

            // Set Variables
            this.y = new VariableCollection<ISet_E>(
               this.Model, 
               this.Sets, 
               "y", 
               item => $"Status of set {item.Name}",
               item => 0, 
               item => 1,
               item => VariableType.Binary); // it is a binary! only bounds of {0;1} are valid.

            // Create Constraints

            // Set Covering constraint (1) - constraint (2) and (3) need to be disabled
            // for each element of the original set at least one new set 
            // which contains the element has to be chosen

            foreach (IElement element in this.FullSet.Elements)
            {
                this.Model.AddConstraint(
                    Expression.Sum(this.Sets.Where(set => set.ContainsElement(element)).Select(set => y[set]))
                    >= 1,
                    $"Element {element} is contained");
            }

            // Set Packing constraint (2) - constraint (1) and (3) need to be disabled
            // for each element of the original set at most one new set
            // which contains the element has to be chosen

            //foreach (IElement element in this.FullSet.Elements)
            //{
            //    this.Model.AddConstraint(
            //        Expression.Sum(this.Sets.Where(set => set.ContainsElement(element)).Select(set => y[set])) 
            //        <= 1,
            //        $"Element {element} is contained");
            //}

            // Set Partitioning constraint (3) - constraint (1) and (2) need to be disabled
            // for each element of the original set exactly one new set
            // which contains the element has to be chosen

            //foreach (IElement element in this.FullSet.Elements)
            //{
            //    this.Model.AddConstraint(
            //        Expression.Sum(this.Sets.Where(set => set.ContainsElement(element)).Select(set => y[set])) 
            //        == 1,
            //        $"Element {element} is contained");
            //}

            // Add the objective
            // Sum of the lowest cost while set- covering(1), packing(2), partitioning(3)
            // \sum_{set \in sets} \{ y_{set} * cost_{set}\}
            this.Model.AddObjective(
                new Objective(Expression.Sum(this.Sets.Select(set => y[set] * set.Cost)),
                "Sum of all set costs",
                ObjectiveSense.Minimize)
                );
        }

        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the List of all Sets with Elements
        /// </summary>
        public List<ISet_E> Sets { get; }

        /// <summary>
        /// Gets the original full Set
        /// </summary>
        public ISet_E FullSet { get; }

        /// <summary>
        /// Gets the Collection of all design variables
        /// </summary>
        public VariableCollection<ISet_E> y { get; }
    }
}
