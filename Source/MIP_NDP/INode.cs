namespace NDP
{   
    /// <summary>
    /// Interface for a node object
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// specifies the demand of the node
        /// </summary>
        double Demand { get; }

        /// <summary>
        /// name of the node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// readable representation of the node
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}