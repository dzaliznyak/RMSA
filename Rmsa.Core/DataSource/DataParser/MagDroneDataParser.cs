using Rmsa.Core.Utils;
using Rmsa.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rmsa.Core.DataSource.DataParser
{
    public class MagDroneDataParser : IDataParser
    {
        readonly DataSourceSettings _settings;

        public MagDroneDataParser(DataSourceSettings settings)
        {
            _settings = settings;
        }

        public bool ParseRecord(List<CircularBuffer> data, string record)
        {
            int sensor = 0;

            if (record.StartsWith("$MDBM"))
            {
                try
                {
                    var parts = record.Split(',');
                    if (parts.Length == 7 && !string.IsNullOrEmpty(parts[6]))
                    {
                        sensor = parts[0].EndsWith("1") ? 1 : 2;

                        var x = Convert.ToDouble(parts[4]);
                        var y = Convert.ToDouble(parts[5]);
                        var z = Convert.ToDouble(parts[6]);
                        var t = Math.Sqrt(x * x + y * y + z * z);

                        if (sensor == 1)
                        {
                            data[0].Put(x);
                            data[1].Put(y);
                            data[2].Put(z);
                            data[3].Put(t);
                        }
                        else if (sensor == 2)
                        {
                            data[4].Put(x);
                            data[5].Put(y);
                            data[6].Put(z);
                            data[7].Put(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"Data parse exception: {record}, {ex.Message}");
                }
            }

            return sensor == 2;
        }

        public bool ParseBuffer(List<List<double>> data, byte[] inBuf)
        {
            throw new NotImplementedException();
        }

        public bool ParseRecord(List<List<double>> data, string record)
        {
            throw new NotImplementedException();
        }
    }
}
