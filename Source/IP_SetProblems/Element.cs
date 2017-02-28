using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetProblems
{
    /// <summary>
    /// creates an element for the Set Problem(s)
    /// </summary>
    public class Element : IElement
    {

        public Element(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// the value of the element
        /// </summary>
        public double Value { get; }

        public string Name { get; }

        public override string ToString() => this.Name;
    }
}

