using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetProblems
{
    /// <summary>
    /// creates a Set for the Set Problem(s)
    /// </summary>
    public class Set_E : ISet_E
    {

        public Set_E(string name, IEnumerable<IElement> elements, double cost, bool isFullSet)
        {
            this.Name = name;
            this.Elements = new HashSet<IElement>(elements);
            this.Cost = cost;           // if the cost is 0 it is the original full set!
            this.IsFullSet = isFullSet;
        }

        /// <summary>
        /// the list of IElements
        /// </summary>
        public HashSet<IElement> Elements { get; }

        /// <summary>
        /// the cost of the set
        /// </summary>
        public double Cost { get; }

        /// <summary>
        /// indicates whether the Set is the original full Set
        /// </summary>
        public bool IsFullSet { get; set; }

        /// <summary>
        /// name of the set
        /// </summary>
        public string Name { get; }

        IEnumerable<IElement> ISet_E.Elements
        {
            get
            {
                return this.Elements;
            }
        }

        /// <summary>
        /// readable representation of the set
        /// </summary>
        /// <returns>string</returns>
        public override string ToString() {
            var setName = this.Name + " with elements: ";
            foreach(var element in this.Elements)
            {
                setName = setName + element.Name + ", " ;
            }
            return setName;
        }

        /// <summary>
        /// indicates whether the given element is contained in this Set (true) or not (false)
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool ContainsElement(IElement element)
        {
            return this.Elements.Contains(element);
        }
    }
}

