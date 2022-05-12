using Rmsa.Core.Utils;
using Rmsa.Model;
using System;
using System.Collections.Generic;

namespace Rmsa.Core.DataSource.DataParser
{
    public class EsperDataParser : IDataParser
    {
        readonly DataSourceSettings _settings;

        public EsperDataParser(DataSourceSettings settings)
        {
            _settings = settings;
        }

        public bool ParseBuffer(List<List<double>> data, byte[] inBuf)
        {
            throw new NotSupportedException();
        }

        public bool ParseRecord(List<List<double>> data, string record)
        {
            var values = record.Split(' ', '\t');

            if (values.Length != data.Count)
            {
                int missingChannelCount = values.Length - data.Count;
                for (int i = 0; i < missingChannelCount; i++)
                {
                    data.Add(new List<double>());
                }
            }

            for (int i = 0; i < values.Length; i++)
            {
                data[i].Add(Convert.ToDouble(values[i]));
            }

            return true;
        }

        public bool ParseRecord(List<CircularBuffer> data, string record)
        {
            var values = record.Split(' ', '\t');

            if (values.Length != data.Count)
            {
                int missingChannelCount = values.Length - data.Count;
                for (int i = 0; i < missingChannelCount; i++)
                {
                    data.Add(new CircularBuffer(_settings.FrameWidth));
                }
            }

            for (int i = 0; i < values.Length; i++)
            {
                data[i].Put(Convert.ToDouble(values[i]));
            }

            return true;
        }
    }
}
