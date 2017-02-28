namespace NDP
{   
    /// <summary>
    /// Interface for an edge object
    /// </summary>
    public interface IEdge
    {   
        /// <summary>
        /// capacity of the edge
        /// </summary>
        double? Capacity { get; }

        /// <summary>
        /// cost per unit transported over the edge
        /// </summary>
        double CostPerFlowUnit { get; }

        /// <summary>
        /// cost for using the edge
        /// </summary>
        double DesignCost { get; }

        /// <summary>
        /// number of units transported over the edge
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
        /// readable representation of the edge
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}