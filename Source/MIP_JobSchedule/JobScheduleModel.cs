using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduling
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Enums;
    using OPTANO.Modeling.Optimization.Operators;

    /// <summary>
    /// A Job Scheduling Model
    /// </summary>
    class JobScheduleModel
    {
        // DISCLAIMER: The Job scheduling problem is a CP by nature, but this one is deliberately modeled as a MIP

        /// <summary>
        /// Initializes a new instance of the <see cref="JobScheduleModel"/> class and initializes all fields. 
        /// </summary>
        /// <param name="jobs">
        /// The jobs for the job scheduling problem
        /// </param>
        /// <param name="setupTimes">
        /// The setup times for the machines
        /// </param>
        /// <param name="tasks">
        /// The different tasks of the jobs
        /// </param>
        /// <param name="ranks">
        /// The ranks of the tasks
        /// </param>
        /// <param name="machines">
        /// The different machines
        /// </param>
        public JobScheduleModel(List<Job> jobs, Dictionary<Job, int> setupTimes, List<Task> tasks, List<int> ranks, List<Machine> machines)
        {
            this.Jobs = jobs;
            this.SetupTimes = setupTimes;
            this.Tasks = tasks;
            this.Ranks = ranks;
            this.Machines = machines;

            this.Model = new Model();

            // create decision variables

            // start time of each task
            this.startTime = new VariableCollection<Task, Machine, int>(
                this.Model,
                this.Tasks,
                this.Machines,
                this.Ranks,
                "StartTime",
                (t, m, r) => $"StartTime_t{t}_m{m}_r{r}",
                (t, m, r) => 0,
                (t, m, r) => double.PositiveInfinity,
                VariableType.Continuous,
                null);

            // assignment of the tasks to the machines
            this.taskMachineAssignment = new VariableCollection<Task, Machine, int>(
                this.Model,
                this.Tasks,
                this.Machines,
                this.Ranks,
                "taskMachineAssignment",
                (t, m, r) => $"Assignment_t{t}_m{m}_r{r}",
                (t, m, r) => 0,
                (t, m, r) => 1,
                VariableType.Binary,
                (t, m, r) => t.StepNumber);

            // delay of the jobs
            this.jobDelay = new VariableCollection<Job>(
                this.Model,
                this.Jobs,
                "Delays",
                (j) => $"Delay_j{j}",
                (j) => 0,
                (j) => double.PositiveInfinity,
                VariableType.Continuous,
                null);

            // Create Constraints

            // latest end time of all tasks
            this.LatestEnd = new Variable("LatestEnd", 0, Double.MaxValue, VariableType.Continuous);


            // Each task must have a machine
            foreach (var task in this.Tasks)
            {
                this.Model.AddConstraint(
                    Expression.Sum(this.Machines.Where(m => m.SupportedTasks.Contains(task)).SelectMany(machine => this.Ranks.Select(rank => taskMachineAssignment[task, machine, rank]))) == 1,
                    string.Format($"EachTaskMustHaveMachineAndRank_t{task}"));
            }

            // Each machine may have only one task on every rank
            foreach (var machine in this.Machines)
            {
                foreach (var rank in this.Ranks)
                {
                    this.Model.AddConstraint(
                        Expression.Sum(this.Tasks.Where(task => machine.SupportedTasks.Contains(task)).Select(task => taskMachineAssignment[task, machine, rank])) <= 1,
                        string.Format($"MaxOneTaskPerMachineAndRank_m{machine}_r{rank}"));
                }
            }


            // Each task must start after its predecessors starts
            foreach (var job in jobs)
            {
                var durationSum = 0;
                for (var taskNo = 0; taskNo < job.Tasks.Count - 1; taskNo++)
                {
                    var predecessor = job.Tasks[taskNo];
                    var successor = job.Tasks[taskNo + 1];

                    durationSum += predecessor.Duration;

                    this.Model.AddConstraint(
                        Expression.Sum(this.Machines.Where(machine => machine.SupportedTasks.Contains(successor)).SelectMany(machine => this.Ranks.Select(rank => startTime[successor, machine, rank])))
                        >= Expression.Sum(this.Machines.Where(machine1 => machine1.SupportedTasks.Contains(predecessor)).SelectMany(machine1 => this.Ranks.Select(rank1 => startTime[predecessor, machine1, rank1]))) + predecessor.Duration,
                        string.Format($"StartsAfter_t{predecessor}_t{successor}"));

                    this.Model.AddConstraint(
                        Expression.Sum(this.Machines.Where(machine => machine.SupportedTasks.Contains(successor))
                        .SelectMany(machine => this.Ranks.Select(rank => startTime[successor, machine, rank])))
                        >= durationSum,
                        string.Format($"StartsAfterSum_t{predecessor}_t{successor}"));
                }
            }

            // very latest end for worst case (durations plus setup in each and every step)
            var bigM1 = this.Tasks.Sum(task => task.Duration + setupTimes[task.Job]) + 1;
            foreach (var task in this.Tasks)
            {
                foreach (var machine in this.Machines.Where(machine => machine.SupportedTasks.Contains(task)))
                {
                    foreach (var rank in this.Ranks)
                    {
                        // If task Assignment is given, Start Time must be set
                        this.Model.AddConstraint(
                            startTime[task, machine, rank] >= taskMachineAssignment[task, machine, rank] * task.Job.Tasks.Where(nt => nt.StepNumber < task.StepNumber).Select(nt => nt.Duration).Sum(),
                            string.Format($"MatchStartTimeLB_t{task}_m{machine}_r{rank}"));

                        // if task is not set, start time must be zero (prevents splitting of task time in "FinishesAfter"
                        this.Model.AddConstraint(
                            startTime[task, machine, rank] <= bigM1 * taskMachineAssignment[task, machine, rank],
                            string.Format($"MatchStartTimeUB_t{task}_m{machine}_r{rank}"));

                        if (rank > 0)
                        {
                            foreach (var task1 in machine.SupportedTasks)
                            {
                                this.Model.AddConstraint(
                                    startTime[task, machine, rank] + ((1 - taskMachineAssignment[task, machine, rank]) * bigM1)
                                    >= startTime[task1, machine, rank - 1] + (taskMachineAssignment[task1, machine, rank - 1]
                                                   * (task1.Duration + (task.Job != task1.Job ? setupTimes[task.Job] : 0))),
                                    string.Format($"AddSetup_t{task}_m{machine}_r{rank}_t1{task1}"));
                            }
                        }
                    }
                }
            }

            // Calculate delays
            foreach (var job in jobs)
            {
                var task = job.Tasks.OrderBy(t => t.StepNumber).Last();
                foreach (var machine in this.Machines.Where(machine => machine.SupportedTasks.Contains(task)))
                {
                    foreach (var rank in this.Ranks)
                    {
                        this.Model.AddConstraint(jobDelay[job] >= startTime[task, machine, rank] + task.Duration - task.Job.DueDate,
                        string.Format($"Delay_j{job}_t{task}_r{rank}_m{machine}"));
                    }
                }
            }

            // find latest end
            foreach (var rank in this.Ranks)
            {
                foreach (var task in this.Tasks)
                {
                    foreach (var machine in this.Machines.Where(machine => machine.SupportedTasks.Contains(task)))
                    {
                        this.Model.AddConstraint(LatestEnd >= startTime[task, machine, rank] + task.Duration, string.Format($"LatestEnd_t{task}_m{machine}_r{rank}"));
                    }
                }
            }

            // Tuning: no free this.Ranks
            foreach (var rank in this.Ranks.Skip(1))
            {
                foreach (var machine in this.Machines)
                {
                    this.Model.AddConstraint(
                        Expression.Sum(machine.SupportedTasks.Select(task => taskMachineAssignment[task, machine, rank]))
                        <= Expression.Sum(machine.SupportedTasks.Select(task1 => taskMachineAssignment[task1, machine, rank - 1])),
                        string.Format($"BeautifyRanks_m{machine}_r{rank}"));
                }
            }

            // Debugging example: How to fix a certain job?

            //this.Model.AddConstraint(
            //    taskMachineAssignment[
            //        this.Tasks.Single(t => t.Job.Color == Color.White && t.StepNumber == 1),
            //        this.Machines.Single(m => m.MachineId == "A"),
            //        0] == 1);


            // Add the objective

            // NOTE: you have 4 different objectives here, which naturally correspond to different output
            //       you can play around with them to get a feel for the outcome just make sure that only one objective is active

            // min sum of start times
            // this.Model.AddObjective(new Objective(Expression.Sum(this.Machines.SelectMany(machine => machine.SupportedTasks.SelectMany(task => this.Ranks.Select(rank => startTime[task, machine, rank])))), "Min Start Time", ObjectiveSense.Minimize));

            // min job Delay (due time)
            // this.Model.AddObjective(new Objective(Expression.Sum(jobs.Select(job => jobDelay[job])), "Min Delay", ObjectiveSense.Minimize));


            // min latest end time
            //this.Model.AddObjective(new Objective(LatestEnd, "MinLatestEnd", ObjectiveSense.Minimize));
            
            // improved min latest end time
            this.Model.AddObjective(new Objective(LatestEnd
                + Expression.Sum(this.Machines.SelectMany(machine => machine.SupportedTasks.SelectMany(task => this.Ranks.Select(rank =>
                (taskMachineAssignment[task, machine, rank] * (machine.Cost + task.StepNumber) * 0.001)
                + (startTime[task, machine, rank] * 0.01))
                ))), "EndWeighted", ObjectiveSense.Minimize));

        }
       
        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the jobs for the scheduling problem
        /// </summary>
        public List<Job> Jobs { get; }

        /// <summary>
        /// Gets the setup times for the machines according to the jobs
        /// </summary>
        public Dictionary<Job, int> SetupTimes { get; }

        /// <summary>
        /// Gets the tasks for the jobs
        /// </summary>
        public List<Task> Tasks { get; }

        /// <summary>
        /// Gets the ranks for the tasks
        /// </summary>
        public List<int> Ranks { get; }

        /// <summary>
        /// Gets the configured machines
        /// </summary>
        public List<Machine> Machines { get; }

        /// <summary>
        /// Gets the latest end time (used for objective)
        /// </summary>
        public Variable LatestEnd { get; }

        /// <summary>
        /// Gets the Collection of the job delay variable
        /// </summary>
        public VariableCollection<Job> jobDelay { get; }

        /// <summary>
        /// Gets the Collection of starting times of the tasks on the machines
        /// </summary>
        public VariableCollection<Task, Machine, int> startTime { get; }

        /// <summary>
        /// Gets the Collection of task machine assignments
        /// </summary>
        public VariableCollection<Task, Machine, int> taskMachineAssignment { get; }
    }
}
