using DDrop.BL.Radiometric.COX.Models;

namespace DDrop.BL.Radiometric.COX.COXBL
{
    public interface ICOXBL
    {
        void SetImageInfo(ushort image_x, ushort image_y, IRF_IR_CAM_DATA_T cam_data, byte ir_image, IRF_IMAGE_INFO_T imageInfo, IRF_IR_CAM_DATA_T cpy_cam_data, int old_image_size);
    }
}