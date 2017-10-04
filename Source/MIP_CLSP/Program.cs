using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using MIP_CLSP;
using OPTANO.Modeling.Optimization.Solver;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CLSP
{
    using OPTANO.Modeling.Common;
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Configuration;
    using OPTANO.Modeling.Optimization.Solver.Gurobi75x;

    /// <summary>
    /// Demo program solving a Capacitated Lot-Sizing Problem
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
            // create time steps with their "name", demand, setup cost,
            // production cost per unit, inventory cost per unit
            var csv = new CsvReader(File.OpenText("timesteps.csv"));
            csv.Configuration.Delimiter = ";";
            csv.Configuration.CultureInfo = new CultureInfo("en-US");
            var periodInformation = csv.GetRecords<PeriodInformation>();

            // use default settings
            var config = new Configuration
            {
                NameHandling = NameHandlingStyle.Manual,
                ComputeRemovedVariables = true
            };
            using (var scope = new ModelScope(config))
            {

                // create a model, based on given data and the model scope
                var clspModel = new CapacitatedLotsizingModel(periodInformation);

                var solverCfg = new GurobiSolverConfiguration()
                {
                    ModelOutputFile = new FileInfo("clsp.lp"),
                };

                // Get a solver instance, change your solver
                var solver = new GurobiSolver(solverCfg);

                // solve the model
                var solution = solver.Solve(clspModel.Model);
                
                // print objective and variable decisions
                Console.WriteLine($"{solution.ObjectiveValues.Single()}");
                clspModel.y.Variables.ForEach(x => Console.WriteLine($"{x.ToString().PadRight(36)}: {x.Value}"));
                clspModel.x.Variables.ForEach(y => Console.WriteLine($"{y.ToString().PadRight(36)}: {y.Value}"));
                clspModel.s.Variables.ForEach(s => Console.WriteLine($"{s.ToString().PadRight(36)}: {s.Value}"));

                clspModel.Model.VariableStatistics.WriteCSV(AppDomain.CurrentDomain.BaseDirectory);

                CreateAndExportLotSizingPlot(clspModel);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Creates, populates and exports a plot that shows the
        /// * produced amounts
        /// * satisfied demands by source (current production or from inventory)
        /// * remaining inventory at the end of a period.
        /// </summary>
        /// <param name="solvedClspModel">It is assumed that <see cref="CapacitatedLotsizingModel.Model"/> is solved and the Solution values are written to the Variables.</param>
        private static void CreateAndExportLotSizingPlot(CapacitatedLotsizingModel solvedClspModel)
        {
            var plotModel = CreatePlottingCanvas();

            AddProducedAmountToPlot(solvedClspModel, plotModel);
            AddDemandSatisfactionToPlot(solvedClspModel, plotModel);
            AddRemainStorageToPlot(solvedClspModel, plotModel);

            ExportPlot(plotModel);
        }

        private static PlotModel CreatePlottingCanvas()
        {
            var plotModel = new PlotModel()
            {
                Title = "Demand Satisfaction",
                LegendBorderThickness = 0,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
            };
            var xAxis = new CategoryAxis();
            xAxis.Labels.AddRange(Enumerable.Range(1, 8).Select(i => $"Period {i}"));
            plotModel.Axes.Add(xAxis);
            var yAxis = new LinearAxis
            {
                AbsoluteMinimum = 0,
                MaximumPadding = 0.06,
                MinimumPadding = 0,
                MajorGridlineStyle = LineStyle.LongDash,
                MajorStep = 10,
                MinorGridlineStyle = LineStyle.Dot,
                MinorStep = 5,
                Title = "Units"
            };
            plotModel.Axes.Add(yAxis);
            return plotModel;
        }

        private static void AddProducedAmountToPlot(CapacitatedLotsizingModel clspModel, PlotModel oxyPlot)
        {
            var currentProduction = clspModel.PeriodInformation.Select(pi => new ColumnItem(clspModel.x[pi].Value)).ToList();

            var productionSeries = new ColumnSeries()
            {
                Title = "Current Production",
                FillColor = OxyColor.FromRgb(byte.Parse("165"), byte.Parse("185"), byte.Parse("79"))
            };
            productionSeries.Items.AddRange(currentProduction);
            oxyPlot.Series.Add(productionSeries);
        }

        private static void AddDemandSatisfactionToPlot(CapacitatedLotsizingModel clspModel, PlotModel oxyPlot)
        {
            var usedStorage = new List<ColumnItem>();
            var usedProduction = new List<ColumnItem>();

            foreach (var currentPeriod in clspModel.PeriodInformation)
            {
                var endStorage = clspModel.s[currentPeriod].Value;
                var producedAmount = clspModel.x[currentPeriod].Value;
                // basically represents the flow conservation constraint
                var initialStorage = -producedAmount + endStorage + currentPeriod.Demand;

                // split bar item in 2 sections.
                var demandFromStorage = Math.Min(initialStorage, currentPeriod.Demand);
                var demandFromProduction = Math.Min(currentPeriod.Demand - demandFromStorage, producedAmount);
                usedStorage.Add(new ColumnItem(demandFromStorage));
                usedProduction.Add(new ColumnItem(demandFromProduction));
            }

            var storageSeries = new ColumnSeries()
            {
                Title = "Demand from Inventory",
                IsStacked = true,
                StackGroup = "DemandSatisfaction",
                FillColor = OxyColor.FromRgb(byte.Parse("132"), byte.Parse("122"), byte.Parse("178"))
            };
            storageSeries.Items.AddRange(usedStorage);

            var productionSeries = new ColumnSeries()
            {
                Title = "Demand from Production",
                IsStacked = true,
                StackGroup = "DemandSatisfaction",
                FillColor = OxyColor.FromRgb(byte.Parse("245"), byte.Parse("174"), byte.Parse("78"))
            };
            productionSeries.Items.AddRange(usedProduction);

            oxyPlot.Series.Add(storageSeries);
            oxyPlot.Series.Add(productionSeries);
        }

        private static void AddRemainStorageToPlot(CapacitatedLotsizingModel clspModel, PlotModel oxyPlot)
        {
            var dataPoints = clspModel.PeriodInformation.Select(pi => new ColumnItem(clspModel.s[pi].Value)).ToList();

            var series = new ColumnSeries()
            {
                Title = "Remaining Inventory",
                FillColor = OxyColor.FromRgb(byte.Parse("82"), byte.Parse("35"), byte.Parse("152"))
            };

            series.Items.AddRange(dataPoints);
            oxyPlot.Series.Add(series);
        }

        private static void ExportPlot(PlotModel plotModel)
        {
            using (var fileStream = File.Create("clspPlot.svg"))
            {
                var svgExporter = new SvgExporter() { Width = 600, Height = 400 };
                svgExporter.Export(plotModel, fileStream);
            }

            using (var fileStream = File.Create("clspPlot.pdf"))
            {
                var pdfExporter = new PdfExporter() { Width = 600, Height = 400 };
                pdfExporter.Export(plotModel, fileStream);
            }
        }
    }
}
