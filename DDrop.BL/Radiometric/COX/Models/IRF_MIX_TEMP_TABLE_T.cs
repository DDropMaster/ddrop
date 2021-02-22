using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_MIX_TEMP_TABLE_T
    {
        public IRF_TEMP_TABLE_T temp_table;                // Temperature table structure
        public IRF_TEMP_OFFSET_TABLE_T temp_offset_table;		// Temperature offset table structure
    }
}