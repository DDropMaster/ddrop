using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDrop.BE.Models;

namespace DDrop.BL.ReferenceBL
{
    public interface IReferenceBl
    {
        Task<byte[]> GetReferencePhotoContent(Guid referencePhotoId);
        Task UpdateReferencePhoto(ReferencePhoto referencePhoto);
        Task DeleteReferencePhoto(Guid referencePhotoId);
        Task<List<ReferencePhoto>> GetReferencePhotoById(BE.Models.Series series);
        Task<SimpleLine> GetReferencePhotoLine(Guid photoId);
    }
}