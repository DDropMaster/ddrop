using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.Measurement
{
    public class MeasurementBl : IMeasurementBl
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public MeasurementBl(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task CreateMeasurement(BE.Models.Measurement deserializedMeasurement, Guid dbSerieForAddId)
        {
            var dbMeasurement = _mapper.Map<BE.Models.Measurement, DbMeasurement>(deserializedMeasurement);

            await Task.Run(() => _dDropRepository.CreateMeasurement(dbMeasurement, dbSerieForAddId));
        }

        public async Task DeleteMeasurement(BE.Models.Measurement measurement)
        {
            await Task.Run(() => _dDropRepository.DeleteMeasurement(_mapper.Map<BE.Models.Measurement, DbMeasurement>(measurement)));
        }

        public async Task UpdateMeasurementName(string text, Guid currentMeasurementId)
        {
            await Task.Run(() => _dDropRepository.UpdateMeasurementName(text, currentMeasurementId));
        }

        public async Task UpdateAmbientTemperature(double value, Guid currentMeasurementId)
        {
            await Task.Run(() => _dDropRepository.UpdateMeasurementAmbientTemperature(value, currentMeasurementId));
        }

        public async Task UpdateMeasurementsOrderInSeries(ObservableCollection<BE.Models.Measurement> measurementsSeries)
        {
            await _dDropRepository.UpdateMeasurementsOrderInSeries(_mapper.Map<ObservableCollection<BE.Models.Measurement>, List<DbMeasurement>>(measurementsSeries));
        }

        public async Task<List<BE.Models.Measurement>> GetMeasurements(BE.Models.Series series)
        {
            var dbSeries = _mapper.Map<BE.Models.Series, DbSeries>(series);

            return _mapper.Map<List<DbMeasurement>, List<BE.Models.Measurement>>(
                await _dDropRepository.GetMeasurements(dbSeries));
        }
    }
}