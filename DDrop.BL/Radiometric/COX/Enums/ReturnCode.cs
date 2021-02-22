namespace DDrop.BL.Radiometric.COX.Enums
{
    public enum ReturnCode
    {
        /// <summary>
        /// OK, No error
        /// </summary>
        IRF_NO_ERROR = 1,

        /// <summary>
        /// Invalid Handle
        /// </summary>
        IRF_HANDLE_ERROR = -1,

        /// <summary>
        /// File Open Error.
        /// </summary>
        IRF_FILE_OPEN_ERROR = -2,
        
        /// <summary>
        /// File Close Error.
        /// </summary>
        IRF_FILE_CLOSE_ERROR = -3,

        /// <summary>
        /// CRD File Read Error
        /// </summary>
        IRF_IR_IMAGE_READ_ERROR = -4,

        /// <summary>
        /// File Stream Buffer Allocation Error.
        /// </summary>
        IRF_FILE_BUFFER_ALLOCATION_ERROR = -5,

        /// <summary>
        /// End of CRD File.
        /// </summary>
        IRF_END_OF_FILE = -6,

        /// <summary>
        /// Begin of CRD File.
        /// </summary>
        IRF_BEGIN_OF_FILE =	-7,

        /// <summary>
        /// CRD File Write Error.
        /// </summary>
        IRF_IR_IMAGE_WRITE_ERROR = -8,

        /// <summary>
        /// Not Found WS2_32.dll
        /// </summary>
        IRF_NOT_FOUND_WINSOCK_DLL =	-9,

        /// <summary>
        ///	Connection error from a camera.
        /// </summary>
        IRF_CAMERA_CONNECTION_ERROR = -10,

        /// <summary>
        /// Disconnected from a camera
        /// </summary>
        IRF_CAMERA_DISCONNECTION = -11,

        /// <summary>
        /// Unknown network packet ID
        /// </summary>
        IRF_PACKET_ID_ERROR = -12,

        /// <summary>
        /// Message sending error
        /// </summary>
        IRF_MESSAGE_SEND_ERROR = -13,

        /// <summary>
        /// First frame position error.
        /// </summary>
        IRF_FIRST_FRAME_POS_ERROR =	-14,

        /// <summary>
        /// Image filter size error.
        /// </summary>
        IRF_FILTER_SIZE_ERROR =	-15,

        /// <summary>
        /// Image frame count is bigger than limit.
        /// </summary>
        IRF_FILE_WRITE_COUNT_OVER =	-16,

        /// <summary>
        /// Palette File open error.
        /// </summary>
        IRF_PALETTE_FILE_OPEN_ERROR = -17,

        /// <summary>
        /// CRD file signature error.
        /// </summary>
        IRF_CRD_SINATURE_ERROR = -18,

        /// <summary>
        /// Unsupported Filter type.
        /// </summary>
        IRF_FILTER_UNSUPPORT_ERROR = -19,
    }
}