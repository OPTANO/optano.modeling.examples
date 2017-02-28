using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLSP
{
    /// <summary>
    /// A time step of the Capacitated Lot-Sizing Model
    /// </summary>
    public class Timestep : ITimestep
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="CLSP.Timestep"/> class, representing a single time step
        /// </summary>
        /// <param name="name">
        /// Name of the time step
        /// </param>
        /// <param name="demand">
        /// The demand in this time step
        /// </param>
        /// <param name="setupCost">
        /// The setup cost in this time step
        /// </param>
        /// <param name="productionCost">
        /// The production cost in this time step
        /// </param>
        /// <param name="inventoryCost">
        /// The inventory cost in this time step
        /// </param>
        /// <param name="capacity">
        /// The production capacity
        /// </param>

        public Timestep(int name, double demand, double setupCost, 
                        double productionCost, double inventoryCost, double capacity)
        {
            this.Name = name;
            this.Demand = demand;
            this.SetupCost = setupCost;
            this.ProductionCost = productionCost;
            this.InventoryCost = inventoryCost;
            this.Capacity = capacity;
        }

        /// <summary>
        /// Gets the name of this time step
        /// </summary>
        public int Name { get; }

        /// <summary>
        /// Gets the demand in this time step
        /// </summary>
        public double? Demand { get; }

        /// <summary>
        /// Gets the cost to setup the machine in this time step
        /// </summary>
        public double SetupCost { get; }

        /// <summary>
        /// Gets the cost to produce a single good in this time step
        /// </summary>
        public double ProductionCost { get; }

        /// <summary>
        /// Gets the inventory cost in this time step
        /// </summary>
        public double InventoryCost { get; }

        /// <summary>
        /// Production capacity in the time step
        /// </summary>
        public double Capacity { get; }

        /// <summary>
        /// Name of the time step
        /// </summary>
        /// <returns>
        /// The name of this time step (<see cref="string"/>).
        /// </returns>
        public override string ToString() => Convert.ToString(this.Name);

    }
}
