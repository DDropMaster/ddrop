using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Db.DbEntities;
using DDrop.Utility.ImageOperations;

namespace DDrop.BL.Measurement
{
    public class DropPhotoBL : IDropPhotoBL
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public DropPhotoBL(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task<byte[]> GetDropPhotoContent(Guid photoId, CancellationToken cancellationToken, bool useCache)
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

                var content = await _dDropRepository.GetPhotoContent(photoId, cancellationToken);

                ImageInterpreter.ByteArrayToFile(content, photoId.ToString(), directoryName);

                return content;
            }

            return await _dDropRepository.GetPhotoContent(photoId, cancellationToken);
        }

        public async Task UpdateDropPhoto(DropPhoto dropPhoto, bool updateContent = false)
        {
            var dbPhoto = _mapper.Map<DropPhoto, DbDropPhoto>(dropPhoto);

            await Task.Run(() => _dDropRepository.UpdateDropPhoto(dbPhoto, updateContent));

            if (dropPhoto.Contour != null)
            {
                var contour = _mapper.Map<Contour, DbContour>(dropPhoto.Contour);
                _dDropRepository.UpdateContour(contour);
            }
            else
            {
                await _dDropRepository.DeleteContour(dropPhoto.PhotoId);
            }
        }

        public async Task DeleteDropPhoto(DropPhoto dropPhoto)
        {
            if (dropPhoto.Contour != null)
            {
                await _dDropRepository.DeleteContour(dropPhoto.ContourId);
            }

            await Task.Run(() => _dDropRepository.DeleteDropPhoto(_mapper.Map<DropPhoto, DbDropPhoto>(dropPhoto)));
        }

        public async Task UpdateDropPhotoName(string text, Guid editedPhotoId)
        {
            await Task.Run(() => _dDropRepository.UpdateDropPhotoName(text, editedPhotoId));
        }

        public async Task CreateDropPhoto(DropPhoto dropPhoto, BE.Models.Measurement owningMeasurement)
        {
            await Task.Run(() => _dDropRepository.CreateDropPhoto(_mapper.Map<DropPhoto, DbDropPhoto>(dropPhoto), _mapper.Map<BE.Models.Measurement, DbMeasurement>(owningMeasurement)));
        }
    }
}