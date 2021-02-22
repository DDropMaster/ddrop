using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ALRMCFG
    {
        /// <summary>
        /// Alarm Enable
        /// </summary>
        public byte f_enable;

        /// <summary>
        /// Alarm Output Mask
        /// </summary>
        public byte output_mask;

        /// <summary>
        /// Alarm Condition
        /// </summary>
        public byte cond;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte reserved;

        /// <summary>
        /// Alarm Temperature
        /// </summary>
        public short temp;

        /// <summary>
        /// Alarm Delay
        /// </summary>
        public ushort delay;
    }
}