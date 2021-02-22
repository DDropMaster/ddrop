using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct cg_roi
    {
        /// <summary>
        /// ROI Mode
        /// </summary>
        public byte mode;

        /// <summary>
        /// Temperature Type ( Select MAX, MIN, AVG )
        /// </summary>
        public byte temp_type;

        /// <summary>
        /// x position
        /// </summary>
        public ushort x;

        /// <summary>
        /// y position
        /// </summary>
        public ushort y;

        /// <summary>
        /// ROI Width
        /// </summary>
        public ushort w;

        /// <summary>
        /// ROI Height
        /// </summary>
        public ushort h;

        /// <summary>
        /// Enable Correction
        /// </summary>
        public byte f_corr;

        /// <summary>
        /// Emissivity
        /// </summary>
        public byte em;

        /// <summary>
        /// Transmission
        /// </summary>
        public byte tr;

        /// <summary>
        /// Atmosphere		
        /// </summary>
        public short at;
        
        public byte reserved;
    }
}