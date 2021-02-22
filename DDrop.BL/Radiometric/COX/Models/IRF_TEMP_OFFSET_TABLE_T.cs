using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_TEMP_OFFSET_TABLE_T
    {
        /// <summary>
        /// Minimum temperature in the normal temperature table
        /// </summary>
        public short normal_min;

        /// <summary>
        /// Maximum temperature in the normal temperature table
        /// </summary>
        public short normal_max;

        /// <summary>
        /// Minimum temperature in the high temperature table
        /// </summary>
        public short high_min;

        /// <summary>
        /// Maximum temperature in the high temperature table
        /// </summary>
        public short high_max;

        /// <summary>
        /// Normal temperature table ( normal_max - normal_min + 1 ) * 4bytes( temperature * 10 )
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public int normal_table;

        /// <summary>
        /// High temperature table ( ( high_max - high_min ) / 5 + 1 ) * 4bytes( temperature * 10 )
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public int high_table;
    }
}