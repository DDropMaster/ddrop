using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using DDrop.Enums;
using DDrop.Models;
using DDrop.Properties;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Table;

namespace DDrop.Utility.ExcelOperations
{
    public static class ExcelOperations
    {
        public static void CreateSingleSeriesExcelFile(UserView user, string fileName)
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

                mainWorksheet.Cells["D1"].Value = user.FirstName;
                mainWorksheet.Cells["D2"].Value = user.LastName;
                mainWorksheet.Cells["D3"].Value = user.Email;
                mainWorksheet.Cells["D4"].Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                mainWorksheet.Cells["D5"].Value = Settings.Default.DimensionlessPlots ? "Да" : "Нет";

                var seriesCombinedChart =
                    mainWorksheet.Drawings.AddChart("seriesCombinedChart", eChartType.XYScatterLines) as
                        ExcelScatterChart;

                if (seriesCombinedChart != null)
                {
                    seriesCombinedChart.Title.Text = "Зависимость радиуса капли от времени испарения";
                    seriesCombinedChart.Legend.Position = eLegendPosition.Right;
                    seriesCombinedChart.XAxis.Title.Text = Settings.Default.DimensionlessPlots ? "Время" : "Время, с";
                    seriesCombinedChart.YAxis.Title.Text = Settings.Default.DimensionlessPlots ? "Радиус" : "Радиус, м";

                    var indexer = 0;

                    var exportAll = user.UserSeries.Where(x => x.IsChecked).ToList().Count == 0;

                    foreach (var currentSeries in user.UserSeries)
                    {
                        if (currentSeries.IsChecked || exportAll)
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
                            worksheet.Cells["E3"].Value = currentSeries.ReferencePhotoForSeries?.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto)?.PixelsInMillimeter ?? 0;
                            worksheet.Cells["E4"].Value = currentSeries.ReferencePhotoForSeries?.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto)?.PixelsInMillimeter ?? 0;
                            worksheet.Cells["E5"].Value = currentSeries.Substance.CommonName;
                            worksheet.Cells["E6"].Value = currentSeries.Settings.GeneralSeriesSettings.IsAcoustic ? "Да" : "Нет";

                            var singleSeriesToExcelOutput = new ObservableCollection<SeriesToExcel>();

