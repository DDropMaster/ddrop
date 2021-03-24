using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using DDrop.Enums;
using DDrop.Models;
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

                mainWorksheet.Cells["D1:H1"].Merge = true;
                mainWorksheet.Cells["D2:H2"].Merge = true;
                mainWorksheet.Cells["D3:H3"].Merge = true;

                mainWorksheet.Cells["A1"].Value = "Имя:";
                mainWorksheet.Cells["A2"].Value = "Фамилия:";
                mainWorksheet.Cells["A3"].Value = "Email:";

                mainWorksheet.Cells["D1"].Value = user.FirstName;
                mainWorksheet.Cells["D2"].Value = user.LastName;
                mainWorksheet.Cells["D3"].Value = user.Email;

                var seriesCombinedChart =
                    mainWorksheet.Drawings.AddChart("seriesCombinedChart", eChartType.XYScatterLines) as
                        ExcelScatterChart;

                if (seriesCombinedChart != null)
                {
                    seriesCombinedChart.Title.Text = "Зависимость радиуса капли от времени испарения";
                    seriesCombinedChart.Legend.Position = eLegendPosition.Right;
                    seriesCombinedChart.XAxis.Title.Text = "Время, с";
                    seriesCombinedChart.YAxis.Title.Text = "Радиус, м";

                    var indexer = 0;

                    var exportAll = user.UserSeries.Where(x => x.IsChecked).ToList().Count == 0;

                    foreach (var currentSeries in user.UserSeries)
                    {
                        if (currentSeries.IsChecked || exportAll)
                        {
                            var worksheet = excelPackage.Workbook.Worksheets.Add($"{currentSeries.Title}");

                            worksheet.Cells["A1:C1"].Merge = true;
                            worksheet.Cells["A2:C2"].Merge = true;
                            worksheet.Cells["A3:C3"].Merge = true;
                            worksheet.Cells["A4:C4"].Merge = true;

                            worksheet.Cells["D1:G1"].Merge = true;
                            worksheet.Cells["D2:G2"].Merge = true;
                            worksheet.Cells["D3:G3"].Merge = true;
                            worksheet.Cells["D4:G4"].Merge = true;

                            worksheet.Cells["A1"].Value = "Название серии:";
                            worksheet.Cells["A2"].Value = "Очет от:";
                            worksheet.Cells["A3"].Value = "Интервал между снимками, c:";
                            worksheet.Cells["A4"].Value = "Пикселей в миллиметре (Спереди), px:";
                            worksheet.Cells["A5"].Value = "Пикселей в миллиметре (Сбоку), px:";

                            worksheet.Cells["D1"].Value = currentSeries.Title;
                            worksheet.Cells["D2"].Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                            worksheet.Cells["D3"].Value = currentSeries.IntervalBetweenPhotos;
                            worksheet.Cells["D4"].Value = currentSeries.ReferencePhotoForSeries?.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto)?.PixelsInMillimeter ?? 0;
                            worksheet.Cells["D5"].Value = currentSeries.ReferencePhotoForSeries?.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto)?.PixelsInMillimeter ?? 0;

                            var singleSeriesToExcelOutput = new ObservableCollection<SeriesToExcel>();

                            if (currentSeries.Settings.GeneralSeriesSettings.UseCreationDateTime)
                            {
                                var orderedMeasurements = currentSeries.MeasurementsSeries
                                    .OrderBy(x => DateTime.Parse(x.CreationDateTime, CultureInfo.InvariantCulture))
                                    .ToList();

                                for (var i = 0; i < currentSeries.MeasurementsSeries.Count; i++)
                                {
                                    var measurement = currentSeries.MeasurementsSeries[i];

                                    if (measurement.Drop.RadiusInMeters != null)
                                    {
                                        singleSeriesToExcelOutput.Add(new SeriesToExcel
                                        {
                                            Time = (DateTime.Parse(orderedMeasurements[i].CreationDateTime,
                                                        CultureInfo.InvariantCulture) -
                                                    DateTime.Parse(orderedMeasurements[0].CreationDateTime,
                                                        CultureInfo.InvariantCulture)).TotalSeconds,
                                            Name = measurement.Name,
                                            RadiusInMeters = measurement.Drop.RadiusInMeters.Value,
                                            VolumeInCubicalMeters = measurement.Drop.VolumeInCubicalMeters,
                                            XDiameterInMeters = measurement.Drop.XDiameterInMeters,
                                            YDiameterInMeters = measurement.Drop.YDiameterInMeters,
                                            ZDiameterInMeters = measurement.Drop.ZDiameterInMeters
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
                                        });
                                    }
                                }
                            }

                            worksheet.Cells["A6"].LoadFromCollection(singleSeriesToExcelOutput, true);

                            var end = worksheet.Dimension.End.Row;

                            var seriesChart =
                                worksheet.Drawings.AddChart("seriesChart", eChartType.XYScatterLines) as
                                    ExcelScatterChart;

                            if (seriesChart != null)
                            {
                                seriesChart.Title.Text =
                                    $"Зависимость радиуса капли от времени испарения для серии {currentSeries.Title}";
                                seriesChart.Legend.Position = eLegendPosition.Right;

                                seriesChart.Series.Add(worksheet.Cells[$"G7:G{end}"], worksheet.Cells[$"A7:A{end}"]);
                                seriesCombinedChart.Series.Add(worksheet.Cells[$"G7:G{end}"],
                                    worksheet.Cells[$"A7:A{end}"]);

                                seriesChart.XAxis.Title.Text = "Время, с";
                                seriesChart.YAxis.Title.Text = "Радиус, м";

                                seriesChart.Series[0].Header = worksheet.Cells["D1"].Value.ToString();
                                seriesCombinedChart.Series[indexer].Header = worksheet.Cells["D1"].Value.ToString();

                                seriesChart.SetSize(510, 660);
                                seriesChart.SetPosition(end + 1, 0, 0, 0);
                            }

                            worksheet.Cells.AutoFitColumns();
                            indexer++;
                        }
                    }
                    
                    seriesCombinedChart.SetSize(510, 660);
                    seriesCombinedChart.SetPosition(mainWorksheet.Dimension.End.Row + 1, 0, 0, 0);
                }

                mainWorksheet.Cells.AutoFitColumns();

                var excelFile = new FileInfo($@"{fileName}");
                excelPackage.SaveAs(excelFile);
            }
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