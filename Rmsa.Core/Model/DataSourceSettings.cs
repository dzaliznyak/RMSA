using System.Text.Json.Serialization;

namespace Rmsa.Model
{
    public class DataSourceSettings
    {
        // general settings
        public DataSourceType DataSourceType { get; set; } = DataSourceType.Generator;
        public DataFormat DataFormat { get; set; } = DataFormat.Srul;
        public DataMode DataMode { get; set; } = DataMode.All;
        public int FrameWidth { get; set; } = 1024;
        public double SamplingRateHz { get; set; } = 1048576;


        // COM port settings
        public string ComPortName { get; set; } = "COM1";
        public int ComPortBaudRate { get; set; } = 115200;


        // file settings
        public string FileName { get; set; }


        // signal generator settings
        public double InputFrequencyHz { get; set; } = 65536;
        public double AmplitudeVrms { get; set; } = 100.0;
        public double DcOffsetV { get; set; } = 0.0;

        [JsonIgnore]
        public bool IsBinaryDataSource => DataFormat == DataFormat.Srul;
    }
}
