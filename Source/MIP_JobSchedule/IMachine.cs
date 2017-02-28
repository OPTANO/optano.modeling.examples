using System.Collections.Generic;

namespace JobScheduling
{
    /// <summary>
    /// interface for a machine object
    /// </summary>
    public interface IMachine
    {
        /// <summary>
        /// the cost for setting up this machine
        /// </summary>
        double Cost { get; set; }
        
        /// <summary>
        /// the name of the machine
        /// </summary>
        string MachineId { get; set; }

        /// <summary>
        /// the jobs that run on this machine
        /// </summary>
        Dictionary<Job, int> SetupTimes { get; set; }

        /// <summary>
        /// the jobs this machine supports
        /// </summary>
        List<Task> SupportedTasks { get; set; }

        /// <summary>
        /// a readable representation of this machine
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}