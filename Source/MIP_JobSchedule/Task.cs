using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduling
{
    using System.Drawing;
    /// <summary>
    /// a task for the job scheduling problem - belongs to a job
    /// </summary>
    public class Task : ITask
    {
        /// <summary>
        ///  the job, this task belongs to - labeled by a color <see cref="JobScheduling.Job"/>
        /// </summary>
        public Job Job { get; set; }

        /// <summary>
        /// time step belonging to this task
        /// </summary>
        public int StepNumber { get; set; }

        /// <summary>
        /// duration of this task
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// a readable representation of the task
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"{this.Job}_{StepNumber-1}");
        }
    }
}
