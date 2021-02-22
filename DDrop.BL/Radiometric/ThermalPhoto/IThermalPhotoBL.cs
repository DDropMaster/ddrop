using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDrop.BL.Radiometric.ThermalPhoto
{
    public interface IThermalPhotoBL
    {
        Task UpdateThermalPhoto(BE.Models.ThermalPhoto dropPhoto, bool updateContent = false);
        Task UpdateThermalPhotoEllipseCoordinate(string temperatureCoordinate, Guid editedPhotoId);
        Task<byte[]> GetThermalPhotoContent(Guid photoId, CancellationToken cancellationToken, bool useCache);
        Task UpdateThermalPhotoName(string text, Guid editedPhotoId);
        Task DeleteThermalPhoto(BE.Models.ThermalPhoto thermalPhoto);
        Task CreateThermalPhoto(BE.Models.ThermalPhoto thermalPhoto);
    }
}