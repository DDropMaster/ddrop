using System;
using System.Runtime.InteropServices;
using DDrop.BL.Radiometric.COX.Enums;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_IR_CAM_DATA_T
    {
        /// <summary>
        /// 16bits raw image data * 10
        /// </summary>
        public ushort ir_image;

        /// <summary>
        /// Raw image size.
        /// </summary>
        public uint image_buffer_size;

        /// <summary>
        /// This variable is remainder data after make a raw image from communication buffer, it used for make next raw image.
        /// </summary>
        public IntPtr lpNextData;

        /// <summary>
        /// This variable is size of remainder data.
        /// </summary>
        public ushort dwSize;

        /// <summary>
        /// This variable is current position in the remainder data.
        /// </summary>
        public ushort dwPosition;

        /// <summary>
        /// cam data in CAM_DATA
        /// </summary>
        public IRF_MESSAGE_TYPE_T msg_type;
        public IRF_SAVEDATA_T save_data;

        /// <summary>
        /// Firmware version in SYS_INFO
        /// </summary>
        public uint fw_ver;

        #region csi 20170728 add core info

        /// <summary>
        /// core type ( 0 : QVGA, 1 : VGA )
        /// </summary>
        public uint core_type;

        /// <summary>
        /// horizontal resolution
        /// </summary>
        public ushort h_res;

        /// <summary>
        /// vertical resolution
        /// </summary>
        public ushort v_res;

        #endregion

        /// <summary>
        /// Primary Message Type Code
        /// </summary>
        public ushort PMSGTYPE;

        /// <summary>
        /// Response Code
        /// </summary>
        public ushort RCODE;

        /// <summary>
        /// Reserved ( Write may cause problem )
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public uint[] reserved;
	}
}