using System.Runtime.InteropServices;
using DDrop.BL.Radiometric.COX.Enums;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_AUTO_RANGE_METHOD_T
    {
        /// <summary>
        /// Automatic scale.
        /// </summary>
        public IRF_AUTOMATIC_TYPE_T autoScale;

        /// <summary>
        /// Input range setting method.
        /// </summary>
        public IRF_AUTO_RANGE_INPUT_METHOD_T inputMethod;

        /// <summary>
        /// Output range setting method.
        /// </summary>
        public IRF_AUTO_RANGE_OUTPUT_METHOD_T outputMethod;

        /// <summary>
        /// Parameter of input range method. (Brightness method)	(0 <= B_Rate <= 1.0)
        /// </summary>
        public float B_Rate;

        /// <summary>
        /// Parameter of input range method. (SD method)	(1.0 <= SD_Rate <= 6.0)
        /// </summary>
        public float SD_Rate;

        /// <summary>
        /// Intercept of linear method (0 <= intercept <= 254)
        /// </summary>
        public byte intercept;

        /// <summary>
        /// Gamma of non-linear method.	(0.1 <= gamma <= 25)
        /// </summary>
        public float gamma;

        /// <summary>
        /// Plateau value for tail-less plateau equalization.
        /// </summary>
        public uint plateau;

        /// <summary>
        /// The epsilon that is threshold value is a scalar arbitrary set to a value between zero and one. (Adaptive Plateau Algorithm)
        /// </summary>
        public float epsilon;

        /// <summary>
        /// The psi is a scalar arbitrary set to a value between zero and one. (Adaptive Plateau Algorithm)
        /// </summary>
        public float psi;

        /// <summary>
        /// previous plateau value for using Adaptive Plateau Algorithm.
        /// </summary>
        public float prevPalteau;
	}
}