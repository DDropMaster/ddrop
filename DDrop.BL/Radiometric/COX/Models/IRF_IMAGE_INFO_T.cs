using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_IMAGE_INFO_T
    {
        public ushort xSize;
        public ushort ySize;
    }
}