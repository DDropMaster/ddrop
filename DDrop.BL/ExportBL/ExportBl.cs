using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.DAL;
using DDrop.Db.DbEntities;
using DDrop.Utility.SeriesExporter;

namespace DDrop.BL.ExportBL
{
    public class ExportBl : IExportBl
    {
        private readonly IMapper _mapper;
        private readonly IDDropRepository _dDropRepository;

        public ExportBl(IMapper mapper, IDDropRepository dDropRepository)
        {
            _mapper = mapper;
            _dDropRepository = dDropRepository;
        }

        public async Task<DbSeries> ImportLocalSeriesAsync(Stream content, Guid userId)
        {
            return await Task.Run(() => SeriesExporter.ImportLocalSeriesAsync(content, userId));
        }

        public async Task ExportLocalSeriesAsync(Guid seriesId, string path)
        {
            var fullDbSeries = await _dDropRepository.GetDbSeriesForExportById(seriesId);

            var fileNames = new List<string>();

            foreach (var measurement in fullDbSeries.MeasurementsSeries)
            {
                measurement.DropPhotos = await _dDropRepository.GetDropPhotosByMeasurementId(measurement.MeasurementId, true);

                foreach (var dropPhoto in measurement.DropPhotos)
                {
                    if (dropPhoto.ContourId.HasValue)
                    {
                        dropPhoto.Contour = await _dDropRepository.GetDbContour(dropPhoto.ContourId.Value);
                    }
                }

                measurement.ThermalPhoto = await _dDropRepository.GetThermalPhotoByMeasurementId(measurement.MeasurementId, true);


                await Task.Run(() => SeriesExporter.ExportMeasurementAsync($"{path}\\{measurement.Name}.dmes", measurement));

                foreach (var dropPhoto in measurement.DropPhotos)
                {
                    dropPhoto.Content = null;
                    dropPhoto.Contour = null;
                    dropPhoto.PhotoId = Guid.Empty;
                }

                fileNames.Add($"{path}\\{measurement.Name}.dmes");
            }

            fullDbSeries.MeasurementsSeries = null;

            await Task.Run(() => SeriesExporter.ExportSeriesLocalAsync(
                $"{path}\\{fullDbSeries.Title}.dser", fullDbSeries));

            fileNames.Add($"{path}\\{fullDbSeries.Title}.dser");

            var zipFile = $"{path}\\{fullDbSeries.Title}.ddrops";

            using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
            {
                foreach (var fPath in fileNames)
                {
                    archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath), CompressionLevel.NoCompression);
                    File.Delete(fPath);
                }
            }
        }
    }
}