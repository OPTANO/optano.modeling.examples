using System;
using System.Collections.Generic;
using System.Linq;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Enums;

namespace MIP_CLSP
{
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
        public CapacitatedLotsizingModel(IEnumerable<PeriodInformation> timesteps)
        {
            this.PeriodInformation = timesteps.ToList();
            this.Model = new Model();

            // Binary production variables
            this.y = new VariableCollection<PeriodInformation>(
                this.Model,
                this.PeriodInformation,
                "y",
                timestep => $"machines_active_in_period_{timestep.Period}",
                timestep => 0,
                timestep => 1,
                VariableType.Binary);

            // Continuous production variables
            this.x = new VariableCollection<PeriodInformation>(
                this.Model,
                this.PeriodInformation,
                "x",
                timestep => $"produced_amount_period_{timestep.Period}",
                timestep => 0,
                timestep => timestep.Capacity,
                VariableType.Continuous);
            
            // Continuous inventory variables
            this.s = new VariableCollection<PeriodInformation>(
                this.Model,
                this.PeriodInformation,
                "s",
                timestep => $"storage_end_of_period_{timestep.Period}",
                timestep => 0,
                // The maximum storage content is limited by the cumulative production. 
                // Since we need to fulfill demands from 'some' previous production, 
                // the maximum available stock amount can be reduced by the occuring demands.
                timestep => this.ComputeStorageVariableUpperBound(timestep),
                VariableType.Continuous);

            // Create constraints

            // flow balance: 'input == output'
            // input: previous storage + production
            // output: outgoing storage + demand
            foreach(var periodInformation in this.PeriodInformation)
            {
                var flowBalanceExpression = this.x[periodInformation] - this.s[periodInformation] - periodInformation.Demand;

                // 'previous storage' only exists after the initial period
                if (periodInformation.Period > 1)
                {
                    var previousPeriodInformation = this.GetPredecessor(periodInformation);
                    flowBalanceExpression += this.s[previousPeriodInformation];
                }
                this.Model.AddConstraint(
                    flowBalanceExpression == 0,
                    $"flow_balance_{periodInformation.Period}");
                
            }

            // Final inventory needs to be 0
            var last = PeriodInformation.Last();
            this.Model.AddConstraint(
                this.s[last] == 0,
                $"final_storage");

            // Production volume 'x' of a time step can not be larger than the capacity
            foreach(var periodInfo in this.PeriodInformation)
            {
                this.Model.AddConstraint(
                    this.x[periodInfo] <= periodInfo.Capacity * this.y[periodInfo],
                    $"production_capacity_{periodInfo}");
            }


            // Add the objective:
            // Sum of the setup, production and inventory costs for all time steps
            // \sum_{time step in Time steps} \{ x_{time step} * SetupCost_{time step} + 
            //                                 y_{time step} * ProductionCost_{time step} +
            //                                 s_{time step} * InventoryCost_{time step} \}
            this.Model.AddObjective(
                new Objective(Expression.Sum(this.PeriodInformation.Select
                (timestep => (y[timestep] * timestep.SetupCost +
                              x[timestep] * timestep.ProductionCost +
                              s[timestep] * timestep.InventoryCost))),
                "totalCost",
                ObjectiveSense.Minimize)
            );
        }

        private PeriodInformation GetPredecessor(PeriodInformation currentPeriod)
        {
            if (currentPeriod.Period <= 1)
            {
                throw new InvalidOperationException($"There is no period before T={currentPeriod.Period}.");
            }

            // make query fail/throw if data is not correct.
            return this.PeriodInformation.Single(p => p.Period == currentPeriod.Period - 1);
        }

        private double ComputeStorageVariableUpperBound(PeriodInformation currentPeriod)
        {
            // we cannot store more than 'theoretical production limit' - 'required amount' (=demand)
            return this.PeriodInformation.Where(pi => pi.Period <= currentPeriod.Period)
                        .Sum(pi => pi.Capacity - pi.Demand);
        }

        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the time steps of this network
        /// </summary>
        public List<PeriodInformation> PeriodInformation { get; }

        /// <summary>
        /// Gets the Collection of all binary production variables
        /// </summary>
        public VariableCollection<PeriodInformation> y { get; }

        /// <summary>
        /// Gets the Collection of all continuous production variables
        /// </summary>
        public VariableCollection<PeriodInformation> x { get; }

        /// <summary>
        /// Gets the Collection of all inventory variables
        /// </summary>
        public VariableCollection<PeriodInformation> s { get; }
    }
}
