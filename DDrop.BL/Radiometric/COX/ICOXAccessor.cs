using System;
using System.Drawing;
using DDrop.BL.Radiometric.COX.Enums;
using DDrop.BL.Radiometric.COX.Models;
using Point = System.Windows.Point;

namespace DDrop.BL.Radiometric.COX
{
    public interface ICOXAccessor
    {
        // Common Interface

        ReturnCode OpenConnectInternal(out IntPtr pHandle, out uint pTimerID, string strDestination, string strServiceName, int nProtocol, int nType);
        ReturnCode CloseConnectInternal(out IntPtr handle, uint timerID);
        ReturnCode GetIRHeaderInternal(IntPtr handle, out IRF_IR_FILE_HEADER_T header, out uint curPos);
        ReturnCode GetIRHeadersInternal(IntPtr handle, out IRF_IR_FILE_HEADER_T header, out IRF_IR_DATA_HEADER_T addedInfo, out uint curPos);
        ReturnCode SendCameraMessageInternal(IntPtr handle, out uint pTimerID, IRF_MESSAGE_TYPE_T type, ushort PMSGTYPE, ushort RCODE);
        ReturnCode SendMessageToCameraInternal(IntPtr handle, out uint pTimerID, IRF_MESSAGE_TYPE_T type, ushort PMSGTYPE, ushort RCODE, uint RCODE2, uint RCODE3, uint RCODE4);
        ReturnCode GetIRImagesInternal(IntPtr handle, out uint pTimerID, out IRF_IR_CAM_DATA_T cam_data);
        ReturnCode GetImageLUTInternal(out byte palette, IRF_PALETTE_TYPE_T paletteType, bool bInvert);
        ReturnCode GetGrayToPaletteImageInternal(out byte from_image, out IntPtr to_image, ushort width, ushort height, out byte palette, int BitsPixel, bool bMirror, bool bFlip);
        ReturnCode GetIRHistogramInternal(out uint hist, out ushort ir_image, long image_size);

        //	CX320 Only Interface

        float GetCorrectedTempInternal(out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, ushort engineOut);
        ReturnCode GetCamTempTableInternal(IntPtr handle, out uint pTimerID, out IRF_RECEIVED_TEMP_TABLE_T temp_table);
        ReturnCode GetTempRangeValueInternal(IRF_DYNAMIC_RANGE_T tempMode, out short min, out short max);

        //	CX320, CX640 Only Interface

