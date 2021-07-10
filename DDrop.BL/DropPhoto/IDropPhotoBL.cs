using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.Measurement
{
    public interface IDropPhotoBL
    {
        Task UpdateDropPhoto(DropPhoto dropPhoto, bool updateContent = false);
        Task<byte[]> GetDropPhotoContent(Guid photoId, CancellationToken cancellationToken, bool useCache);
        Task DeleteDropPhoto(DropPhoto dropPhoto);
        Task UpdateDropPhotoName(string text, Guid editedPhotoId);
        Task CreateDropPhoto(DropPhoto dropPhoto, BE.Models.Measurement owningMeasurement);
        Task<Contour> GetDropPhotoContour(Guid contourId);
        Task<List<DropPhoto>> GetDropPhotosByMeasurementId(Guid measurementId, bool withContent = false);
        Task<List<SimpleLine>> GetDropPhotoLines(Guid photoId);
        Task<DropPhoto> GetDropPhoto(Guid photoId);
    }
}