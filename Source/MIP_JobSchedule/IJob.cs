using System.Collections.Generic;
using System.Drawing;

namespace JobScheduling
{
    /// <summary>
    /// interface for a job object
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// the identifier for this job
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// the due date of this job
        /// </summary>
        int DueDate { get; set; }

        /// <summary>
        /// the list of tasks contained in this job
        /// </summary>
        List<Task> Tasks { get; set; }

        /// <summary>
        /// a readable representation of this job
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}