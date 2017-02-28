namespace SetProblems
{
    /// <summary>
    /// Interface for an element
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// the name of the element
        /// </summary>
        string Name { get; }
        /// <summary>
        /// the value of the element
        /// </summary>
        double Value { get; }

        /// <summary>
        /// a readable representation of the element
        /// </summary>
        /// <returns>string</returns>
        string ToString();
    }
}