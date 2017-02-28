namespace VRP
{
    /// <summary>
    /// Interface for a node object
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// demand of the node
        /// </summary>
        double Demand { get; }
        /// <summary>
        /// name of the node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// indicates whether the node is a depot (true) or not (false)
        /// </summary>
        bool IsDepot { get; set; }

        /// <summary>
        /// a readable representation of the node
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}