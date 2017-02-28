namespace Knapsack
{
    /// <summary>
    /// Interface for an item object
    /// </summary>
    public interface IKnapsackItem
    {
        /// <summary>
        /// the name of the item
        /// </summary>
        string Name { get; }
        /// <summary>
        /// the value of the item
        /// </summary>
        double Value { get; }
        /// <summary>
        /// the weight of the item
        /// </summary>
        double Weight { get; }

        /// <summary>
        /// a readable representation of the item
        /// </summary>
        /// <returns>string</returns>
        string ToString();
    }
}