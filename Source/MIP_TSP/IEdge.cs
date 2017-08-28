
namespace TSP
{
    public interface IEdge
    {
        /// <summary>
        /// distance of the edge, double if specified else null is replaced with Double.positiveInfinity
        /// </summary>
        double? Distance { get; }

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