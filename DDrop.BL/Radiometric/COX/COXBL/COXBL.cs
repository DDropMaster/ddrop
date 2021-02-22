using DDrop.BL.Radiometric.COX.Models;

namespace DDrop.BL.Radiometric.COX.COXBL
{
    public class COXBL : ICOXBL
    {
        private const int IRF_MAX_IR_BUFFER_SIZE = 1;
        private const int SAVEDATA_VER_20 = 0x20;

        public void SetImageInfo(ushort image_x, ushort image_y, IRF_IR_CAM_DATA_T cam_data, byte ir_image, IRF_IMAGE_INFO_T imageInfo, IRF_IR_CAM_DATA_T cpy_cam_data, int old_image_size)
        {
            // update struct_ir_image buffer.
            if (cam_data.image_buffer_size != image_x * image_y)
            {
                cam_data.ir_image = new ushort();

                if (SAVEDATA_VER_20 < cam_data.save_data.ver)
                {
                    cam_data.ir_image = (ushort)(image_x * (image_y + 1) * IRF_MAX_IR_BUFFER_SIZE);
                    cam_data.image_buffer_size = (uint)(image_x * (image_y + 1));
                }
                else
                {
                    cam_data.ir_image = (ushort)(image_x * image_y * IRF_MAX_IR_BUFFER_SIZE);
                    cam_data.image_buffer_size = (uint)(image_x * image_y);
                }

                byte m_image = (byte)(image_x * image_y);
            }


            // update ir_image buffer
            if (old_image_size != image_x * image_y)
            {
                old_image_size = image_x * image_y;
            }

            imageInfo.xSize = image_x;
            imageInfo.ySize = image_y;


            if (SAVEDATA_VER_20 < cam_data.save_data.ver)
            {
                ir_image = (byte) (imageInfo.xSize * (imageInfo.ySize + 1)); // (320 * 240 * 2 bytes)/frame.
            }
            else
            {
                ir_image = (byte) (imageInfo.xSize * imageInfo.ySize);   // (320 * 240 * 2 bytes)/frame.
            }

            //	bug fixed... 20150128
            cpy_cam_data.ir_image = ir_image;
        }
    }
}