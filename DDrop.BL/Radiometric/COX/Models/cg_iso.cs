using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct cg_iso
    {
        /// <summary>
        /// ISO Mode Mask
        /// </summary>
        public byte mode_mask;

        /// <summary>
        /// ISO Transparent Mask
        /// </summary>
        public byte tran_mask;

        /// <summary>
        /// Max Temperature
        /// </summary>
        public short max_temp;

        /// <summary>
        /// Min Temperature
        /// </summary>
        public short min_temp;

        /// <summary>
        /// Above Seed Color
        /// </summary>
        public uint above_color;

        /// <summary>
        /// Below Seed Color
        /// </summary>
        public uint below_color;

        /// <summary>
        /// Between Seed Color
        /// </summary>
        public uint between_color;
    }
}