using Rmsa.Converter;
using SkiaSharp;
using System.ComponentModel;

namespace Rmsa
{
    public static class Defines
    {
        public const string ConfigDir = "config";
        public const string LogsDir = "logs";
        public const string LogFileName = "logs\\{0:yyyy-MM-dd_HH-mm-ss}.log";
        public const string DataSourceConfigFileName = "config\\DataSource.config";
        public const string SettingsFileName = "config\\main.config";
        public const string ChannelSettingsFileName = "config\\{0}.config";
        public static SKColor DisplayBackgroundColor = new SKColor(0, 12, 0);
        public static int DesiredFps = 60;
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataMode
    {
        // each new data sample append to the current frame. The frame has a fixed length and works like a FIFO queue
        Stream = 1,
        // new data collected until the whole frame is received, then the old frame replaced with the new one
        Frame = 2,
        // new data samples append to existing data, prev data never deleted
        All = 3,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum TransformAlgorithm
    {
        NOP,
        FFT,
        DFT,
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataSourceType
    {
        [Description("File")]
        File,
        [Description("COM Port")]
        ComPort,
        [Description("Generator")]
        Generator,
    }

    //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataChannelNo
    {
        [Description("Channel 1")]
        CH1 = 1,

        [Description("Channel 2")]
        CH2 = 2,

        [Description("Channel 3")]
        CH3 = 3,

        [Description("Channel 4")]
        CH4 = 4,

        [Description("Channel 5")]
        CH5 = 5,

        [Description("Channel 6")]
        CH6 = 6,

        [Description("Channel 7")]
        CH7 = 7,

        [Description("Channel 8")]
        CH8 = 8
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DataFormat
    {
        [Description("Text, 3-Channel, Tab Separated")]
        Esper,

        [Description("Binary 24bit, 2-Channel, CR-LF Separated")]
        Srul
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ResultType
    {
        Real,
        Imaginary,
        Phase,

        [Description("Phase Degrees")]
        PhaseDegrees,

        [Description("Phase Radians")]
        PhaseRadians,

        [Description("Magnitude")]
        Magnitude,

        [Description("Log Magnitude")]
        LogMagnitude,

        [Description("Magnitude Squared")]
        MagnitudeSquared
    }

}
