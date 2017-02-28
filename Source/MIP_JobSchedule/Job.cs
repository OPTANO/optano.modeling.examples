using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduling
{
    using System.Drawing;
    /// <summary>
    /// A job for the job scheduling problem - consists of tasks
    /// </summary>
    public class Job : IJob
    {
        /// <summary>
        /// a list of tasks for this job <see cref="JobScheduling.Task"/>
        /// </summary>
        public List<Task> Tasks { get; set; } = new List<Task>();

        /// <summary>
        /// due date of the job including all tasks
        /// </summary>
        public int DueDate { get; set; }

        /// <summary>
        /// the color is an indicator for the job
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// a readable representation of the job
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"{Color}").Replace(" ", "").Replace("[", "").Replace("]", "");
        }
    }
}
