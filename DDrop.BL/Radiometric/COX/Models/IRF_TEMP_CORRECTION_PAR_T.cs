using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_TEMP_CORRECTION_PAR_T
    {
        public float emissivity;
        public float atmTemp;
        public float atmTrans;
        public int zero_offset;
    }
}