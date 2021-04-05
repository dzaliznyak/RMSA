using Rmsa.Core.Utils;
using Rmsa.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Rmsa.Core.DataSource
{
    public class FileDataSource : IDataSource
    {
        readonly DataSourceSettings _settings;
        readonly IDataParser _dataParser;
        
        bool _stopped;

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<string> Error;

        public FileDataSource(DataSourceSettings settings, IDataParser dataParser)
        {
            _settings = settings;
            _dataParser = dataParser;
        }

        public void Connect()
        {
            Task.Run(async () => 
            {
                _stopped = false;

                try
                {
                    if (_settings.DataMode == DataMode.All)
                    {
                        await StartAsAll();
                    }
                    else
                    {
                        while (!_stopped)
                        {
                            if (_settings.DataMode == DataMode.Stream)
                            {
                                if (_settings.IsBinaryDataSource)
                                    await StartAsBinaryStream();
                                else
                                    await StartAsTextStream();
                            }
                            else if (_settings.DataMode == DataMode.Frame)
                            {
                                if (_settings.IsBinaryDataSource)
                                    await StartAsBinaryFrame();
                                else
                                    await StartAsTextFrame();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, ex.Message);
                }
            });
        }

        public void Disconnect()
        {
            _stopped = true;
        }

        Task StartAsAll()
        {
            List<List<double>> data = ReadAll();

            DataReceived?.Invoke(this, new DataReceivedEventArgs(data, _settings.SamplingRateHz));

            return Task.CompletedTask;
        }

        Task StartAsBinaryStream()
        {
            throw new NotImplementedException();
        }

        async Task StartAsTextFrame()
        {
            var tempData = new List<List<double>>();
            List<List<double>> data = null;

            int delayBetweenFrames = (int)(_settings.FrameWidth / _settings.SamplingRateHz * 1000);
            int samplesCounter = 0;

            string[] records = File.ReadAllLines(_settings.FileName);
            foreach (var line in records)
            {
                if (!_dataParser.ParseRecord(tempData, line))
                    continue;

                if (data == null)
                {
                    data = new List<List<double>>();
                    for (int i = 0; i < tempData.Count; i++)
                        data.Add(new List<double>());
                }

                for (int i = 0; i < tempData.Count; i++)
                {
                    data[i].Add(tempData[i][0]);
                    tempData[i].Clear();
                }

                if (++samplesCounter >= _settings.FrameWidth)
                {
                    samplesCounter = 0;

                    DataReceived?.Invoke(this, new DataReceivedEventArgs(data, _settings.SamplingRateHz));

                    data = new List<List<double>>();
                    for (int i = 0; i < tempData.Count; i++)
                        data.Add(new List<double>());

                    await Task.Delay(delayBetweenFrames);
                }
            }
        }

        async Task StartAsBinaryFrame()
        {
            const int BufLen = 128;

            byte[] fileBytes = File.ReadAllBytes(_settings.FileName);
            var data = new List<List<double>>();

            int delayBetweenFrames = (int)(_settings.FrameWidth / _settings.SamplingRateHz * 1000);

            byte[] copy = new byte[BufLen];
            int index = 0;
            while (index < fileBytes.Length - BufLen)
            {
                Array.Copy(fileBytes, index, copy, 0, BufLen);

                if (_dataParser.ParseBuffer(data, copy))
                {
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(data, _settings.SamplingRateHz));
                    data = new List<List<double>>();
                    await Task.Delay(delayBetweenFrames);
                }

                index += BufLen;
            }
        }

        public List<List<double>> ReadAll()
        {
            const int BufLen = 128;

            var data = new List<List<double>>();

            if (_settings.IsBinaryDataSource)
            {
                byte[] fileBytes = File.ReadAllBytes(_settings.FileName);

                byte[] copy = new byte[BufLen];
                int index = 0;
                while (index < fileBytes.Length - BufLen)
                {
                    Array.Copy(fileBytes, index, copy, 0, BufLen);

                    _dataParser.ParseBuffer(data, copy);

                    index += BufLen;
                }
            }
            else
            {
                string[] records = File.ReadAllLines(_settings.FileName);
                foreach (var item in records)
                {
                    _dataParser.ParseRecord(data, item);
                }
            }

            return data;
        }

        async Task StartAsTextStream()
        {
            string[] records = File.ReadAllLines(_settings.FileName);
            var calc = new StreamFrameCalculator(_settings.SamplingRateHz, _settings.FrameWidth);
            var data = new List<CircularBuffer>();

            int samplesCounter = 0;
            foreach (var line in records)
            {
                if (!_dataParser.ParseRecord(data, line))
                    continue;

                // skip update if not all samples collected
                if (++samplesCounter >= calc.SamplesPerUpdate)
                {
                    samplesCounter = 0;

                    var outData = new List<List<double>>();
                    foreach (var item in data)
                        outData.Add(item.ToList());

                    DataReceived?.Invoke(this, new DataReceivedEventArgs(outData, _settings.SamplingRateHz));

                    await Task.Delay(calc.DelayBetweenFrames);
                }
            }
        }

    }
}
