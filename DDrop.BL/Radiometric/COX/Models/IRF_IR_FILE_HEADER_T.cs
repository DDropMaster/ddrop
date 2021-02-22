using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_IR_FILE_HEADER_T
    {
        public byte ver;
        public IRF_IMAGE_INFO_T image_info;
        public IRF_TEMP_CORRECTION_PAR_T temp_correction;
    }
}