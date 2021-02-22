using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using DDrop.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace DDrop.Utility.ExcelOperations
{
    public class ExcelOperations
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
                            worksheet.Cells["A4"].Value = "Пикселей в миллиметре, px:";

                            worksheet.Cells["D1"].Value = currentSeries.Title;
                            worksheet.Cells["D2"].Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                            worksheet.Cells["D3"].Value = currentSeries.IntervalBetweenPhotos;
                            worksheet.Cells["D4"].Value =
                                currentSeries.ReferencePhotoForSeries?.PixelsInMillimeter ?? 0;

                            var singleSeriesToExcelOutput = new ObservableCollection<SeriesToExcel>();

                            if (currentSeries.UseCreationDateTime)
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
    }
}