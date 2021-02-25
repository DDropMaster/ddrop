using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.DAL;
using DDrop.Db.DbEntities;
using DDrop.Utility.ImageOperations;

namespace DDrop.BL.Radiometric.ThermalPhoto
{
    public class ThermalPhotoBL : IThermalPhotoBL
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public ThermalPhotoBL(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task<byte[]> GetThermalPhotoContent(Guid photoId, CancellationToken cancellationToken, bool useCache)
        {
            if (useCache)
            {
                string directoryName = "Cache";
                string path = Path.Combine(Environment.CurrentDirectory, directoryName);
                var fullFilePath = $@"{path}\{photoId}";

                if (File.Exists(fullFilePath))
                {
                    var cachedContent = ImageInterpreter.FileToByteArray(fullFilePath);

                    if (cachedContent != null)
                    {
                        return cachedContent;
                    }
                }

                var content = await _dDropRepository.GetThermalPhotoContent(photoId, cancellationToken);

                ImageInterpreter.ByteArrayToFile(content, photoId.ToString(), directoryName);

                return content;
            }

            return await _dDropRepository.GetThermalPhotoContent(photoId, cancellationToken);
        }

        public async Task UpdateThermalPhoto(BE.Models.ThermalPhoto dropPhoto, bool updateContent = false)
        {
            var dbPhoto = _mapper.Map<BE.Models.ThermalPhoto, DbThermalPhoto>(dropPhoto);

            await Task.Run(() => _dDropRepository.UpdateThermalPhoto(dbPhoto, updateContent));
        }

        public async Task UpdateThermalPhotoEllipseCoordinate(string temperatureCoordinate, Guid editedPhotoId)
        {
            await Task.Run(() => _dDropRepository.UpdateThermalPhotoEllipseCoordinate(temperatureCoordinate, editedPhotoId));
        }

        public async Task UpdateThermalPhotoName(string text, Guid editedPhotoId)
        {
            await Task.Run(() => _dDropRepository.UpdateThermalPhotoName(text, editedPhotoId));
        }

        public async Task DeleteThermalPhoto(BE.Models.ThermalPhoto thermalPhoto)
        {
            if (thermalPhoto.Contour != null && thermalPhoto.ContourId != null)
            {
                await _dDropRepository.DeleteContour(thermalPhoto.ContourId.Value);
            }

            await Task.Run(() => _dDropRepository.DeleteThermalPhoto(_mapper.Map<BE.Models.ThermalPhoto, DbThermalPhoto>(thermalPhoto)));
        }

        public async Task CreateThermalPhoto(BE.Models.ThermalPhoto thermalPhoto)
        {
            await Task.Run(() => _dDropRepository.CreateThermalPhoto(_mapper.Map<BE.Models.ThermalPhoto, DbThermalPhoto>(thermalPhoto)));
        }
    }
}