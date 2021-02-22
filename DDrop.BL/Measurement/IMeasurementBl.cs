using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DDrop.BL.Measurement
{
    public interface IMeasurementBl
    {
        Task CreateMeasurement(BE.Models.Measurement deserializedMeasurement, Guid dbSerieForAddId);
        Task DeleteMeasurement(BE.Models.Measurement measurement);
        Task UpdateMeasurementName(string text, Guid currentMeasurementId);
        Task UpdateAmbientTemperature(double value, Guid currentMeasurementId);
        Task UpdateMeasurementsOrderInSeries(ObservableCollection<BE.Models.Measurement> measurementsSeries);
    }
}