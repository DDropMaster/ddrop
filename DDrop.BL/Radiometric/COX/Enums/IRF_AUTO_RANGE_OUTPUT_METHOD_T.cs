namespace DDrop.BL.Radiometric.COX.Enums
{
    public enum IRF_AUTO_RANGE_OUTPUT_METHOD_T
    {
        /// <summary>
        /// Linear method. (contrast + brightness)
        /// </summary>
        _IRF_LINEAR = 0,

        /// <summary>
        /// Non-Linear method. (Gamma function)
        /// </summary>
        _IRF_NON_LINEAR,

        /// <summary>
        /// Tail-less Plateau Equalization.
        /// </summary>
        _IRF_TPE,

        /// <summary>
        /// Adaptive Plateau Equalization.
        /// </summary>
        _IRF_APE,

        /// <summary>
        /// Self-adaptive plateau equalization.
        /// </summary>
        _IRF_SAPE
    }
}