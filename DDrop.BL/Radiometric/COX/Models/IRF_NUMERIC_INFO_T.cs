using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_NUMERIC_INFO_T
    {
        /// <summary>
        /// minimum temperature
        /// </summary>
        public float min;

        /// <summary>
        /// maximum temperature
        /// </summary>
        public float max;

        /// <summary>
        /// average temperature
        /// </summary>
        public float avg;

        /// <summary>
        /// standard deviation
        /// </summary>
        public float std;
    }
}