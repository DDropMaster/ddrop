using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.Db.DbEntities;

namespace DDrop.BL.Series
{
    public interface ISeriesBL
    {
        Task DeleteSeries(BE.Models.Series series);
        Task CreateSeries(BE.Models.Series seriesToAdd);
        Task<DbSeries> GetDbSeriesForExportById(Guid seriesId);
        Task CreateFullSeries(DbSeries dbSerieForAdd);
        Task UpdateSeriesName(string text, Guid seriesId);
        Task UpdateSeriesIntervalBetweenPhotos(double intervalBetweenPhotos, Guid seriesId);
        Task<List<BE.Models.Series>> GetSeriesByUserId(Guid userId);
        Task<List<BE.Models.Series>> GetSeriesByUserIdLight(Guid dbUserId);
        Task<BE.Models.Series> GetSingleSerie(Guid seriesId);
        Task UpdateSeriesRegionOfInterest(string regionOfInterest, Guid seriesId);
        Task UpdateSeriesSettings(string settings, Guid seriesId);
    }
}