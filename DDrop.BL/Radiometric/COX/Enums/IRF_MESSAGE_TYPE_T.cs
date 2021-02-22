namespace DDrop.BL.Radiometric.COX.Enums
{
    public enum IRF_MESSAGE_TYPE_T
    {
        /// <summary>
        /// No Received Packet
        /// </summary>
		_IRF_NONE = -1,

        /// <summary>
        /// Receive Acknowledgement as a result of request
        /// </summary>
        _IRF_ACK = 0,

        /// <summary>
        /// Receive Negative Acknowledgement as a result of request
        /// </summary>
        _IRF_NAK = 1,

        /// <summary>
        /// Send Alive Message
        /// </summary>
        _IRF_ALIVE = 2,

        /// <summary>
        /// Request to start raw data transfer.
        /// </summary>
        _IRF_STREAM_ON = 3,

        /// <summary>
        /// Request to stop raw data transfer.
        /// </summary>
        _IRF_STREAM_OFF = 4,

        /// <summary>
        /// Receive raw data
        /// </summary>
        _IRF_STREAM_DATA = 5,

        /// <summary>
        /// Request a camera configuration data.
        /// </summary>
        _IRF_REQ_CAM_DATA = 7,

        /// <summary>
        /// Receive a camera configuration data as a result of request.
        /// </summary>
        _IRF_CAM_DATA = 8,

        /// <summary>
        /// Request camera to save one of various setting CMD_xxxx.
        /// </summary>
        _IRF_SET_CAM_DATA = 10,

        /// <summary>
        /// User color palette update. (pc --> cam)
        /// </summary>
        _IRF_SET_USER_PALETTE = 11,

        /// <summary>
        /// Request System Information. (pc --> cam)
        /// </summary>
        _IRF_REQ_SYS_INFO = 12,

        /// <summary>
        /// Receive System Information as a result of request.	(cam --> pc)
        /// </summary>
        _IRF_SYS_INFO = 13,

        /// <summary>
        /// Request to start spot streaming transfer.
        /// </summary>
        _IRF_SPOT_STREAM_ON = 14,

        /// <summary>
        /// Request to stop spot streaming transfer.
        /// </summary>
        _IRF_SPOT_STREAM_OFF = 15,

        /// <summary>
        /// Receive spot streaming data.
        /// </summary>
        _IRF_SPOT_STREAM_DATA = 16,

        /// <summary>
        /// Request temperature and offset table. (pc --> cam)
        /// </summary>
        _IRF_REQ_TEMP_TABLE = 19,
    }
}