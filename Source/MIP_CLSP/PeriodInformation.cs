using System;

namespace MIP_CLSP
{
    /// <summary>
    /// A time step of the Capacitated Lot-Sizing Model
    /// </summary>
    public class PeriodInformation
    {
        /// <summary>
        /// Gets the name of this time step
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Gets the demand in this time step
        /// </summary>
        public double Demand { get; set; }

        /// <summary>
        /// Gets the cost to setup the machine in this time step
        /// </summary>
        public double SetupCost { get; set; }

        /// <summary>
        /// Gets the cost to produce a single good in this time step
        /// </summary>
        public double ProductionCost { get; set; }

        /// <summary>
        /// Gets the inventory cost in this time step
        /// </summary>
        public double InventoryCost { get; set; }

        /// <summary>
        /// Production capacity in the time step
        /// </summary>
        public double Capacity { get; set; }

        /// <summary>
        /// Name of the time step
        /// </summary>
        /// <returns>
        /// The name of this time step (<see cref="string"/>).
        /// </returns>
        public override string ToString() => Convert.ToString(Period);

    }
}
