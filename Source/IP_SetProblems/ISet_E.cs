using System.Collections.Generic;

namespace SetProblems
{
    /// <summary>
    /// Interface for the sets of elements
    /// </summary>
    public interface ISet_E
    {
        /// <summary>
        /// the cost of the set
        /// </summary>
        double Cost { get; }    // if the cost is 0 it is the original full set!

        /// <summary>
        ///  indicates whether the Set is the original full Set
        /// </summary>
        bool IsFullSet { get; set; }

        /// <summary>
        /// the elements contained in the set
        /// </summary>
        IEnumerable<IElement> Elements { get; }

        /// <summary>
        /// the name of the list
        /// </summary>
        string Name { get; }

        /// <summary>
        /// indicates whether the given element is contained in this Set (true) or not (false)
        /// </summary>
        /// <param name="element"></param>
        /// <returns>boolean</returns>
        bool ContainsElement(IElement element);

        /// <summary>
        /// a readable representation of the set
        /// </summary>
        /// <returns>string</returns>
        string ToString();
    }
}