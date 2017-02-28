namespace WLP
{
    /// <summary>
    /// Interface for a node object
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// creation cost of the node, 0 if it is a customer
        /// </summary>
        double CreationCost { get; }

        /// <summary>
        /// demand of the node, a negative demand is supply (warehouse)
        /// </summary>
        double Demand { get; }

        /// <summary>
        /// the name of the node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// a readable representation of the node
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}