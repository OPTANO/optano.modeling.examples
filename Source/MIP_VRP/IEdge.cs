namespace VRP
{
    /// <summary>
    /// interface for an edge object
    /// </summary>
    public interface IEdge
    {
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
        /// transportation cost for one unit on the edge
        /// </summary>
        double? transportationCost { get; }

        /// <summary>
        /// a readable representation of the edge
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}