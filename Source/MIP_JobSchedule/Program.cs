using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduling
{
    using System.Data.SqlClient;
    using System.Drawing;
    using System.IO;

    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Enums;
    using OPTANO.Modeling.Optimization.Solver.Cplex127;

    /// <summary>
    /// Program for solving the job scheduling problem
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">
        /// no arguments required
        /// </param>
        static void Main(string[] args)
        {
            // create jobs with their respective color and due date
            var jobs = new List<Job>
                           {
                               new Job { Color = Color.White, DueDate = 40 },
                               new Job { Color = Color.Brown, DueDate = 40 },
                               new Job { Color = Color.Green, DueDate = 40 },
                               new Job { Color = Color.Black, DueDate = 40 },
                           };

            // add setup times for the jobs created beforehand
            var setupTimes = new Dictionary<Job, int>()
                            {
                                    { jobs.Single(j => j.Color == Color.White), 4 },
                                    { jobs.Single(j => j.Color == Color.Brown), 2 },
                                    { jobs.Single(j => j.Color == Color.Green), 3 },
                                    { jobs.Single(j => j.Color == Color.Black), 0 },
                            };

            // add tasks to the different jobs created beforehand
            var tasks = new List<Task>
                            {
                                //  white
                                new Task() { Job = jobs.Single(j => j.Color == Color.White), StepNumber = 1, Duration = 4 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.White), StepNumber = 2, Duration = 3 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.White), StepNumber = 3, Duration = 4 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.White), StepNumber = 4, Duration = 2 },

                                // brown
                                new Task() { Job = jobs.Single(j => j.Color == Color.Brown), StepNumber = 1, Duration = 4 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Brown), StepNumber = 2, Duration = 6 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Brown), StepNumber = 3, Duration = 4 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Brown), StepNumber = 4, Duration = 3 },

                                // green
                                new Task() { Job = jobs.Single(j => j.Color == Color.Green), StepNumber = 1, Duration = 3 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Green), StepNumber = 2, Duration = 4 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Green), StepNumber = 3, Duration = 3 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Green), StepNumber = 4, Duration = 3 },

                                // black
                                new Task() { Job = jobs.Single(j => j.Color == Color.Black), StepNumber = 1, Duration = 4 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Black), StepNumber = 2, Duration = 8 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Black), StepNumber = 3, Duration = 2 },
                                new Task() { Job = jobs.Single(j => j.Color == Color.Black), StepNumber = 4, Duration = 8 },

                                };

            // create a rank for each task
            var ranks = Enumerable.Range(0, tasks.Count).ToList();

            // set up the machines with their name, the beforehand created setup times and the supported tasks as well as their setup cost
            var machines = new List<Machine>
                               {
                                   new Machine
                                       {
                                           MachineId = "A",
                                           SetupTimes = setupTimes,
                                           SupportedTasks = tasks.Where(task => new int[] { 1, 2 }.Contains(task.StepNumber)).ToList(),
                                           Cost = 1
                                       },
                                   new Machine
                                       {
                                           MachineId = "B",
                                           SetupTimes = setupTimes,
                                           SupportedTasks = tasks.Where(task => new int[] { 1, 2, 3 }.Contains(task.StepNumber)).ToList(),
                                           Cost = 2
                                       },
                                   new Machine
                                       {
                                           MachineId = "C",
                                           SetupTimes = setupTimes,
                                           SupportedTasks = tasks.Where(task => new int[] { 2, 3, 4 }.Contains(task.StepNumber)).ToList(),
                                           Cost = 3
                                       },
                                   new Machine
                                       {
                                           MachineId = "D",
                                           SetupTimes = setupTimes,
                                           SupportedTasks = tasks.Where(task => new int[] { 3, 4 }.Contains(task.StepNumber)).ToList(),
                                           Cost = 4
                                       },
                               };


            // register tasks with jobs
            jobs.ForEach(job => job.Tasks.AddRange(tasks.Where(task => task.Job == job).OrderBy(task => task.StepNumber)));

            var scopeSettings = new OptimizationConfigSection();
            scopeSettings.ModelElement.EnableFullNames = true;
            scopeSettings.ModelElement.ComputeRemovedVariables = true;


            using (var scope = new ModelScope(scopeSettings))
            {

                // create a model, based on given data and the model scope
                var jobScheduleModel = new JobScheduleModel(jobs, setupTimes, tasks, ranks, machines);

                // Get a solver instance, change your solver
                CplexSolverConfiguration config = new CplexSolverConfiguration();
                config.TimeLimit = 120;
                var solver = new CplexSolver(config);

                // solve the model
                var solution = solver.Solve(jobScheduleModel.Model);

                // print objective and variable decisions
                Console.WriteLine($"Objective: {solution.ObjectiveValues.Single().Key} {(int)Math.Round(solution.ObjectiveValues.Single().Value)}");
                Console.WriteLine($"Latest End: {(int)jobScheduleModel.LatestEnd.Value}");

                foreach (var machine in machines)
                {
                    foreach (var rank in ranks)
                    {
                        foreach (var task in machine.SupportedTasks)
                        {
                            if ((int)Math.Round(jobScheduleModel.taskMachineAssignment[task, machine, rank].Value) > 0)
                            {
                                Console.WriteLine(
                                    $"Machine {machine}, Rank {rank}: Assigns Task={task}, Start: {(int)Math.Round(jobScheduleModel.startTime[task, machine, rank].Value):####}, Duration: {task.Duration:##}, End: {(int)Math.Round(jobScheduleModel.startTime[task, machine, rank].Value) + task.Duration:####}");
                            }
                        }
                    }

                    Console.WriteLine("---");
                }

                foreach (var job in jobs)
                {
                    foreach (var task in job.Tasks)
                    {
                        foreach (var machine in machines.Where(m => m.SupportedTasks.Contains(task)))
                        {
                            foreach (var rank in ranks)
                            {
                                if ((int)Math.Round(jobScheduleModel.taskMachineAssignment[task, machine, rank].Value) > 0)
                                {
                                    Console.WriteLine(
                                        $"Task={task}, Rank {rank}: Assigned Machine {machine}, Start: {(int)Math.Round(jobScheduleModel.startTime[task, machine, rank].Value):####}, Duration: {task.Duration:##}, End: {(int)Math.Round(jobScheduleModel.startTime[task, machine, rank].Value) + task.Duration:####}");
                                }
                            }
                        }
                    }

                    Console.WriteLine("---");
                }
            }
        }
    }
}
