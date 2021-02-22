using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_SET_USER_PALETTE
    {
        /// <summary>
        /// Reserved
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public byte[] Info;

        /// <summary>
        /// Data Index(0:userPalette1, 1:userPalette2)
        /// </summary>
        public byte Index;

        /// <summary>
        /// RGBA (4bytes * 256 level)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] byte[] Data;

        /// <summary>
        /// PNG File length
        /// </summary>
        uint pngLength;

        /// <summary>
        /// PNG File data;
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8192)]
        byte[] pngData;
    }
}