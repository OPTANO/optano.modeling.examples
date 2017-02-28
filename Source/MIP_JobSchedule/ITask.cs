namespace JobScheduling
{
    /// <summary>
    /// interface for the task objects
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// the duration of this task
        /// </summary>
        int Duration { get; set; }

        /// <summary>
        /// the job this task belongs to
        /// </summary>
        Job Job { get; set; }

        /// <summary>
        /// the time step this task belongs to
        /// </summary>
        int StepNumber { get; set; }

        /// <summary>
        /// a readable representation of this task
        /// </summary>
        /// <returns>String</returns>
        string ToString();
    }
}