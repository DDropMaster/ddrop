using System.Runtime.InteropServices;

namespace DDrop.BL.Radiometric.COX.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IRF_SAVEDATA_T
    {
        #region union1

        /// <summary>
        /// CRC Data
        /// </summary>
        public uint crc;

        /// <summary>
        /// Setup Data Version	( CG Model : 0x20 )
        /// </summary>
        public byte ver;

        /// <summary>
        /// Sensor Type ( 0x00 : CX320, 0x01 : CX640, 0x20 : CG QVGA, 0x21 : CG VGA )
        /// </summary>
        public byte sensor;

        /// <summary>
        /// Video Output Format ( 0 : NTSC, 1 : PAL )
        /// </summary>
        public byte tv;

        /// <summary>
        /// Temperature Measurement Mode ( 0 : Normal, 1 : High, 2 : Medical )
        /// </summary>
        public byte temp_mode;

        /// <summary>
        /// RS485 ID
        /// </summary>
        public byte id;

        /// <summary>
        /// RS485 Baudrates
        /// </summary>
        public byte baudrate;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public short level;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public ushort span;

        /// <summary>
        /// AGC MODE ( 1 : AUTO, 0 : MANUAL )
        /// </summary>
        public byte agc;

        /// <summary>
        /// Video Color Invert ( 0 : OFF, 1 : LUMA, 2 : CHROMA, 3 : L + C )
        /// </summary>
        public byte invert;

        /// <summary>
        /// Video Mirror ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte mirror;

        /// <summary>
        /// Video Flip ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte flip;

        /// <summary>
        /// Color-Bar ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte colorbar;

        /// <summary>
        /// Show Temperature Information ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte showinfo;

        /// <summary>
        /// High/Low Indicator ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte indicator;

        /// <summary>
        /// Temperature Unit ( 0 : Celcius, 1 : Fahrenheit )
        /// </summary>
        public byte unit;

        /// <summary>
        /// DHCP Enable ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte dhcp;

        /// <summary>
        /// Color Pallete ( reference IRF_CAM_PALETTE_TYPE_T )
        /// </summary>
        public byte color;

        /// <summary>
        /// Alpha Blending ( 0 ; OFF, 1 : 20%, 2 : 40%, 3 : 60%, 4 : 80% )
        /// </summary>
        public byte alpha;

        /// <summary>
        /// Video Zoom ( 0 : OFF, 1 : x2, 2 : x4 )
        /// </summary>
        public byte zoom;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte sharp;

        /// <summary>
        /// Noise Filter ( 0 : OFF, 1 : NR1, 2 : NR2, 3 : NR1 + NR2 )
        /// </summary>
        public byte noise;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public ushort nuc;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte econt;

        /// <summary>
        /// IP Address ( ex : 192.168.0.100 => 0xC0A80064 )
        /// </summary>
        public uint ipaddr;

        /// <summary>
        /// Subnet Mask ( ex : 255.255.255.0 => 0xFFFFFF00 )
        /// </summary>
        public uint netmask;

        /// <summary>
        /// Gateway ( ex : 192.168.0.1 => 0xC0A80001 )
        /// </summary>
        public uint gateway;

        /// <summary>
        /// Not Used
        /// </summary>
        public uint dns;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte alarm1_func;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte alarm1_cond;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public short alarm1_value;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte alarm2_func;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte alarm2_cond;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public short alarm2_value;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte down_filter;

        /// <summary>
        /// Show Center Cross ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte show_center;

        /// <summary>
        /// Show Spot For CX Model ( 0 : OFF, 1 : ON ) 
        /// </summary>
        public byte show_spot;

        /// <summary>
        /// Show ROI For CG Model ( 0 : OFF, 1 : ON )
        /// Show Correction Parameters ( 0 : OFF, 1 : ON ) 
        /// </summary>
        public byte show_correction;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte show_isotherm;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte alarm1_duration;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte alarm2_duration;

        #region innerStruct

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public roi[] roi;

        #endregion

        #region CG Model

        /// <summary>
        /// Show Display Icon Flag ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte f_disp_icon;

        /// <summary>
        /// Video Brightness Value ( -40 ~ 40 )
        /// </summary>
        public byte brightness;

        /// <summary>
        /// Video Contrast Value ( -10 ~ 10 )
        /// </summary>
        public byte contrast;

        /// <summary>
        /// Video Edge Enhancement Flag ( 0 : OFF, 1 : ON )
        /// </summary>
        public byte f_edge_enhance;

        /// <summary>
        /// Core NUC Mode Value ( 0 : OFF, 1 : TIME, 2 : AUTO, 3 : TIME + AUTO )
        /// </summary>
        public byte nuc_mode;

        /// <summary>
        /// Core NUC Time Value ( 0 : 1 MIN, 1 : 5 MIN, 2 : 10 MIN, 3 : 30 MIN, 4 : 60 MIN )
        /// </summary>
        public byte nuc_time;

        /// <summary>
        /// Core NUC Auto Threshold Value ( 0 : lowest, 1 : low, 2 : middle, 3 : high, 4 : highest )
        /// </summary>
        public ushort nuc_thres;

        /// <summary>
        /// Core AGC Manual Maximum Value ( 0 ~ 16383 )
        /// </summary>
        public ushort agc_man_max;

        /// <summary>
        /// Core AGC Manual Minimum Value ( 0 ~ 16383 )
        /// </summary>
        public ushort agc_man_min;

        /// <summary>
        /// Serial Protocol Value ( 0 : Pelco-D, 1 : COX )
        /// </summary>
        public byte srl_protocol;

        /// <summary>
        /// Video left margin ( 0 ~ 70 )
        /// </summary>
        public ushort scn0_l_margin;

        /// <summary>
        /// Video right margin ( 0 ~ 70 )
        /// </summary>
        public ushort scn0_r_margin;

        /// <summary>
        /// Video top margin ( 0 ~ 70 )
        /// </summary>
        public ushort scn0_t_margin;

        /// <summary>
        /// Video bottom margin ( 0 ~ 70 )
        /// </summary>
        public ushort scn0_b_margin;

        /// <summary>
        /// HDMI left margin ( 0 ~ 150 )
        /// </summary>
        public ushort scn1_l_margin;

        /// <summary>
        /// HDMI right margin ( 0 ~ 150 )
        /// </summary>
        public ushort scn1_r_margin;

        /// <summary>
        /// HDMI top margin ( 0 ~ 150 )
        /// </summary>
        public ushort scn1_t_margin;

        /// <summary>
        /// HDMI bottom margin ( 0 ~ 150 )
        /// </summary>
        public ushort scn1_b_margin;

        /// <summary>
        /// HDMI Mode ( 2 : 480P, 3 : 576P, 4 : 720P 50, 5 : 720P 60, 6 : 1080I 50, 7 : 1080I 60, 9 : 1080P 50, 10 : 1080P 60 )
        /// </summary>
        public byte hdmi_mode;

        /// <summary>
        /// Alarm1 type ( 0 : NO, 1 : NC )
        /// </summary>
        public byte alarm1_type;

        /// <summary>
        /// Alarm1 operation mode ( 0 : OFF, 1 : STABILIZE, 2 : ALIVE PWM, 3 : TEMPERATURE, 4 : TEST ALARM ON, 5 : TEST ALARM OFF )
        /// </summary>
        public byte alarm1_mode;

        /// <summary>
        /// Alarm1 duration ( 0 ~ 99 )
        /// </summary>
        public byte alarm1_dura;

        /// <summary>
        /// Alarm1 remote control ( 0 : OFF, 1 : ON ) 
        /// </summary>
        public byte alarm1_remote_ctrl;

        /// <summary>
        /// Alarm2 type ( 0 : NO, 1 : NC )
        /// </summary>
        public byte alarm2_type;

        /// <summary>
        /// Alarm2 operation mode ( 0 : OFF, 1 : STABILIZE, 2 : ALIVE PWM, 3 : TEMPERATURE, 4 : TEST ALARM ON, 5 : TEST ALARM OFF )
        /// </summary>
        public byte alarm2_mode;

        /// <summary>
        /// Alarm2 duration ( 0 ~ 99 )
        /// </summary>
        public byte alarm2_dura;

        /// <summary>
        /// Alarm2 remote control ( 0 : OFF, 1 : ON ) 
        /// </summary>
        public byte alarm2_remote_ctrl;

        /// <summary>
        /// Temperature Zero Offset
        /// </summary>
        public short zero_offset;

        /// <summary>
        /// Measure Distance
        /// </summary>
        public ushort measure_distance;

        /// <summary>
        /// Noise Reduction 1 Strength
        /// </summary>
        public byte nr1_strength;

        /// <summary>
        /// Noise Reduction 2 Strength
        /// </summary>
        public byte nr2_strength;

        /// <summary>
        /// Edge Enhancement Strength
        /// </summary>
        public byte ee_strength;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] public byte[] reserved1;

        #endregion

        #endregion

        #region union2

        /// <summary>
        /// 9Hz Mode
        /// </summary>
        public byte limit9;

        /// <summary>
        /// CX Model Only
        /// </summary>
        public byte enable_high;

        /// <summary>
        /// Correction Parameters ( 0 : Not Used, 1 : Used )
        /// </summary>
        public byte correction;

        /// <summary>
        /// Emissivity ( 1 ~ 100 )
        /// </summary>
        public byte emissivity;

        /// <summary>
        /// Transmission ( 1 ~ 100 )
        /// </summary>
        public byte transmission;

        /// <summary>
        /// Atmosphere (-500 ~ 1000 )
        /// </summary>
        public short atmosphere;

        /// <summary>
        /// CX Model Only
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public spot[] spot;

        /// <summary>
        /// CX Model Only
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public cg_roi[] cg_roi;

        /// <summary>
        /// CX Model Only
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]  public iso[] iso;

        /// <summary>
        /// CG Model Only
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public cg_roi[] cg_iso;


        #region Start CG Model Only

        /// <summary>
        /// Supported HDMI Output Resolution List
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] hdmi_list;

        /// <summary>
        /// Supported Temperature Mode
        /// </summary>
        public ushort support_temp_mode;

        /// <summary>
        /// Support Motorized Focus
        /// </summary>
        public byte f_support_motor_focus;

        /// <summary>
        /// Support Motorized Zoom
        /// </summary>
        public byte f_support_motor_zoom;

        /// <summary>
        /// Supported Measurment Distance Minimum
        /// </summary>
        public ushort meas_dist_min;

        /// <summary>
        /// Supported Measurment Distance Maximum
        /// </summary>
        public ushort meas_dist_max;

        #endregion

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] public byte[] reserved2;

        #endregion

        #region union3

        /// <summary>
        /// MAX Temperature 
        /// </summary>
        public ALRMCFG max_temp;

        /// <summary>
        /// MIN Temperature 
        /// </summary>
        public ALRMCFG min_temp;

        /// <summary>
        /// AVG Temperature
        /// </summary>
        public ALRMCFG avg_temp;

        /// <summary>
        /// CTR Temperature
        /// </summary>
        public ALRMCFG ctr_temp;

        /// <summary>
        /// ROI Temperature 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ALRMCFG[] roi_temp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] reserved3;

        #endregion
    }
}