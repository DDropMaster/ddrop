using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct roi
    {
        /// <summary>
        /// ROI Function Mask ( 0x01 : Enable, 0x02 : Exclude )
        /// </summary>
        public byte flag;

        /// <summary>
        /// Position (x2)
        /// </summary>
        public ushort x1;

        /// <summary>
        /// Position (x2)
        /// </summary>
        public ushort y1;

        /// <summary>
        /// Position (x2)
        /// </summary>
        public ushort x2;

        /// <summary>
        /// Position (x2)
        /// </summary>
        public ushort y2;
    }
}