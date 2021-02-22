using System;
using System.Drawing;
using System.Runtime.InteropServices;
using DDrop.BL.Radiometric.COX.Enums;
using DDrop.BL.Radiometric.COX.Models;
using Point = System.Windows.Point;

namespace DDrop.BL.Radiometric.COX
{
    public class COXAccessor : ICOXAccessor
    {
        #region C++ Realization

        //	Common Interface

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "OpenConnect")]
        private static extern ReturnCode OpenConnect(out IntPtr pHandle, out uint pTimerID, string strDestination, string strServiceName, int nProtocol, int nType);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "CloseConnect")]
        private static extern ReturnCode CloseConnect(out IntPtr handle, uint timerID);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRHeader")]
        private static extern ReturnCode GetIRHeader(IntPtr handle, out IRF_IR_FILE_HEADER_T header, out uint curPos);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRHeaders")]
        private static extern ReturnCode GetIRHeaders(IntPtr handle, out IRF_IR_FILE_HEADER_T header, out IRF_IR_DATA_HEADER_T addedInfo, out uint curPos);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SendCameraMessage")]
        private static extern ReturnCode SendCameraMessage(IntPtr handle, out uint pTimerID, IRF_MESSAGE_TYPE_T type, ushort PMSGTYPE, ushort RCODE);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SendMessageToCamera")]
        private static extern ReturnCode SendMessageToCamera(IntPtr handle, out uint pTimerID, IRF_MESSAGE_TYPE_T type,
            ushort PMSGTYPE, ushort RCODE, uint RCODE2, uint RCODE3, uint RCODE4);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRImages")]
        private static extern ReturnCode GetIRImages(IntPtr handle, out uint pTimerID, out IRF_IR_CAM_DATA_T cam_data);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetImageLUT")]
        private static extern ReturnCode GetImageLUT(out byte palette, IRF_PALETTE_TYPE_T paletteType, bool bInvert);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetGrayToPaletteImage")]
        private static extern ReturnCode GetGrayToPaletteImage(out byte from_image, out IntPtr to_image, ushort width,
            ushort height, out byte palette, int BitsPixel, bool bMirror, bool bFlip);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRHistogram")]
        private static extern ReturnCode GetIRHistogram(out uint hist, out ushort ir_image, long image_size);

        //	CX320 Only Interface

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetCorrectedTemp")]
        private static extern float GetCorrectedTemp(out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, ushort engineOut);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetCamTempTable")]
        private static extern ReturnCode GetCamTempTable(IntPtr handle, out uint pTimerID, out IRF_RECEIVED_TEMP_TABLE_T temp_table);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetTempRangeValue")]
        private static extern ReturnCode GetTempRangeValue(IRF_DYNAMIC_RANGE_T tempMode, out short min, out short max);

        //	CX320, CX640 Only Interface

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetTempMapTable")]
        private static extern ReturnCode GetTempMapTable(out float tempLUT, IRF_DYNAMIC_RANGE_T tempMode);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetImage")]
        private static extern ReturnCode GetImage(out byte image, IntPtr ir_image, long image_size, out float tempLUT, out float level, out float span, out IRF_AUTO_RANGE_METHOD_T method);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetCorrectedImage")]
        private static extern ReturnCode GetCorrectedImage(out byte image, IntPtr ir_image, long image_size,
            out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, out float level, out float span, out IRF_AUTO_RANGE_METHOD_T method);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetPointTemp")]
        private static extern ReturnCode GetPointTemp(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, Point pt);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetROITemp")]
        private static extern ReturnCode GetROITemp(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, Rectangle roi, out IRF_NUMERIC_INFO_T numInfo, out Point min_pt,
            out Point max_pt);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetRawToTemp")]
        private static extern ReturnCode GetRawToTemp(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, out float tempImage);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SendPaletteMessage")]
        private static extern ReturnCode SendPaletteMessage(IntPtr handle, out uint pTimerID,
            IRF_SET_USER_PALETTE struct_palette);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetCameraTempRangeValue")]
        private static extern ReturnCode GetCameraTempRangeValue(IRF_DYNAMIC_RANGE_T tempMode,
            IRF_CAM_MODEL_TYPE_T camType, out short min,
            out short max);

        //	CG320, CG640 Only Interface

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetImageCG")]
        private static extern ReturnCode GetImageCG(out byte image, IntPtr ir_image, long image_size, out float level,
            out float span,
            out IRF_AUTO_RANGE_METHOD_T method);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetPointTempCG")]
        private static extern ReturnCode GetPointTempCG(IntPtr ir_image, IRF_IMAGE_INFO_T image_info,
            IRF_TEMP_CORRECTION_PAR_T corrPara,
            Point pt);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetROITempCG")]
        private static extern ReturnCode GetROITempCG(IntPtr ir_image, IRF_IMAGE_INFO_T image_info,
            IRF_TEMP_CORRECTION_PAR_T corrPara,
            Rectangle roi, out IRF_NUMERIC_INFO_T numInfo, out Point min_pt, out Point max_pt);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetRawToTempCG")]
        private static extern ReturnCode GetRawToTempCG(IntPtr ir_image, IRF_IMAGE_INFO_T image_info,
            IRF_TEMP_CORRECTION_PAR_T corrPara,
            out float tempImage);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertRawToTempCG")]
        private static extern ReturnCode ConvertRawToTempCG(IntPtr _ir_image, IRF_TEMP_CORRECTION_PAR_T _corr, ushort _raw);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetTempRangeValueCG")]
        private static extern ReturnCode GetTempRangeValueCG(IntPtr _ir_image, IRF_DYNAMIC_RANGE_T tempMode, ushort min,
            ushort max);

        //	Image Filter Interface

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ApplyImageFilter")]
        private static extern ReturnCode ApplyImageFilter(out byte image, ushort width, ushort height, IRF_IMAGE_FILTER_T filter);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ApplyColorImageFilter")]
        private static extern ReturnCode ApplyColorImageFilter(IntPtr image, ushort width, ushort height, IRF_IMAGE_FILTER_T filter, int bitPixel);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "BilateralFilter")]
        private static extern ReturnCode BilateralFilter(out byte image, ushort width, ushort height, float sigD, float sigR, int w);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetGaussianKernel")]
        private static extern ReturnCode GetGaussianKernel(out int kernel, out int mult, int sz);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "FastGaussianBlur")]
        private static extern ReturnCode FastGaussianBlur(out byte img, int iw, int ih, out int Gkernel, out int Gmult, int radius);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "FastStackBlur")]
        private static extern ReturnCode FastStackBlur(out byte img, int w, int h, int radius);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "BoxBlur")]
        private static extern ReturnCode BoxBlur(out byte src, int src_w, int src_h, int sz);

        //	CRD File Interface
        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "LoadIRCamDataCG")]
        private static extern ReturnCode LoadIRCamDataCG(out IRF_IR_CAM_DATA_T _p_cam_data, out ushort _p_ir_buf,
            long _image_size);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "LoadIRImage")]
        private static extern ReturnCode LoadIRImage(out IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string fileName, out uint totSize);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRImageFromStream")]
        private static extern ReturnCode GetIRImageFromStream(IntPtr handle, out ushort ir_image, long image_size,
            uint totStreamSize,
            out uint curPos, out int gap_time, bool bLoop);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRImageFromStream_n")]
        private static extern ReturnCode GetIRImageFromStream_n(IntPtr handle, out ushort ir_image, long image_size,
            uint totStreamSize,
            out uint curPos, out int gap_time, bool bLoop, bool new_ver);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetIRImageFromStream_v2")]
        private static extern ReturnCode GetIRImageFromStream_v2(IntPtr handle, out ushort ir_image, long image_size,
            uint totStreamSize,
            out uint curPos, out int gap_time, out long curTime, bool bLoop, byte ver);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetRevIRImageFromStream")]
        private static extern ReturnCode GetRevIRImageFromStream(IntPtr handle, out ushort ir_image, long image_size,
            out uint curPos,
            out int gap_time);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetRevIRImageFromStream_n")]
        private static extern ReturnCode GetRevIRImageFromStream_n(IntPtr handle, out ushort ir_image, long image_size,
            out uint curPos,
            out int gap_time, bool new_ver);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "GetRevIRImageFromStream_v2")]
        private static extern ReturnCode GetRevIRImageFromStream_v2(IntPtr handle, out ushort ir_image, long image_size,
            out uint curPos,
            out int gap_time, out long curTime, byte ver);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SaveIRImage")]
        private static extern ReturnCode SaveIRImage(out IntPtr handle, string filename, out IRF_IR_FILE_HEADER_T pHeader);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SaveIRHeader")]
        private static extern ReturnCode SaveIRHeader(out IntPtr handle, string filename, out IRF_IR_FILE_HEADER_T pHeader,
            out IRF_IR_DATA_HEADER_T pAddedData);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SetIRImageToStream")]
        private static extern ReturnCode SetIRImageToStream(IntPtr handle, out ushort ir_image, long image_size,
            int millisecond,
            out short frameCnt);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "SetIRImageToStream_v2")]
        private static extern ReturnCode SetIRImageToStream_v2(IntPtr handle, out ushort ir_image, long image_size,
            int millisecond,
            out short frameCnt, byte ver);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "CreateIRCamDataCG")]
        private static extern ReturnCode CreateIRCamDataCG(out IRF_IR_CAM_DATA_T p_cam_data);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "CloseIRCamDataCG")]
        private static extern ReturnCode CloseIRCamDataCG(out IRF_IR_CAM_DATA_T _p_cam_data);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "CloseIRStream")]
        private static extern ReturnCode CloseIRStream(IntPtr handle);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "DrawColorBar")]
        private static extern ReturnCode DrawColorBar(IntPtr hWnd, IntPtr hDC, out byte palette, float level, float span,
            IRF_TEMP_MODE_T tempUnit, bool bUpdateOnlyTickArea);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "DrawMinMaxPos")]
        private static extern ReturnCode DrawMinMaxPos(IntPtr hDC, Point minP, Point maxP, int size);

        //	Temperature Converter

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertFahToCels")]
        private static extern float ConvertFahToCels(float temp);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertCelsToFah")]
        private static extern float ConvertCelsToFah(float temp);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertKelvToCels")]
        private static extern float ConvertKelvToCels(float temp);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertKelvToFah")]
        private static extern float ConvertKelvToFah(float temp);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertCelToKel")]
        private static extern float ConvertCelToKel(float temp);

        [DllImport("ThermalCamDll", CallingConvention = CallingConvention.StdCall, ExactSpelling = false, EntryPoint = "ConvertFahToKel")]
        private static extern float ConvertFahToKel(float temp);

        #endregion

        #region C# Realization

        //	Common Interface

        public ReturnCode OpenConnectInternal(out IntPtr pHandle, out uint pTimerID, string strDestination, string strServiceName, int nProtocol, int nType)
        {
            return OpenConnect(out pHandle, out pTimerID, strDestination, strServiceName, nProtocol, nType);
        }

        public ReturnCode CloseConnectInternal(out IntPtr handle, uint timerID)
        {
            return CloseConnect(out handle, timerID);
        }

        public ReturnCode GetIRHeaderInternal(IntPtr handle, out IRF_IR_FILE_HEADER_T header, out uint curPos)
        {
            return GetIRHeader(handle, out header, out curPos);
        }

        public ReturnCode GetIRHeadersInternal(IntPtr handle, out IRF_IR_FILE_HEADER_T header, out IRF_IR_DATA_HEADER_T addedInfo, out uint curPos)
        {
            return GetIRHeaders(handle, out header, out addedInfo, out curPos);
        }

        public ReturnCode SendCameraMessageInternal(IntPtr handle, out uint pTimerID, IRF_MESSAGE_TYPE_T type, ushort PMSGTYPE, ushort RCODE)
        {
            return SendCameraMessage(handle, out pTimerID, type, PMSGTYPE, RCODE);
        }

        public ReturnCode SendMessageToCameraInternal(IntPtr handle, out uint pTimerID, IRF_MESSAGE_TYPE_T type, ushort PMSGTYPE, ushort RCODE, uint RCODE2, uint RCODE3, uint RCODE4)
        {
            return SendMessageToCamera(handle, out pTimerID, type, PMSGTYPE, RCODE, RCODE2, RCODE3, RCODE4);
        }

        public ReturnCode GetIRImagesInternal(IntPtr handle, out uint pTimerID, out IRF_IR_CAM_DATA_T cam_data)
        {
            return GetIRImages(handle, out pTimerID, out cam_data);
        }

        public ReturnCode GetImageLUTInternal(out byte palette, IRF_PALETTE_TYPE_T paletteType, bool bInvert)
        {
            return GetImageLUT(out palette, paletteType, bInvert);
        }

        public ReturnCode GetGrayToPaletteImageInternal(out byte from_image, out IntPtr to_image, ushort width, ushort height, out byte palette, int BitsPixel, bool bMirror, bool bFlip)
        {
            return GetGrayToPaletteImage(out from_image, out to_image, width, height, out palette, BitsPixel, bMirror, bFlip);
        }

        public ReturnCode GetIRHistogramInternal(out uint hist, out ushort ir_image, long image_size)
        {
            return GetIRHistogram(out hist, out ir_image, image_size);
        }

        //	CX320 Only Interface

        public float GetCorrectedTempInternal(out float tempLUT, IRF_TEMP_CORRECTION_PAR_T corrPara, ushort engineOut)
        {
            return GetCorrectedTemp(out tempLUT, corrPara, engineOut);
        }

        public ReturnCode GetCamTempTableInternal(IntPtr handle, out uint pTimerID, out IRF_RECEIVED_TEMP_TABLE_T temp_table)
        {
            return GetCamTempTable(handle, out pTimerID, out temp_table);
        }

        public ReturnCode GetTempRangeValueInternal(IRF_DYNAMIC_RANGE_T tempMode, out short min, out short max)
        {
            return GetTempRangeValue(tempMode, out min, out max);
        }

        //	CX320, CX640 Only Interface

        public ReturnCode GetTempMapTableInternal(out float tempLUT, IRF_DYNAMIC_RANGE_T tempMode)
        {
            return GetTempMapTable(out tempLUT, tempMode);
        }

        public ReturnCode GetImageInternal(out byte image, IntPtr ir_image, long image_size, out float tempLUT, out float level,
            out float span, out IRF_AUTO_RANGE_METHOD_T method)
        {
            return GetImage(out image, ir_image, image_size, out tempLUT, out level, out span, out method);
        }

        public ReturnCode GetCorrectedImageInternal(out byte image, IntPtr ir_image, long image_size, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, out float level, out float span, out IRF_AUTO_RANGE_METHOD_T method)
        {
            return GetCorrectedImage(out image, ir_image, image_size, out tempLUT, corrPara, out level, out span,
                out method);
        }

        public ReturnCode GetPointTempInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, Point pt)
        {
            return GetPointTemp(ir_image, image_info, out tempLUT, corrPara, pt);
        }

        public ReturnCode GetROITempInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, Rectangle roi, out IRF_NUMERIC_INFO_T numInfo, out Point min_pt,
            out Point max_pt)
        {
            return GetROITemp(ir_image, image_info, out tempLUT, corrPara, roi, out numInfo, out min_pt, out max_pt);
        }

        public ReturnCode GetRawToTempInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, out float tempLUT,
            IRF_TEMP_CORRECTION_PAR_T corrPara, out float tempImage)
        {
            return GetRawToTemp(ir_image, image_info, out tempLUT, corrPara, out tempImage);
        }

        public ReturnCode SendPaletteMessageInternal(IntPtr handle, out uint pTimerID, IRF_SET_USER_PALETTE struct_palette)
        {
            return SendPaletteMessage(handle, out pTimerID, struct_palette);
        }

        public ReturnCode GetCameraTempRangeValueInternal(IRF_DYNAMIC_RANGE_T tempMode, IRF_CAM_MODEL_TYPE_T camType, out short min,
            out short max)
        {
            return GetCameraTempRangeValue(tempMode, camType, out min, out max);
        }

        //	CG320, CG640 Only Interface

        public ReturnCode GetImageCGInternal(out byte image, IntPtr ir_image, long image_size, out float level, out float span,
            out IRF_AUTO_RANGE_METHOD_T method)
        {
            return GetImageCG(out image, ir_image, image_size, out level, out span, out method);
        }

        public ReturnCode GetPointTempCGInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, IRF_TEMP_CORRECTION_PAR_T corrPara,
            Point pt)
        {
            return GetPointTempCG(ir_image, image_info, corrPara, pt);
        }

        public ReturnCode GetROITempCGInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, IRF_TEMP_CORRECTION_PAR_T corrPara,
            Rectangle roi, out IRF_NUMERIC_INFO_T numInfo, out Point min_pt, out Point max_pt)
        {
            return GetROITempCG(ir_image, image_info, corrPara, roi, out numInfo, out min_pt, out max_pt);
        }

        public ReturnCode GetRawToTempCGInternal(IntPtr ir_image, IRF_IMAGE_INFO_T image_info, IRF_TEMP_CORRECTION_PAR_T corrPara,
            out float tempImage)
        {
            return GetRawToTempCG(ir_image, image_info, corrPara, out tempImage);
        }

        public ReturnCode ConvertRawToTempCGInternal(IntPtr _ir_image, IRF_TEMP_CORRECTION_PAR_T _corr, ushort _raw)
        {
            return ConvertRawToTempCG(_ir_image, _corr, _raw);
        }

        public ReturnCode GetTempRangeValueCGInternal(IntPtr _ir_image, IRF_DYNAMIC_RANGE_T tempMode, ushort min, ushort max)
        {
            return GetTempRangeValueCG(_ir_image, tempMode, min, max);
        }

        //	Image Filter Interface

        public ReturnCode ApplyImageFilterInternal(out byte image, ushort width, ushort height, IRF_IMAGE_FILTER_T filter)
        {
            return ApplyImageFilter(out image, width, height, filter);
        }

        public ReturnCode ApplyColorImageFilterInternal(IntPtr image, ushort width, ushort height, IRF_IMAGE_FILTER_T filter, int bitPixel)
        {
            return ApplyColorImageFilter(image, width, height, filter, bitPixel);
        }

        public ReturnCode BilateralFilterInternal(out byte image, ushort width, ushort height, float sigD, float sigR, int w)
        {
            return BilateralFilter(out image, width, height, sigD, sigR, w);
        }

        public ReturnCode GetGaussianKernelInternal(out int kernel, out int mult, int sz)
        {
            return GetGaussianKernel(out kernel, out mult, sz);
        }

        public ReturnCode FastGaussianBlurInternal(out byte img, int iw, int ih, out int Gkernel, out int Gmult, int radius)
        {
            return FastGaussianBlur(out img, iw, ih, out Gkernel, out Gmult, radius);
        }

        public ReturnCode FastStackBlurInternal(out byte img, int w, int h, int radius)
        {
            return FastStackBlur(out img, w, h, radius);
        }

        public ReturnCode BoxBlurInternal(out byte src, int src_w, int src_h, int sz)
        {
            return BoxBlur(out src, src_w, src_h, sz);
        }

        // CRD File Interface

        public ReturnCode LoadIRCamDataCGInternal(out IRF_IR_CAM_DATA_T _p_cam_data, out ushort _p_ir_buf, long _image_size)
        {
            return LoadIRCamDataCG(out _p_cam_data, out _p_ir_buf, _image_size);
        }

        public ReturnCode LoadIRImageInternal(out IntPtr handle, string fileName, out uint totSize)
        {
            return LoadIRImage(out handle, fileName, out totSize);
        }

        public ReturnCode GetIRImageFromStreamInternal(IntPtr handle, out ushort ir_image, long image_size, uint totStreamSize,
            out uint curPos, out int gap_time, bool bLoop)
        {
            return GetIRImageFromStream(handle, out ir_image, image_size, totStreamSize, out curPos, out gap_time, bLoop);
        }

        public ReturnCode GetIRImageFromStream_nInternal(IntPtr handle, out ushort ir_image, long image_size, uint totStreamSize,
            out uint curPos, out int gap_time, bool bLoop, bool new_ver)
        {
            return GetIRImageFromStream_n(handle, out ir_image, image_size, totStreamSize, out curPos, out gap_time, bLoop, new_ver);
        }

        public ReturnCode GetIRImageFromStream_v2Internal(IntPtr handle, out ushort ir_image, long image_size, uint totStreamSize,
            out uint curPos, out int gap_time, out long curTime, bool bLoop, byte ver)
        {
            return GetIRImageFromStream_v2(handle, out ir_image, image_size, totStreamSize, out curPos, out gap_time,
                out curTime, bLoop, ver);
        }

        public ReturnCode GetRevIRImageFromStreamInternal(IntPtr handle, out ushort ir_image, long image_size, out uint curPos,
            out int gap_time)
        {
            return GetRevIRImageFromStream(handle, out ir_image, image_size, out curPos, out gap_time);
        }

        public ReturnCode GetRevIRImageFromStream_nInternal(IntPtr handle, out ushort ir_image, long image_size, out uint curPos,
            out int gap_time, bool new_ver)
        {
            return GetRevIRImageFromStream_n(handle, out ir_image, image_size, out curPos, out gap_time, new_ver);
        }

        public ReturnCode GetRevIRImageFromStream_v2Internal(IntPtr handle, out ushort ir_image, long image_size, out uint curPos,
            out int gap_time, out long curTime, byte ver)
        {
            return GetRevIRImageFromStream_v2(handle, out ir_image, image_size, out curPos, out gap_time, out curTime,
                ver);
        }

        public ReturnCode SaveIRImageInternal(out IntPtr handle, string filename, out IRF_IR_FILE_HEADER_T pHeader)
        {
            return SaveIRImage(out handle, filename, out pHeader);
        }

        public ReturnCode SaveIRHeaderInternal(out IntPtr handle, string filename, out IRF_IR_FILE_HEADER_T pHeader,
            out IRF_IR_DATA_HEADER_T pAddedData)
        {
            return SaveIRHeader(out handle, filename, out pHeader, out pAddedData);
        }

        public ReturnCode SetIRImageToStreamInternal(IntPtr handle, out ushort ir_image, long image_size, int millisecond,
            out short frameCnt)
        {
            return SetIRImageToStream(handle, out ir_image, image_size, millisecond, out frameCnt);
        }

        public ReturnCode SetIRImageToStream_v2Internal(IntPtr handle, out ushort ir_image, long image_size, int millisecond,
            out short frameCnt, byte ver)
        {
            return SetIRImageToStream_v2(handle, out ir_image, image_size, millisecond, out frameCnt, ver);
        }

        public ReturnCode CreateIRCamDataCGInternal(out IRF_IR_CAM_DATA_T p_cam_data)
        {
            return CreateIRCamDataCG(out p_cam_data);
        }

        public ReturnCode CloseIRCamDataCGInternal(out IRF_IR_CAM_DATA_T _p_cam_data)
        {
            return CloseIRCamDataCG(out _p_cam_data);
        }

        public ReturnCode CloseIRStreamInternal(IntPtr handle)
        {
            return CloseIRStream(handle);
        }

        public ReturnCode DrawColorBarInternal(IntPtr hWnd, IntPtr hDC, out byte palette, float level, float span,
            IRF_TEMP_MODE_T tempUnit, bool bUpdateOnlyTickArea)
        {
            return DrawColorBar(hWnd, hDC, out palette, level, span, tempUnit, bUpdateOnlyTickArea);
        }

        public ReturnCode DrawMinMaxPosInternal(IntPtr hDC, Point minP, Point maxP, int size)
        {
            return DrawMinMaxPos(hDC, minP, maxP, size);
        }

        // Temperature Converter

        public float ConvertFahToCelsInternal(float temp)
        {
            return ConvertFahToCels(temp);
        }

        public float ConvertCelsToFahInternal(float temp)
        {
            return ConvertCelsToFah(temp);
        }

        public float ConvertKelvToCelsInternal(float temp)
        {
            return ConvertKelvToCels(temp);
        }

        public float ConvertKelvToFahInternal(float temp)
        {
            return ConvertKelvToFah(temp);
        }

        public float ConvertCelToKelInternal(float temp)
        {
            return ConvertCelToKel(temp);
        }

        public float ConvertFahToKelInternal(float temp)
        {
            return ConvertFahToKel(temp);
        }

        #endregion

    }
}