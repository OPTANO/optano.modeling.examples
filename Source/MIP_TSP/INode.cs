namespace TSP
{
    /// <summary>
    /// Interface for a node object
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Name of the node
        /// </summary>
        string Name { get; }

        /// <summary>
        /// indicates whether this node is a starting node (true) or not (false)
        /// </summary>
        bool IsStartingNode { get; set; }
  
        /// <summary>
        /// readable representation of the node
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}