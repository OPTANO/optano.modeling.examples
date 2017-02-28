namespace WLP
{
    /// <summary>
    /// Interface for an edge object
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// capacity of the edge
        /// </summary>
        double Capacity { get; }
        
        /// <summary>
        /// cost for transporting one unit on this edge
        /// </summary>
        double CostPerFlowUnit { get; }

        /// <summary>
        /// amount of units transported on this edge
        /// </summary>
        double Flow { get; set; }

        /// <summary>
        /// starting node of the edge (origin)
        /// </summary>
        INode FromNode { get; }

        /// <summary>
        /// ending node of the edge (destination)
        /// </summary>
        INode ToNode { get; }

        /// <summary>
        /// indicates whether the edge is used (true) or not (false)
        /// </summary>
        bool IsUsed { get; set; }

        /// <summary>
        /// a readable representation of the node
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}