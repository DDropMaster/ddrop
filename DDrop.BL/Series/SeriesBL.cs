using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using AutoMapper;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.Series
{
    public class SeriesBL : ISeriesBL
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public SeriesBL(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task DeleteSeries(BE.Models.Series series, BE.Models.Series currentSeries)
        {
            var userSeries = series;

            var dbSeriesToDelete = _mapper.Map<BE.Models.Series, DbSeries>(userSeries);

            await _dDropRepository.DeleteSingleSeries(dbSeriesToDelete);
        }

        public async Task CreateSeries(BE.Models.Series seriesToAdd)
        {
            await Task.Run(() => _dDropRepository.CreateSeries(_mapper.Map<BE.Models.Series, DbSeries>(seriesToAdd)));
        }

        public async Task<DbSeries> GetDbSeriesForExportById(Guid seriesId)
        {
            return await Task.Run(() => _dDropRepository.GetDbSeriesForExportById(seriesId));
        }

        public async Task CreateFullSeries(DbSeries dbSerieForAdd)
        {
            await Task.Run(() => _dDropRepository.CreateFullSeries(dbSerieForAdd));
        }

        public async Task UpdateSeriesName(string text, Guid seriesId)
        {
            await Task.Run(() => _dDropRepository.UpdateSeriesName(text, seriesId));
        }

        public async Task UpdateSeriesIntervalBetweenPhotos(int intervalBetweenPhotos, Guid seriesId)
        {
            await Task.Run(() => _dDropRepository.UpdateSeriesIntervalBetweenPhotos(intervalBetweenPhotos, seriesId));
        }

        public async Task<List<BE.Models.Series>> GetSeriesByUserId(Guid userId)
        {
            var dbSeries = await Task.Run(() => _dDropRepository.GetSeriesByUserId(userId));

            return _mapper.Map<List<DbSeries>, List<BE.Models.Series>>(dbSeries);
        }

        public async Task UpdateSeriesRegionOfInterest(string regionOfInterest, Guid seriesId)
        {
            await Task.Run(() => _dDropRepository.UpdateRegionOfInterest(seriesId, regionOfInterest));
        }

        public async Task UpdateSeriesSettings(string settings, Guid seriesId)
        {
            await Task.Run(() => _dDropRepository.UpdateSeriesSettings(seriesId, settings));
        }
    }
}