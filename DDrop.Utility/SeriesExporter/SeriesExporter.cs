using System;
using System.IO;
using System.Threading.Tasks;
using DDrop.Db.DbEntities;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.Utility.SeriesExporter
{
    public static class SeriesExporter
    {
        public static async Task<DbSeries> ImportLocalSeriesAsync(Stream content, DbUser user)
        {
            var series = await Task.Run(() => JsonSerializeProvider.DeserializeFromStream<DbSeries>(content));
            series.AddedDate = DateTime.Now;
            series.SeriesId = Guid.NewGuid();
            series.CurrentUser = user;
            series.CurrentUserId = user.UserId;
            
            if (series.ReferencePhotoForSeries != null)
            {
                foreach (var referencePhoto in series.ReferencePhotoForSeries)
                {
                    referencePhoto.PhotoId = Guid.NewGuid();
                    referencePhoto.CurrentSeriesId = series.SeriesId;
                }
            }

            return series;
        }

        public static async Task<DbMeasurement> ImportLocalMeasurementAsync(Stream content, DbSeries series)
        {
            var measurement = await Task.Run(() => JsonSerializeProvider.DeserializeFromStream<DbMeasurement>(content));

            measurement.MeasurementId = Guid.NewGuid();
            measurement.AddedDate = DateTime.Now;

            if (measurement.FrontDropPhoto != null)
            {
                var frontDropPhotoId = Guid.NewGuid();
                measurement.FrontDropPhoto.PhotoId = frontDropPhotoId;
                measurement.FrontDropPhotoId = frontDropPhotoId;

                if (measurement.FrontDropPhoto.Contour != null)
                {
                    measurement.FrontDropPhoto.Contour.ContourId = frontDropPhotoId;
                }
            }

            if (measurement.SideDropPhoto != null)
            {
                var sideDropPhotoId = Guid.NewGuid();
                measurement.SideDropPhoto.PhotoId = sideDropPhotoId;
                measurement.SideDropPhotoId = sideDropPhotoId;

                if (measurement.SideDropPhoto.Contour != null)
                {
                    measurement.SideDropPhoto.Contour.ContourId = sideDropPhotoId;
                }
            }

            measurement.Drop.DropId = measurement.MeasurementId;
            measurement.CurrentSeriesId = series.SeriesId;
            measurement.CurrentSeries = series;

            return measurement;
        }

        public static async Task ExportMeasurementAsync(string fileName, DbMeasurement dbMeasurement)
        {
            await Task.Run(() => JsonSerializeProvider.SerializeToFileAsync(dbMeasurement, fileName));
        }

        public static async Task ExportSeriesLocalAsync(string fileName, DbSeries dbSeries)
        {
            await Task.Run(() => JsonSerializeProvider.SerializeToFileAsync(dbSeries, fileName));
        }
    }
}