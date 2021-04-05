using DSPLib;
using Rmsa.Core.Utils;
using Rmsa.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rmsa.Core.DataSource
{
    public class GeneratorDataSource : IDataSource
    {
        readonly DataSourceSettings _settings;
        readonly double[] _timeSeries;
        bool _stopped;

        public double SamplingRateHz => _settings.SamplingRateHz;

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<string> Error;

        public GeneratorDataSource(DataSourceSettings settings)
        {
            _settings = settings;

            uint points = (uint)_settings.FrameWidth;
            if (settings.DataMode == DataMode.Stream)
                points = (uint)_settings.SamplingRateHz * 10; // 10 seconds of data

            _timeSeries = DSP.Generate.ToneSampling(
                amplitudeVrms: _settings.AmplitudeVrms,
                frequencyHz: _settings.InputFrequencyHz,
                samplingFrequencyHz: _settings.SamplingRateHz,
                points,
                dcV: _settings.DcOffsetV);
        }

        public void Connect()
        {
            _stopped = false;

            Task.Run(async () =>
            {
                try
                {
                    while (!_stopped)
                    {
                        if (_settings.DataMode == DataMode.Stream)
                        {
                            await StartAsStream();
                        }
                        else if (_settings.DataMode == DataMode.Frame)
                        {
                            await StartAsFrame();
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

        async Task StartAsFrame()
        {
            var data = new List<List<double>>
            {
                new List<double>(_timeSeries)
            };
            var args = new DataReceivedEventArgs(data, SamplingRateHz);

            DataReceived?.Invoke(this, args);

            int delayBetweenFrames = (int)(_settings.FrameWidth / _settings.SamplingRateHz * 1000);
            await Task.Delay(delayBetweenFrames);
        }

        async Task StartAsStream()
        {
            var calc = new StreamFrameCalculator(_settings.SamplingRateHz, _settings.FrameWidth);
            var data = new List<CircularBuffer>
            {
                new CircularBuffer(_settings.FrameWidth)
            };

            int samplesCounter = 0;
            foreach (var val in _timeSeries)
            {
                data[0].Put(val);

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
