using Rmsa.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Rmsa.Core.DataSource
{
    public class ComConnection : IDataSource
    {
        readonly object _locker = new object();
        readonly FileStream _logFile;
        readonly DataSourceSettings _settings;
        readonly IDataParser _dataParser;
        readonly int _delayBetweenFrames;

        List<List<double>> _data = new List<List<double>>();
        SerialPort _port;

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<string> Error;


        public ComConnection(DataSourceSettings settings, IDataParser dataParser)
        {
            _settings = settings;
            _dataParser = dataParser;

            _delayBetweenFrames = (int)(_settings.FrameWidth / _settings.SamplingRateHz * 1000);

            string logFileName = string.Format(Defines.LogFileName, DateTime.Now);
            _logFile = new FileStream(logFileName, FileMode.Create, FileAccess.Write);
        }

        public void Connect()
        {
            lock (_locker)
            {
                _port = new SerialPort
                {
                    PortName = _settings.ComPortName,
                    BaudRate = _settings.ComPortBaudRate,
                };

                _port.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                _port.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceived);

                _port.Open();
            }
        }

        public void Disconnect()
        {
            lock (_locker)
            {
                if (_port != null)
                {
                    if (_port.IsOpen)
                        _port.Close();

                    _port.DataReceived -= new SerialDataReceivedEventHandler(OnDataReceived);
                    _port.ErrorReceived -= new SerialErrorReceivedEventHandler(OnErrorReceived);

                    _port = null;
                }
            }
        }

        void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Error?.Invoke(this, e.ToString());
        }

        async void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int MAX_BYTES = 256;

                while (_port.BytesToRead > 0)
                {
                    int bytesToRead = _port.BytesToRead <= MAX_BYTES ? _port.BytesToRead : MAX_BYTES;
                    
                    var buf = new byte[bytesToRead];
                    int actuallyRead = _port.Read(buf, 0, bytesToRead);

                    if (actuallyRead == bytesToRead)
                    {
                        _logFile.Write(buf, 0, buf.Length);

                        if (_dataParser.ParseBuffer(_data, buf))
                        {
                            DataReceived?.Invoke(this, new DataReceivedEventArgs(_data, _settings.SamplingRateHz));
                            _data = new List<List<double>>();
                            await Task.Delay(_delayBetweenFrames);
                        }
                    }
                    else
                    {
                        Trace.WriteLine($"actuallyRead != bytesToRead ({actuallyRead} != {bytesToRead})");
                    }
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, ex.Message);
            }
        }

    }
}
