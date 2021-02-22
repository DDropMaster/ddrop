using System.Runtime.InteropServices;
using DDrop.BL.Radiometric.COX.Enums;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_RECEIVED_TEMP_TABLE_T
    {
        /// <summary>
        /// Message type
        /// </summary>
        public IRF_MESSAGE_TYPE_T msg_type;

        /// <summary>
        /// Temperature table structure	
        /// </summary>
        public IRF_MIX_TEMP_TABLE_T sTemp_table;

        /// <summary>
        /// Primary Message Type Code
        /// </summary>
        public ushort PMSGTYPE;

        /// <summary>
        /// Response Code
        /// </summary>
        public ushort RCODE;
    }
}