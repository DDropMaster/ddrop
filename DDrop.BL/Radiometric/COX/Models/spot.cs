using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct spot
    {
        public byte enable;
        public ushort x;
        public ushort y;
        public byte local;
        public byte em;
        public byte tr;
        public short at;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] reserved;
    }
}