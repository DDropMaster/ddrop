using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DDrop.Db.DbEntities;

namespace DDrop.DAL
{
    public interface IDDropRepository
    {
        Task CreateUserAsync(DbUser user);
        Task CreatePlot(DbPlot plot);
        Task DeletePlot(DbPlot plot);
        Task UpdatePlot(DbPlot plotToUpdate);
        Task UpdateUserAsync(DbUser user);
        Task UpdatePlotName(string text, Guid plotId);
        Task<DbUser> GetUserByLogin(string email);
        Task<List<DbSeries>> GetSeriesByUserId(Guid dbUserId);
        Task<DbSeries> GetDbSeriesForExportById(Guid seriesId);
        Task CreateSeries(DbSeries series);
        Task UpdateRegionOfInterest(Guid seriesId, string regionOfInterest);
        Task UpdateSeriesSettings(Guid seriesId, string settings);
        Task UpdateSeriesName(string seriesName, Guid seriesId);
        Task DeleteSingleSeries(DbSeries series);
        Task CreateFullSeries(DbSeries series);
        Task<List<DbReferencePhoto>> GetReferencePhotoById(DbSeries series);
        Task<List<DbMeasurement>> GetMeasurements(DbSeries series);
        Task UpdateSeriesIntervalBetweenPhotos(int interval, Guid seriesId);
        Task CreateMeasurement(DbMeasurement measurement, Guid seriesId);
        Task<DbMeasurement> GetMeasurement(Guid measurementId);
        Task UpdateDrop(DbDrop drop);
        Task UpdateDropTemperature(double temperature, Guid dropId);
        Task DeleteMeasurement(DbMeasurement measurement);
        Task UpdateDropPhoto(DbDropPhoto dropPhoto, bool updateContent = false);
        Task CreateDropPhoto(DbDropPhoto dropPhoto, DbMeasurement measurement);
        Task UpdateDropPhotoName(string newName, Guid photoId);
        Task DeleteDropPhoto(DbDropPhoto dropPhoto);
        Task<List<DbDropPhoto>> GetDropPhotosByMeasurementId(Guid measurementId);
        Task UpdateMeasurementName(string newName, Guid measurementId);
        Task UpdateMeasurementAmbientTemperature(double temperature, Guid measurementId);
        Task<byte[]> GetPhotoContent(Guid photoId, CancellationToken cancellationToken);
        Task UpdateMeasurementsOrderInSeries(List<DbMeasurement> newDbMeasurements);
        Task DeleteReferencePhoto(Guid dbReferencePhotoId);
        Task UpdateReferencePhoto(DbReferencePhoto referencePhoto);
        void UpdateContour(DbContour contour);
        Task DeleteContour(Guid contourId);
        Task<DbContour> GetDbContour(Guid contourId);
        Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId);
        Task<byte[]> GetThermalPhotoContent(Guid photoId, CancellationToken cancellationToken);
        Task SaveLogEntry(DbLogEntry dbLogEntry);
        Task UpdateThermalPhoto(DbThermalPhoto dbPhoto, bool updateContent);
        Task CreateThermalPhoto(DbThermalPhoto thermalPhoto);
        Task DeleteThermalPhoto(DbThermalPhoto thermalPhoto);
        Task UpdateThermalPhotoName(string text, Guid editedPhotoId);
        Task UpdateThermalPhotoEllipseCoordinate(string temperatureCoordinate, Guid editedPhotoId);
        Task UpdateSubstance(DbSubstances substance);
        Task DeleteSubstance(Guid substanceId);
        Task DeleteComment(DbComment comment);
        Task UpdateComment(DbComment comment, Guid entityId);
    }
}