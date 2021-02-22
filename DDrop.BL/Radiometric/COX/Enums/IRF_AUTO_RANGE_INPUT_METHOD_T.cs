namespace DDrop.BL.Radiometric.COX.Enums
{
    public enum IRF_AUTO_RANGE_INPUT_METHOD_T
    {
        /// <summary>
        /// MinMax Algorithm
        /// </summary>
        _IRF_MIN_MAX = 0,

        /// <summary>
        /// Brightness Rate (%)
        /// </summary>
        _IRF_BRIGHTNESS_RATE,

        /// <summary>
        /// Standard Deviation Rate (%)
        /// </summary>
        _IRF_SD_RATE,

        /// <summary>
        /// Auto Brightness
        /// </summary>
        _IRF_AUTO_BRIGHT,

        /// <summary>
        /// Enhance Histogram
        /// </summary>
        _IRF_ENHANCE_HIST
    }
}