                            if (currentSeries.Settings.GeneralSeriesSettings.UseCreationDateTime)
                            {
                                var orderedMeasurements = currentSeries.MeasurementsSeries
                                    .OrderBy(x => x.CreationDateTime)
                                    .ToList();

                                for (var i = 0; i < currentSeries.MeasurementsSeries.Count; i++)
                                {
                                    var measurement = currentSeries.MeasurementsSeries[i];

                                    if (measurement.Drop.RadiusInMeters != null)
                                    {
                                        singleSeriesToExcelOutput.Add(new SeriesToExcel
                                        {
                                            Time = (orderedMeasurements[i].CreationDateTime - orderedMeasurements[0].CreationDateTime).TotalSeconds,
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
                            }
                            else
                            {
                                for (var i = 0; i < currentSeries.MeasurementsSeries.Count; i++)
                                {
                                    var measurement = currentSeries.MeasurementsSeries[i];
                                    if (measurement.Drop.RadiusInMeters != null)
                                    {
                                        singleSeriesToExcelOutput.Add(new SeriesToExcel
                                        {
                                            Time = i * currentSeries.IntervalBetweenPhotos,
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
                            }

                            worksheet.Cells["A8:H8"].Merge = true;
                            worksheet.Cells["A8"].Value = "Данные";
                            worksheet.Cells["A8"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A8"].Style.Font.Bold = true;

                            worksheet.Cells["A9"].LoadFromCollection(singleSeriesToExcelOutput, true);
                            worksheet.Cells["A9:H9"].Style.Font.Bold = true;

                            var end = worksheet.Dimension.End.Row;

                            var seriesChart = worksheet.Drawings.AddChart("seriesChart", eChartType.XYScatterLines) as ExcelScatterChart;

                            if (seriesChart != null)
                            {
                                seriesChart.Title.Text = $"Зависимость радиуса капли от времени испарения для серии {currentSeries.Title}";
                                seriesChart.Legend.Position = eLegendPosition.Right;

                                seriesChart.Series.Add(worksheet.Cells[$"F10:F{end}"], worksheet.Cells[$"A10:A{end}"]);
                                seriesCombinedChart.Series.Add(worksheet.Cells[$"F10:F{end}"], worksheet.Cells[$"A10:A{end}"]);

                                seriesChart.XAxis.Title.Text = Settings.Default.DimensionlessPlots ? "Время" : "Время, с";
                                seriesChart.YAxis.Title.Text = Settings.Default.DimensionlessPlots ? "Радиус" : "Радиус, м";

                                seriesChart.Series[0].Header = worksheet.Cells["E1"].Value.ToString();
                                seriesCombinedChart.Series[indexer].Header = worksheet.Cells["E1"].Value.ToString();

                                seriesChart.SetSize(510, 660);
                                seriesChart.SetPosition(end + 1, 0, 0, 0);
                            }

                            worksheet.Cells.AutoFitColumns();
                            indexer++;
                        }

                        if (currentSeries.Settings.GeneralSeriesSettings.UseThermalPlot && currentSeries.ThermalPlot != null)
                        {
                            
                        }
                    }

                    foreach (var plot in user.Plots)
                    {
                        var plotWorkSheet = excelPackage.Workbook.Worksheets.Add($"График: {plot.Name}");

                        plotWorkSheet.Cells["A1:C1"].Merge = true;
                        plotWorkSheet.Cells["D1:G1"].Merge = true;

                        plotWorkSheet.Cells["A1"].Value = "Название графика:";
                        plotWorkSheet.Cells["D1"].Value = plot.Name;

                        var plotToExcelOutput = new ObservableCollection<PlotToExcel>();

                        foreach (var point in plot.Points)
                        {
                            plotToExcelOutput.Add(new PlotToExcel
                            {
                                X = point.X,
                                Y = point.Y
                            });
                        }

                        plotWorkSheet.Cells["A3"].LoadFromCollection(plotToExcelOutput, true);

                        if (plotToExcelOutput.Count > 0)
                        {
                            var end = plotWorkSheet.Dimension.End.Row;

                            var manualPlotChart = plotWorkSheet.Drawings.AddChart("manualPlotChart", eChartType.XYScatterLines) as ExcelScatterChart;

                            if (manualPlotChart != null)
                            {
                                manualPlotChart.Title.Text = plot.PlotType == PlotTypeView.Temperature
                                    ? $"Зависимость температуры капли от времени испарения для графика {plot.Name}"
                                    : $"Зависимость радиуса капли от времени испарения для графика {plot.Name}";
                                manualPlotChart.Legend.Position = eLegendPosition.Right;

                                manualPlotChart.Series.Add(plotWorkSheet.Cells[$"B4:B{end}"], plotWorkSheet.Cells[$"A4:A{end}"]);

                                manualPlotChart.XAxis.Title.Text = Settings.Default.DimensionlessPlots ? "Время" : "Время, с";
                                manualPlotChart.YAxis.Title.Text = GetPlotYAxisTitle(plot.PlotType, Settings.Default.DimensionlessPlots);

                                manualPlotChart.Series[0].Header = plotWorkSheet.Cells["D1"].Value.ToString();

                                manualPlotChart.SetSize(510, 660);
                                manualPlotChart.SetPosition(end + 1, 0, 0, 0);
                            }
                        }

                        plotWorkSheet.Cells.AutoFitColumns();
                    }

                    seriesCombinedChart.SetSize(510, 660);
                    seriesCombinedChart.SetPosition(mainWorksheet.Dimension.End.Row + 1, 0, 0, 0);
                }
                
                mainWorksheet.Cells.AutoFitColumns();                

                var excelFile = new FileInfo($@"{fileName}");
                excelPackage.SaveAs(excelFile);
            }
        }

        private static string GetPlotYAxisTitle(PlotTypeView plotType, bool dimensionless)
        {
            if (plotType == PlotTypeView.Radius && dimensionless)
            {
                return "Радиус";
            }
            else if (plotType == PlotTypeView.Radius && !dimensionless)
            {
                return "Радиус, м";
            }
            else if (plotType == PlotTypeView.Temperature && dimensionless)
            {
                return "Температура";
            }

            return "Температура, градусы Цельсия";
        }

        public static ObservableCollection<SimplePointView> GetPlotPointsFromFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var points = new ObservableCollection<SimplePointView>();

            using (var package = new ExcelPackage(fileInfo))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();
                var data = worksheet.Tables.First().ConvertTableToObjects<SimplePointView>();
                foreach (var point in data)
                {
                    points.Add(point);
                }

                package.Save();
            }

            return points;
        }

        private static IEnumerable<T> ConvertTableToObjects<T>(this ExcelTable table) where T : new()
        {
            //DateTime Conversion
            var convertDateTime = new Func<double, DateTime>(excelDate =>
            {
                if (excelDate < 1)
                    throw new ArgumentException("Excel dates cannot be smaller than 0.");

                var dateOfReference = new DateTime(1900, 1, 1);

                if (excelDate > 60d)
                    excelDate = excelDate - 2;
                else
                    excelDate = excelDate - 1;
                return dateOfReference.AddDays(excelDate);
            });

            //Get the properties of T
            var tprops = (new T())
                .GetType()
                .GetProperties()
                .ToList();

            //Get the cells based on the table address
            var start = table.Address.Start;
            var end = table.Address.End;
            var cells = new List<ExcelRangeBase>();

            //Have to use for loops insteadof worksheet.Cells to protect against empties
            for (var r = start.Row; r <= end.Row; r++)
                for (var c = start.Column; c <= end.Column; c++)
                    cells.Add(table.WorkSheet.Cells[r, c]);

            var groups = cells
                .GroupBy(cell => cell.Start.Row)
                .ToList();

            //Assume the second row represents column data types (big assumption!)
            var types = groups
                .Skip(1)
                .First()
                .Select(rcell => rcell.Value.GetType())
                .ToList();

            //Assume first row has the column names
            var colnames = groups
                .First()
                .Select((hcell, idx) => new { Name = hcell.Value.ToString(), index = idx })
                .Where(o => tprops.Select(p => p.Name).Contains(o.Name))
                .ToList();

            //Everything after the header is data
            var rowvalues = groups
                .Skip(1) //Exclude header
                .Select(cg => cg.Select(c => c.Value).ToList());

            //Create the collection container
            var collection = rowvalues
                .Select(row =>
                {
                    var tnew = new T();
                    colnames.ForEach(colname =>
                    {
                        //This is the real wrinkle to using reflection - Excel stores all numbers as double including int
                        var val = row[colname.index];
                        var type = types[colname.index];
                        var prop = tprops.First(p => p.Name == colname.Name);

                        //If it is numeric it is a double since that is how excel stores all numbers
                        if (type == typeof(double))
                        {
                            if (!string.IsNullOrWhiteSpace(val?.ToString()))
                            {
                                //Unbox it
                                var unboxedVal = (double)val;

                                //FAR FROM A COMPLETE LIST!!!
                                if (prop.PropertyType == typeof(Int32))
                                    prop.SetValue(tnew, (int)unboxedVal);
                                else if (prop.PropertyType == typeof(double))
                                    prop.SetValue(tnew, unboxedVal);
                                else if (prop.PropertyType == typeof(DateTime))
                                    prop.SetValue(tnew, convertDateTime(unboxedVal));
                                else
                                    throw new NotImplementedException(String.Format("Type '{0}' not implemented yet!", prop.PropertyType.Name));
                            }
                        }
                        else
                        {
                            //Its a string
                            prop.SetValue(tnew, val);
                        }
                    });

                    return tnew;
                });


            //Send it back
            return collection;
        }
    }
}