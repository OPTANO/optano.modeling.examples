namespace CLSP
{
    /// <summary>
    /// Interface for a time step object
    /// </summary>
    public interface ITimestep
    {
        /// <summary>
        /// the demand of that time step, double if specified else null is replaced with Double.positiveInfinity
        /// </summary>
        double? Demand { get; }

        /// <summary>
        /// the inventory cost in the time step
        /// </summary>
        double InventoryCost { get; }

        /// <summary>
        /// the name of the time step
        /// </summary>
        int Name { get; }

        /// <summary>
        /// the production cost in the time step
        /// </summary>
        double ProductionCost { get; }

        /// <summary>
        /// the setup cost in the time step
        /// </summary>
        double SetupCost { get; }

        /// <summary>
        /// the production capacity in the time step
        /// </summary>
        double Capacity { get; }

        /// <summary>
        /// a readable representation of the time step
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}