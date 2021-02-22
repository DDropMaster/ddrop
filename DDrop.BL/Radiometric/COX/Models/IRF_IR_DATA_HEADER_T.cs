using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_IR_DATA_HEADER_T
    {
        public byte dynamic_range;
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;
        public int total_frame;
        public IRF_SAVEDATA_T save_data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 460)]
        public byte[] reserved;
    }
}