        ReturnCode GetTempMapTableInternal(out float tempLUT, IRF_DYNAMIC_RANGE_T tempMode);
        ReturnCode GetImageInternal(out byte image, IntPtr ir_image, long image_size, out float tempLUT, out float level, out float span, out IRF_AUTO_RANGE_METHOD_T method);
        ReturnCode GetCorrectedImageInternal(out byte image, IntPtr ir_image, long image_size, out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, out float level, out float span,out IRF_AUTO_RANGE_METHOD_T method);
        ReturnCode GetPointTempInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, Point pt);
        ReturnCode GetROITempInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, Rectangle roi, out IRF_NUMERIC_INFO_T numInfo, out Point min_pt, out Point max_pt);
        ReturnCode GetRawToTempInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, out float tempImage);
        ReturnCode SendPaletteMessageInternal(IntPtr handle, out uint pTimerID, IRF_SET_USER_PALETTE struct_palette);
        ReturnCode GetCameraTempRangeValueInternal(IRF_DYNAMIC_RANGE_T tempMode, IRF_CAM_MODEL_TYPE_T camType, out short min, out short max);

        //	CG320, CG640 Only Interface

        ReturnCode GetImageCGInternal(out byte image, IntPtr ir_image, long image_size, out float level, out float span, out IRF_AUTO_RANGE_METHOD_T method);
        ReturnCode GetPointTempCGInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, IRF_TEMP_CORRECTION_PAR_T corrPara, Point pt);
        ReturnCode GetROITempCGInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, IRF_TEMP_CORRECTION_PAR_T corrPara, Rectangle roi, out IRF_NUMERIC_INFO_T numInfo, out Point min_pt, out Point max_pt);
        ReturnCode GetRawToTempCGInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, IRF_TEMP_CORRECTION_PAR_T corrPara, out float tempImage);
        ReturnCode ConvertRawToTempCGInternal(IntPtr _ir_image, IRF_TEMP_CORRECTION_PAR_T _corr, ushort _raw);
        ReturnCode GetTempRangeValueCGInternal(IntPtr _ir_image, IRF_DYNAMIC_RANGE_T tempMode, ushort min, ushort max);

        //	Image Filter Interface

        ReturnCode ApplyImageFilterInternal(out byte image, ushort width, ushort height, IRF_IMAGE_FILTER_T filter);
        ReturnCode ApplyColorImageFilterInternal(IntPtr image, ushort width, ushort height, IRF_IMAGE_FILTER_T filter, int bitPixel);
        ReturnCode BilateralFilterInternal(out byte image, ushort width, ushort height, float sigD, float sigR, int w);
        ReturnCode GetGaussianKernelInternal(out int kernel, out int mult, int sz);
        ReturnCode FastGaussianBlurInternal(out byte img, int iw, int ih, out int Gkernel, out int Gmult, int radius);
        ReturnCode FastStackBlurInternal(out byte img, int w, int h, int radius);
        ReturnCode BoxBlurInternal(out byte src, int src_w, int src_h, int sz);

        // CRD File Interface

        ReturnCode CreateIRCamDataCGInternal(out IRF_IR_CAM_DATA_T p_cam_data);
        ReturnCode CloseIRCamDataCGInternal(out IRF_IR_CAM_DATA_T _p_cam_data);
        ReturnCode LoadIRCamDataCGInternal(out IRF_IR_CAM_DATA_T _p_cam_data, out ushort _p_ir_buf, long _image_size);
        ReturnCode LoadIRImageInternal(out IntPtr handle, string fileName, out uint totSize);
        ReturnCode GetIRImageFromStreamInternal(IntPtr handle, out ushort ir_image, long image_size, uint totStreamSize, out uint curPos, out int gap_time, bool bLoop);
        ReturnCode GetIRImageFromStream_nInternal(IntPtr handle, out ushort ir_image, long image_size, uint totStreamSize, out uint curPos, out int gap_time, bool bLoop, bool new_ver);  // version < 20
        ReturnCode GetIRImageFromStream_v2Internal(IntPtr handle, out ushort ir_image, long image_size, uint totStreamSize, out uint curPos, out int gap_time, out Int64 curTime, bool bLoop, byte ver);  // version >= 20
        ReturnCode GetRevIRImageFromStreamInternal(IntPtr handle, out ushort ir_image, long image_size, out uint curPos, out int gap_time);
        ReturnCode GetRevIRImageFromStream_nInternal(IntPtr handle, out ushort ir_image, long image_size, out uint curPos, out int gap_time, bool new_ver);
        ReturnCode GetRevIRImageFromStream_v2Internal(IntPtr handle, out ushort ir_image, long image_size, out uint curPos, out int gap_time, out Int64 curTime, byte ver);
        ReturnCode SaveIRImageInternal(out IntPtr handle, string filename, out IRF_IR_FILE_HEADER_T pHeader);
        ReturnCode SaveIRHeaderInternal(out IntPtr handle, string filename, out IRF_IR_FILE_HEADER_T pHeader, out IRF_IR_DATA_HEADER_T pAddedData);
        ReturnCode SetIRImageToStreamInternal(IntPtr handle, out ushort ir_image, long image_size, int millisecond, out short frameCnt); // file version < 20
        ReturnCode SetIRImageToStream_v2Internal(IntPtr handle, out ushort ir_image, long image_size, int millisecond, out short frameCnt, byte ver);
        ReturnCode CloseIRStreamInternal(IntPtr handle);
        ReturnCode DrawColorBarInternal(IntPtr hWnd, IntPtr hDC, out byte palette, float level, float span, IRF_TEMP_MODE_T tempUnit, bool bUpdateOnlyTickArea);
        ReturnCode DrawMinMaxPosInternal(IntPtr hDC, Point minP, Point maxP, int size);
        
        // Temperature Converter

        float ConvertFahToCelsInternal(float temp);
        float ConvertCelsToFahInternal(float temp);
        float ConvertKelvToCelsInternal(float temp);
        float ConvertKelvToFahInternal(float temp);
        float ConvertCelToKelInternal(float temp);
        float ConvertFahToKelInternal(float temp);
    }
}