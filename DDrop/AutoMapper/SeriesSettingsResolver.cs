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
                        AutoCalculationSettings = GetAutoCalculationSettings(seriesSettings),
                        GeneralSeriesSettings = GetGeneralSeriesSettings(seriesSettings),
                        ErrorSettings = GetErrorSettings(seriesSettings)
                    };
                }
            }

            return new SeriesSettings()
            {
                AutoCalculationSettings = new AutoCalculationSettings(),
                GeneralSeriesSettings = new GeneralSeriesSettings(),
                ErrorSettings = new ErrorSettings(),
            };
        }

        private AutoCalculationSettings GetAutoCalculationSettings(SeriesSettings seriesSettings)
        {
            if (seriesSettings.AutoCalculationSettings != null)
            {
                return new AutoCalculationSettings()
                {
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

        private GeneralSeriesSettings GetGeneralSeriesSettings(SeriesSettings seriesSettings)
        {
            if (seriesSettings.GeneralSeriesSettings != null)
            {
                return new GeneralSeriesSettings()
                {
                    UseCreationDateTime = seriesSettings.GeneralSeriesSettings.UseCreationDateTime,
                    UseThermalPlot = seriesSettings.GeneralSeriesSettings.UseThermalPlot,
                    IsAcoustic = seriesSettings.GeneralSeriesSettings.IsAcoustic
                };
            }

            return new GeneralSeriesSettings();
        }

        private ErrorSettings GetErrorSettings(SeriesSettings seriesSettings)
        {
            if (seriesSettings.ErrorSettings != null)
            {
                return new ErrorSettings()
                {
                    FullRadiusError = seriesSettings.ErrorSettings.FullRadiusError,
                    FullTemperatureError = seriesSettings.ErrorSettings.FullTemperatureError
                };
            }

            return new ErrorSettings();
        }
    }
}