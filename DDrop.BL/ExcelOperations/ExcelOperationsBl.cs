using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DDrop.BE.Enums;
using DDrop.BE.Models;
using DDrop.BL.CustomPlots;
using DDrop.BL.ExcelOperations;
using DDrop.BL.ExcelOperations.Models;
using DDrop.Utility.SeriesLocalStorageOperations;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace DDrop.Utility.ExcelOperations
{
    public class ExcelOperationsBl : IExcelOperationsBl
    {
        private readonly ICustomPlotsBl _customPlotsBl;

        public ExcelOperationsBl(ICustomPlotsBl customPlotsBl)
        {
            _customPlotsBl = customPlotsBl;
        }

        public async Task CreateSingleSeriesExcelFile(ExcelReport report)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var mainWorksheet = excelPackage.Workbook.Worksheets.Add("Общая информация");

                mainWorksheet.Cells["A1:C1"].Merge = true;
                mainWorksheet.Cells["A2:C2"].Merge = true;
                mainWorksheet.Cells["A3:C3"].Merge = true;
                mainWorksheet.Cells["A4:C4"].Merge = true;
                mainWorksheet.Cells["A5:C5"].Merge = true;

                mainWorksheet.Cells["D1:H1"].Merge = true;
                mainWorksheet.Cells["D2:H2"].Merge = true;
                mainWorksheet.Cells["D4:H4"].Merge = true;
                mainWorksheet.Cells["D5:H5"].Merge = true;

                mainWorksheet.Cells["A1"].Value = "Имя:";
                mainWorksheet.Cells["A2"].Value = "Фамилия:";
                mainWorksheet.Cells["A3"].Value = "Email:";
                mainWorksheet.Cells["A4"].Value = "Отчет от:";
                mainWorksheet.Cells["A5"].Value = "Безразмерные графики:";

                mainWorksheet.Cells["A1"].Style.Font.Bold = true;
                mainWorksheet.Cells["A2"].Style.Font.Bold = true;
                mainWorksheet.Cells["A3"].Style.Font.Bold = true;
                mainWorksheet.Cells["A4"].Style.Font.Bold = true;
                mainWorksheet.Cells["A5"].Style.Font.Bold = true;

                mainWorksheet.Cells["D1"].Value = report.UserFirstName;
                mainWorksheet.Cells["D2"].Value = report.UserLastName;
                mainWorksheet.Cells["D3"].Value = report.UserEmail;
                mainWorksheet.Cells["D4"].Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                mainWorksheet.Cells["D5"].Value = report.Demensionless ? "Да" : "Нет";

                var seriesCombinedRadiusChart = mainWorksheet.Drawings.AddChart("seriesCombinedRadiusChart", eChartType.XYScatterLines) as ExcelScatterChart;
                var seriesCombinedTemperatureChart = mainWorksheet.Drawings.AddChart("seriesCombinedTemperatureChart", eChartType.XYScatterLines) as ExcelScatterChart;

                seriesCombinedRadiusChart.Title.Text = "Зависимость радиуса капли от времени испарения";
                seriesCombinedRadiusChart.Legend.Position = eLegendPosition.Right;
                seriesCombinedRadiusChart.XAxis.Title.Text = report.Demensionless ? "Время" : "Время, с";
                seriesCombinedRadiusChart.YAxis.Title.Text = report.Demensionless ? "Радиус" : "Радиус, м";

                seriesCombinedTemperatureChart.Title.Text = "Зависимость температуры капли от времени испарения";
                seriesCombinedTemperatureChart.Legend.Position = eLegendPosition.Right;
                seriesCombinedTemperatureChart.XAxis.Title.Text = report.Demensionless ? "Время" : "Время, с";
                seriesCombinedTemperatureChart.YAxis.Title.Text = report.Demensionless ? "Температура" : "Температура, градусы Цельсия";

                var indexerRadius = 0;
                var indexerTemperature = 0;

                foreach (var plot in report.UserPlots)
                {
                    var plotWorkSheet = excelPackage.Workbook.Worksheets.Add($"График: {plot.Name}");

                    plotWorkSheet.Cells["A1:C1"].Merge = true;
                    plotWorkSheet.Cells["D1:G1"].Merge = true;

                    plotWorkSheet.Cells["A1"].Value = "Название графика:";
                    plotWorkSheet.Cells["A1"].Style.Font.Bold = true;
                    plotWorkSheet.Cells["D1"].Value = plot.Name;

                    var plotToExcelOutput = new ObservableCollection<PlotToExcel>();

                    plot.Points = JsonSerializeProvider.DeserializeFromString<List<SimplePoint>>(await _customPlotsBl.GetPlotPoints(plot.PlotId));

                    foreach (var point in plot.Points)
                    {
                        plotToExcelOutput.Add(new PlotToExcel
                        {
                            X = report.Demensionless ? point.X / plot.Settings.DimensionlessSettings.XDimensionlessDivider : point.X,
                            Y = report.Demensionless ? point.Y / plot.Settings.DimensionlessSettings.YDimensionlessDivider : point.Y
                        });
                    }

                    plot.Points = null;

                    plotWorkSheet.Cells["A3"].LoadFromCollection(plotToExcelOutput, true);
                    plotWorkSheet.Cells["A3:B3"].Style.Font.Bold = true;

                    if (plotToExcelOutput.Count > 0)
                    {
                        var end = plotWorkSheet.Dimension.End.Row;

                        var manualPlotChart = plotWorkSheet.Drawings.AddChart("manualPlotChart", eChartType.XYScatterLines) as ExcelScatterChart;

                        if (manualPlotChart != null)
                        {
                            manualPlotChart.Title.Text = plot.PlotType == PlotType.Temperature
                                ? $"Зависимость температуры капли от времени испарения для графика {plot.Name}"
                                : $"Зависимость радиуса капли от времени испарения для графика {plot.Name}";
                            manualPlotChart.Legend.Position = eLegendPosition.Right;

                            manualPlotChart.Series.Add(plotWorkSheet.Cells[$"B4:B{end}"], plotWorkSheet.Cells[$"A4:A{end}"]);

                            if (plot.PlotType == PlotType.Radius)
                            {
                                seriesCombinedRadiusChart.Series.Add(plotWorkSheet.Cells[$"B4:B{end}"], plotWorkSheet.Cells[$"A4:A{end}"]);
                                seriesCombinedRadiusChart.Series[indexerRadius].Header = plotWorkSheet.Cells["D1"].Value.ToString();
                                indexerRadius++;
                            }

                            if (plot.PlotType == PlotType.Temperature)
                            {
                                seriesCombinedTemperatureChart.Series.Add(plotWorkSheet.Cells[$"B4:B{end}"], plotWorkSheet.Cells[$"A4:A{end}"]);
                                seriesCombinedTemperatureChart.Series[indexerTemperature].Header = plotWorkSheet.Cells["D1"].Value.ToString();
                                indexerTemperature++;
                            }

                            manualPlotChart.XAxis.Title.Text = report.Demensionless ? "Время" : "Время, с";
                            manualPlotChart.YAxis.Title.Text = GetPlotYAxisTitle(plot.PlotType, report.Demensionless);

                            manualPlotChart.Series[0].Header = plotWorkSheet.Cells["D1"].Value.ToString();

                            manualPlotChart.SetSize(510, 660);
                            manualPlotChart.SetPosition(end + 1, 0, 0, 0);
                        }
                    }

                    plotWorkSheet.Cells.AutoFitColumns();
                }

                foreach (var currentSeries in report.Series)
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add($"{currentSeries.Title}");

                    worksheet.Cells["A1:D1"].Merge = true;
                    worksheet.Cells["A2:D2"].Merge = true;
                    worksheet.Cells["A3:D3"].Merge = true;
                    worksheet.Cells["A4:D4"].Merge = true;
                    worksheet.Cells["A5:D5"].Merge = true;
                    worksheet.Cells["A6:D6"].Merge = true;

                    worksheet.Cells["E1:H1"].Merge = true;
                    worksheet.Cells["E2:H2"].Merge = true;
                    worksheet.Cells["E3:H3"].Merge = true;
                    worksheet.Cells["E4:H4"].Merge = true;
                    worksheet.Cells["E5:H5"].Merge = true;
                    worksheet.Cells["E6:H6"].Merge = true;

                    worksheet.Cells["A1"].Value = "Название серии:";
                    worksheet.Cells["A2"].Value = "Интервал между снимками, c:";
                    worksheet.Cells["A3"].Value = "Пикселей в миллиметре (Спереди), px:";
                    worksheet.Cells["A4"].Value = "Пикселей в миллиметре (Сбоку), px:";
                    worksheet.Cells["A5"].Value = "Вещество";
                    worksheet.Cells["A6"].Value = "Акустическая";

                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A2"].Style.Font.Bold = true;
                    worksheet.Cells["A3"].Style.Font.Bold = true;
                    worksheet.Cells["A4"].Style.Font.Bold = true;
                    worksheet.Cells["A5"].Style.Font.Bold = true;
                    worksheet.Cells["A6"].Style.Font.Bold = true;

                    worksheet.Cells["A7:H7"].Merge = true;

                    worksheet.Cells["E1"].Value = currentSeries.Title;
                    worksheet.Cells["E2"].Value = currentSeries.IntervalBetweenPhotos;
                    worksheet.Cells["E3"].Value = currentSeries.ReferencePhotoForSeries?.FirstOrDefault(x => x.PhotoType == PhotoType.FrontDropPhoto)?.PixelsInMillimeter ?? 0;
                    worksheet.Cells["E4"].Value = currentSeries.ReferencePhotoForSeries?.FirstOrDefault(x => x.PhotoType == PhotoType.SideDropPhoto)?.PixelsInMillimeter ?? 0;
                    worksheet.Cells["E5"].Value = currentSeries.Substance.CommonName;
                    worksheet.Cells["E6"].Value = currentSeries.Settings.GeneralSeriesSettings.IsAcoustic ? "Да" : "Нет";

                    var singleSeriesToExcelOutput = new ObservableCollection<SeriesToExcel>();
                    var singleSeriesPlotToExcelOutput = new ObservableCollection<PlotToExcel>();

                    double averageAmbientTemperatures = 0;

                    for (var i = 0; i < currentSeries.MeasurementsSeries.Count; i++)
                    {
                        var measurement = currentSeries.MeasurementsSeries[i];

                        double time = 0.0;

                        averageAmbientTemperatures += measurement.AmbientTemperature ?? 0;

                        if (currentSeries.Settings.GeneralSeriesSettings.UseCreationDateTime)
                        {
                            time = (measurement.CreationDateTime - currentSeries.MeasurementsSeries[0].CreationDateTime).TotalSeconds;
                        }

                        if (measurement.Drop.RadiusInMeters != null)
                        {
                            singleSeriesToExcelOutput.Add(new SeriesToExcel
                            {
                                Time = currentSeries.Settings.GeneralSeriesSettings.UseCreationDateTime ? time : i * currentSeries.IntervalBetweenPhotos,
                                Name = measurement.Name,
                                RadiusInMeters = measurement.Drop.RadiusInMeters.Value,
                                VolumeInCubicalMeters = measurement.Drop.VolumeInCubicalMeters,
                                XDiameterInMeters = measurement.Drop.XDiameterInMeters,
                                YDiameterInMeters = measurement.Drop.YDiameterInMeters,
                                ZDiameterInMeters = measurement.Drop.ZDiameterInMeters,
                                Temperature = measurement.Drop.Temperature ?? 0
                            });
                        }
                    }

                    if (report.Demensionless)
                    {
                        if (singleSeriesToExcelOutput.Count > 0)
                        {
                            var initialRadius = singleSeriesToExcelOutput[0].RadiusInMeters;
                            var wholeEvaporationTime = singleSeriesToExcelOutput[singleSeriesToExcelOutput.Count - 1].Time;
                            averageAmbientTemperatures = averageAmbientTemperatures / currentSeries.MeasurementsSeries.Count(x => x.AmbientTemperature != 0);


                            for (int j = 0; j < singleSeriesToExcelOutput.Count; j++)
                            {
                                singleSeriesToExcelOutput[j].Time = singleSeriesToExcelOutput[j].Time / wholeEvaporationTime;
                                singleSeriesToExcelOutput[j].RadiusInMeters = singleSeriesToExcelOutput[j].RadiusInMeters / initialRadius;
                                singleSeriesToExcelOutput[j].Temperature = singleSeriesToExcelOutput[j].Temperature / averageAmbientTemperatures;
                            }
                        }
                    }

                    worksheet.Cells["A8:H8"].Merge = true;
                    worksheet.Cells["A8"].Value = "Данные";
                    worksheet.Cells["A8"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A8"].Style.Font.Bold = true;

                    worksheet.Cells["A9"].LoadFromCollection(singleSeriesToExcelOutput, true);
                    worksheet.Cells["A9:H9"].Style.Font.Bold = true;

                    if (currentSeries.Settings.GeneralSeriesSettings.UseThermalPlot && currentSeries.ThermalPlot != null)
                    {
                        worksheet.Cells["J8:Q8"].Merge = true;
                        worksheet.Cells["J8"].Value = "Дополнительный температурный график";
                        worksheet.Cells["J8"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells["J8"].Style.Font.Bold = true;

                        currentSeries.ThermalPlot.Points = JsonSerializeProvider.DeserializeFromString<List<SimplePoint>>(await _customPlotsBl.GetPlotPoints(currentSeries.ThermalPlot.PlotId));

                        foreach (var point in currentSeries.ThermalPlot.Points)
                        {
                            singleSeriesPlotToExcelOutput.Add(new PlotToExcel
                            {
                                X = report.Demensionless ? point.X / currentSeries.ThermalPlot.Settings.DimensionlessSettings.XDimensionlessDivider : point.X,
                                Y = report.Demensionless ? point.Y / currentSeries.ThermalPlot.Settings.DimensionlessSettings.YDimensionlessDivider : point.Y
                            });
                        }

                        worksheet.Cells["J9"].LoadFromCollection(singleSeriesPlotToExcelOutput, true);
                        worksheet.Cells["J9:K9"].Style.Font.Bold = true;
                    }

                    var end = worksheet.Dimension.End.Row;

                    if (singleSeriesToExcelOutput.Count > 0)
                    {
                        var seriesRadiusChart = worksheet.Drawings.AddChart("seriesRadiusChart", eChartType.XYScatterLines) as ExcelScatterChart;

                        seriesRadiusChart.Title.Text = $"Зависимость радиуса капли от времени испарения для серии {currentSeries.Title}";
                        seriesRadiusChart.Legend.Position = eLegendPosition.Right;

                        seriesRadiusChart.Series.Add(worksheet.Cells[$"F10:F{10 + (singleSeriesToExcelOutput.Count - 1)}"], worksheet.Cells[$"A10:A{10 + (singleSeriesToExcelOutput.Count - 1)}"]);
                        seriesCombinedRadiusChart.Series.Add(worksheet.Cells[$"F10:F{10 + (singleSeriesToExcelOutput.Count - 1)}"], worksheet.Cells[$"A10:A{10 + (singleSeriesToExcelOutput.Count - 1)}"]);

                        seriesRadiusChart.XAxis.Title.Text = report.Demensionless ? "Время" : "Время, с";
                        seriesRadiusChart.YAxis.Title.Text = report.Demensionless ? "Радиус" : "Радиус, м";

                        if (!currentSeries.Settings.GeneralSeriesSettings.UseThermalPlot && singleSeriesToExcelOutput.Any(x => x.Temperature != 0 && !double.IsNaN(x.Temperature)))
                        {
                            var seriesTemperatureChart = worksheet.Drawings.AddChart("seriesTemperatureChart", eChartType.XYScatterLines) as ExcelScatterChart;
                             
                            seriesTemperatureChart.Title.Text = $"Зависимость радиуса капли от времени испарения для серии {currentSeries.Title}";
                            seriesTemperatureChart.Legend.Position = eLegendPosition.Right;

                            seriesTemperatureChart.Series.Add(worksheet.Cells[$"H10:H{10 + (singleSeriesToExcelOutput.Count - 1)}"], worksheet.Cells[$"A10:A{10 + (singleSeriesToExcelOutput.Count - 1)}"]);
                            seriesCombinedTemperatureChart.Series.Add(worksheet.Cells[$"H10:H{10 + (singleSeriesToExcelOutput.Count - 1)}"], worksheet.Cells[$"A10:A{10 + (singleSeriesToExcelOutput.Count - 1)}"]);

                            seriesTemperatureChart.XAxis.Title.Text = report.Demensionless ? "Время" : "Время, с";
                            seriesTemperatureChart.YAxis.Title.Text = report.Demensionless ? "Температура" : "Температура, градусы Цельсия";

                            seriesTemperatureChart.Series[0].Header = worksheet.Cells["E1"].Value.ToString();
                            seriesCombinedTemperatureChart.Series[indexerTemperature].Header = worksheet.Cells["E1"].Value.ToString();

                            seriesTemperatureChart.SetSize(510, 660);
                            seriesTemperatureChart.SetPosition(end + 1, 0, 10, 0);

                            indexerTemperature++;
                        }

                        seriesRadiusChart.Series[0].Header = worksheet.Cells["E1"].Value.ToString();
                        seriesCombinedRadiusChart.Series[indexerRadius].Header = worksheet.Cells["E1"].Value.ToString();

                        seriesRadiusChart.SetSize(510, 660);
                        seriesRadiusChart.SetPosition(end + 1, 0, 0, 0);

                        worksheet.Cells.AutoFitColumns();
                        indexerRadius++;
                    }

                    if (currentSeries.Settings.GeneralSeriesSettings.UseThermalPlot && currentSeries.ThermalPlot != null)
                    {
                        var seriesTemperatureChart = worksheet.Drawings.AddChart("seriesTemperatureChart", eChartType.XYScatterLines) as ExcelScatterChart;

                        seriesTemperatureChart.Title.Text = $"Зависимость радиуса капли от времени испарения для серии {currentSeries.Title}";
                        seriesTemperatureChart.Legend.Position = eLegendPosition.Right;

                        seriesTemperatureChart.Series.Add(worksheet.Cells[$"K10:K{10 + (singleSeriesPlotToExcelOutput.Count - 1)}"], worksheet.Cells[$"J10:J{10 + (singleSeriesPlotToExcelOutput.Count - 1)}"]);
                        seriesCombinedTemperatureChart.Series.Add(worksheet.Cells[$"K10:K{10 + (singleSeriesPlotToExcelOutput.Count - 1)}"], worksheet.Cells[$"J10:J{10 + (singleSeriesPlotToExcelOutput.Count - 1)}"]);

                        seriesTemperatureChart.XAxis.Title.Text = report.Demensionless ? "Время" : "Время, с";
                        seriesTemperatureChart.YAxis.Title.Text = report.Demensionless ? "Температура" : "Температура, градусы Цельсия";

                        seriesTemperatureChart.Series[0].Header = worksheet.Cells["E1"].Value.ToString();
                        seriesCombinedTemperatureChart.Series[indexerTemperature].Header = worksheet.Cells["E1"].Value.ToString();
                            
                        seriesTemperatureChart.SetSize(510, 660);
                        seriesTemperatureChart.SetPosition(end + 1, 0, 10, 0);

                        indexerTemperature++;
                    }
                }

                seriesCombinedRadiusChart.SetSize(510, 660);
                seriesCombinedRadiusChart.SetPosition(mainWorksheet.Dimension.End.Row + 1, 0, 0, 0);

                seriesCombinedTemperatureChart.SetSize(510, 660);
                seriesCombinedTemperatureChart.SetPosition(mainWorksheet.Dimension.End.Row + 1, 0, 10, 0);


                mainWorksheet.Cells.AutoFitColumns();              

                var excelFile = new FileInfo($@"{report.FileName}");
                excelPackage.SaveAs(excelFile);
            }
        }

        private string GetPlotYAxisTitle(PlotType plotType, bool dimensionless)
        {
            if (plotType == PlotType.Radius && dimensionless)
            {
                return "Радиус";
            }
            else if (plotType == PlotType.Radius && !dimensionless)
            {
                return "Радиус, м";
            }
            else if (plotType == PlotType.Temperature && dimensionless)
            {
                return "Температура";
            }

            return "Температура, градусы Цельсия";
        }

        public List<SimplePoint> GetPlotPointsFromFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var points = new List<SimplePoint>();

            using (var package = new ExcelPackage(fileInfo))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();
                var data = worksheet.Tables.First().ConvertTableToObjects<SimplePoint>();
                foreach (var point in data)
                {
                    points.Add(point);
                }

                package.Save();
            }

            return points;
        }
    }
}