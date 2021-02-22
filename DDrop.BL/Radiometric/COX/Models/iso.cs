using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct iso
    {
        public byte enable;
        public uint seed_color;
        public short top;
        public short bottom;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] byte[] reserved;
    }
}