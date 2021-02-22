using System;
using System.Collections.Generic;
using System.Drawing;
using DDrop.BE.Enums;
using DDrop.BE.Models.Thermal;
using Flir.Atlas.Image;

namespace DDrop.BL.Radiometric.ThermalDataExtractor
{
    public class ThermalBL : IThermalBL
    {
        public FlirImage ProcessImage(string imageFilePath)
        {
            var thermalImage = new ThermalImageFile(imageFilePath);

            FlirImage flirImageData = new FlirImage
            {
                DateTaken = thermalImage.DateTaken,
                Description = thermalImage.Description,
                Height = thermalImage.Height,
                MaxSignalValue = thermalImage.MaxSignalValue,
                MinSignalValue = thermalImage.MinSignalValue,
                Precision = thermalImage.Precision,
                TemperatureUnit = (TempUnit)Enum.Parse(typeof(TempUnit), thermalImage.TemperatureUnit.ToString()),
                Width = thermalImage.Width,
                ThermalData = ExtractTemperatureReadings(thermalImage),
                Title = thermalImage.Title,
                Image = thermalImage.Image
            };

            if (thermalImage.CameraInformation != null)
                flirImageData.CameraInfo = new CameraInfo
                {
                    Filter = thermalImage.CameraInformation.Filter,
                    Fov = thermalImage.CameraInformation.Fov,
                    Lens = thermalImage.CameraInformation.Lens,
                    Model = thermalImage.CameraInformation.Model,
                    RangeMax = thermalImage.CameraInformation.Range.Maximum,
                    RangeMin = thermalImage.CameraInformation.Range.Minimum,
                    SerialNumber = thermalImage.CameraInformation.SerialNumber
                };

            if (thermalImage.CompassInformation != null)
                flirImageData.CompassInfo = new BE.Models.Thermal.CompassInfo
                {
                    Degrees = thermalImage.CompassInformation.Degrees,
                    Pitch = thermalImage.CompassInformation.Pitch,
                    Roll = thermalImage.CompassInformation.Roll
                };

            if (thermalImage.GpsInformation != null)
                flirImageData.GpsInfo = new GpsInfo
                {
                    Altitude = thermalImage.GpsInformation.Altitude,
                    Dop = thermalImage.GpsInformation.Dop,
                    Latitude = thermalImage.GpsInformation.Latitude,
                    Longitude = thermalImage.GpsInformation.Longitude,
                    MapDatum = thermalImage.GpsInformation.MapDatum,
                    Satellites = thermalImage.GpsInformation.Satellites
                };

            if (thermalImage.ThermalParameters != null)
                flirImageData.ThermalParameters = new ThermalParameters
                {
                    AtmosphericTemperature = thermalImage.ThermalParameters.AtmosphericTemperature,
                    Distance = thermalImage.ThermalParameters.Distance,
                    Emissivity = thermalImage.ThermalParameters.Emissivity,
                    ExternalOpticsTemperature = thermalImage.ThermalParameters.ExternalOpticsTemperature,
                    ExternalOpticsTransmission = thermalImage.ThermalParameters.ExternalOpticsTransmission,
                    ReferenceTemperature = thermalImage.ThermalParameters.ReferenceTemperature,
                    ReflectedTemperature = thermalImage.ThermalParameters.ReflectedTemperature,
                    RelativeHumidity = thermalImage.ThermalParameters.RelativeHumidity,
                    Transmission = thermalImage.ThermalParameters.Transmission
                };

            thermalImage.Close();

            return flirImageData;
        }

        private IList<ThermalData> ExtractTemperatureReadings(ThermalImageFile thermalImage)
        {
            IList<ThermalData> thermalData = new List<ThermalData>();

            double[,] rawThermalData = thermalImage.ImageProcessing.GetPixelsArray();

            for (int y = 0; y <= thermalImage.Size.Height - 1; y++)
            for (int x = 0; x <= thermalImage.Size.Width - 1; x++)
            {
                ThermalValue thermalValue = thermalImage.GetValueAt(new Point(x, y));

                thermalData.Add(new ThermalData
                {
                    X = x,
                    Y = y,
                    RawValue = rawThermalData[y, x],
                    TemperatureValue = thermalValue.Value,
                    TemperatureUnit = (TempUnit)Enum.Parse(typeof(TempUnit), thermalImage.TemperatureUnit.ToString())
                });
            }

            return thermalData;
        }
    }
}