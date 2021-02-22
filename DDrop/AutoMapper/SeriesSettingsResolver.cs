using AutoMapper;
using DDrop.BE.Models;
using DDrop.Db.DbEntities;
using DDrop.Utility.SeriesLocalStorageOperations;

namespace DDrop.AutoMapper
{
    public class SeriesSettingsResolver : IValueResolver<DbSeries, Series, SeriesSettings>
    {
        public SeriesSettings Resolve(DbSeries source, Series destination, SeriesSettings destMember, ResolutionContext context)
        {
            if (source.Settings != null)
            {
                var seriesSettings = JsonSerializeProvider.DeserializeFromString<SeriesSettings>(source.Settings);

                if (seriesSettings != null)
                {
                    return new SeriesSettings()
                    {
                        AutoCalculationSettings = GetAutoCalculationSettings(seriesSettings, destination)
                    };
                }
            }

            return new SeriesSettings()
            {
                AutoCalculationSettings = new AutoCalculationSettings()
            };
        }

        private AutoCalculationSettings GetAutoCalculationSettings(SeriesSettings seriesSettings, Series series)
        {
            if (seriesSettings.AutoCalculationSettings != null)
            {
                return new AutoCalculationSettings()
                {
                    CurrentSeries = series,
                    UseSideRoi = seriesSettings.AutoCalculationSettings.UseSideRoi,
                    UseThermalRoi = seriesSettings.AutoCalculationSettings.UseThermalRoi,
                    UseFrontRoi = seriesSettings.AutoCalculationSettings.UseFrontRoi,
                    TemperatureDetectingMode = seriesSettings.AutoCalculationSettings.TemperatureDetectingMode,
                    ThermalAutoCalculationSettings = seriesSettings.AutoCalculationSettings.ThermalAutoCalculationSettings,
                    UseRoi = seriesSettings.AutoCalculationSettings.UseRoi
                };
            }
            
            return new AutoCalculationSettings();
        }
    }
}