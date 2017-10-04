using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MIP_CLSP;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CLSP
{
    /// <summary>
    /// Helper Class for creating and exporting an <see cref="OxyPlot"/> chart.
    /// </summary>
    public static class PlottingUtils
    {
        /// <summary>
        /// Creates, populates and exports a plot that shows the
        /// * produced amounts
        /// * satisfied demands by source (current production or from inventory)
        /// * remaining inventory at the end of a period.
        /// </summary>
        /// <param name="solvedClspModel">It is assumed that <see cref="CapacitatedLotsizingModel.Model"/> is solved and the Solution values are written to the Variables.</param>
        public static void CreateAndExportLotSizingPlot(CapacitatedLotsizingModel solvedClspModel)
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