using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CLSP
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Enums;
    using OPTANO.Modeling.Optimization.Operators;

    /// <summary>
    /// A Capacitated Lot-Sizing Model
    /// </summary>
    public class CapacitatedLotsizingModel
    {
       
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="CapacitatedLotsizingModel"/> class and initializes all fields
        /// </summary>
        /// <param name="timesteps">
        /// The time steps of the problem
        /// </param>
        public CapacitatedLotsizingModel(List<ITimestep> timesteps)
        {
            this.Timesteps = timesteps;
         
            this.Model = new Model();
               

            // Binary production variables
            this.y = new VariableCollection<ITimestep>(
                this.Model,
                this.Timesteps,
                "y",
                timestep => new StringBuilder($"Are we producing in time step {timestep.Name}?"),
                timestep => 0,
                timestep => 1,
                VariableType.Binary);

            // Continuous production variables
            this.x = new VariableCollection<ITimestep>(
                this.Model,
                this.Timesteps,
                "x",
                timestep => new StringBuilder($"Production amount in time step {timestep.Name}"),
                timestep => 0,
                timestep => timestep.Demand ?? double.PositiveInfinity,
                VariableType.Continuous);

            // the maximum production volume per time step is 
            // the largest demand of all time steps
            var bigM = this.Timesteps.Max(timestep => timestep.Demand);

            // Continuous inventory variables
            this.s = new VariableCollection<ITimestep>(
                this.Model,
                this.Timesteps,
                "s",
                timestep => new StringBuilder($"Inventory amount in time step {timestep.Name}"),
                timestep => 0,
                timestep => (double)bigM, // take bigM here as an upper bound
                VariableType.Continuous);

            // Create constraints

            // The sum of the production volume of period t 
            // + the inventory of the prior period need to add up to 
            // -> the demand and inventory in period t
            // start at period 1, since period 0 has no production / inventory

            for (int t = 0; t < this.Timesteps.Count-1; t++) // if you want to use a foreach
            {                                                // implement a successor() method
                if (t > 0)  // if you want to split up based on time steps you can simply
                            // ask for the different time steps and create ...
                {
                this.Model.AddConstraint(
                    (this.x[this.Timesteps[t]] + this.s[this.Timesteps[t]] - this.s[this.Timesteps[t - 1]] - (double)this.Timesteps[t].Demand) 
                    == 0,
                    $"Demand, production & inventory - balance for time step {Timesteps[t]}");
                }
                else       // ... separate constraints for them
                {
                this.Model.AddConstraint(
                    (this.x[Timesteps[t]] + this.s[Timesteps[t]] - (double)Timesteps[t].Demand) 
                    == 0,
                    $"Demand, production & inventory - balance for time step {Timesteps[t]}");
                }
            }

            // Starting inventory and end inventory need to be 0

            // Get the first time step:
            var first = Timesteps[0];
                // Force the inventory of the first time step to be 0
                this.Model.AddConstraint(
                    this.s[first] == 0,
                    $"The inventory is empty at the beginning of the first period.");

                // Get the last time step:
                var last = Timesteps[Timesteps.Count - 1];
                // Force the inventory of the last time step to be 0
                this.Model.AddConstraint(
                    this.s[last] == 0,
                    $"The inventory is empty at the end of the last period.");


            // Production volume 'x' of a time step can not be larger than the capacity

            for (int t = 0; t < this.Timesteps.Count - 1; t++)
            {
                this.Model.AddConstraint(
                    this.x[Timesteps[t]] <= Timesteps[t].Capacity * this.y[Timesteps[t]],
                    $"capacity in period {t} can not be exceeded");
            }


            // Add the objective:
            // Sum of the setup, production and inventory costs for all time steps
            // \sum_{time step in Time steps} \{ x_{time step} * SetupCost_{time step} + 
            //                                 y_{time step} * ProductionCost_{time step} +
            //                                 y_{time step} * InventoryCost_{time step} \}
            this.Model.AddObjective(
                new Objective(Expression.Sum(this.Timesteps.Select
                (timestep => (y[timestep] * timestep.SetupCost +
                              x[timestep] * timestep.ProductionCost +
                              x[timestep] * timestep.InventoryCost))),
                "sum of all cost",
                ObjectiveSense.Minimize)
            );
        }

        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the time steps of this network
        /// </summary>
        public List<ITimestep> Timesteps { get; }

        /// <summary>
        /// Gets the Collection of all binary production variables
        /// </summary>
        public VariableCollection<ITimestep> y { get; }

        /// <summary>
        /// Gets the Collection of all continuous production variables
        /// </summary>
        public VariableCollection<ITimestep> x { get; }

        /// <summary>
        /// Gets the Collection of all inventory variables
        /// </summary>
        public VariableCollection<ITimestep> s { get; }
    }
}
