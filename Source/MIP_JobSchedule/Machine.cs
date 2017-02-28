using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduling
{
    using System.Drawing;
    /// <summary>
    /// a machine for the job scheduling problem
    /// </summary>
    public class Machine : IMachine
    {
        /// <summary>
        /// label for the machine
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// the tasks <see cref="JobScheduling.Task"/> that are supported by this machine
        /// if a machine is assigned to a job those tasks need to match these
        /// </summary>
        public List<Task> SupportedTasks { get; set; } = new List<Task>();

        /// <summary>
        /// set-up time for the machine to do a certain job <see cref="JobScheduling.Job"/>
        /// </summary>
        public Dictionary<Job, int> SetupTimes {get; set;} = new Dictionary<Job, int>();

        /// <summary>
        /// cost for running the machine
        /// </summary>
        public double Cost { get; set; }

        /// <summary>
        /// a readable representation of the machine
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return MachineId;
        }
    }
}
