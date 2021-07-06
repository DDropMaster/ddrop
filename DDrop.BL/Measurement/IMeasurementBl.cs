using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDrop.BL.Measurement
{
    public interface IMeasurementBl
    {
        Task CreateMeasurement(BE.Models.Measurement deserializedMeasurement, Guid dbSerieForAddId);
        Task DeleteMeasurement(BE.Models.Measurement measurement);
        Task UpdateMeasurementName(string text, Guid currentMeasurementId);
        Task UpdateAmbientTemperature(double value, Guid currentMeasurementId);
        Task UpdateMeasurementsOrderInSeries(List<BE.Models.Measurement> measurementsSeries);
        Task<List<BE.Models.Measurement>> GetMeasurements(BE.Models.Series series);
        Task<BE.Models.Measurement> GetMeasurement(Guid measurementId);
    }
}