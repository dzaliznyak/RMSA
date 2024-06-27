using Rmsa.Core.Utils;
using Rmsa.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Rmsa.Core.DataSource
{
    public class SerialTextDataSource : IDataSource
    {
        readonly object _locker = new();
        //readonly FileStream _logFile;
        readonly DataSourceSettings _settings;
        readonly IDataParser _dataParser;
        //readonly int _delayBetweenFrames;
        readonly List<CircularBuffer> _data;

        //List<List<double>> _data = new();
        SerialPort _port;
        private bool _continue;

        //public event EventHandler<DataReceivedEventArgs> DataReceived;
        //public event EventHandler<string> Error;

        readonly Thread _readThread = new Thread(Read);

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<string> Error;

        public SerialTextDataSource(DataSourceSettings settings, IDataParser dataParser)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _dataParser = dataParser ?? throw new ArgumentNullException(nameof(dataParser));

            _data = new List<CircularBuffer>
            {
                new CircularBuffer(32),
                new CircularBuffer(32),
                new CircularBuffer(32),
                new CircularBuffer(32),
                new CircularBuffer(32),
                new CircularBuffer(32),
                new CircularBuffer(32),
                new CircularBuffer(32)
            };
        }

        public void Connect()
        {
            lock (_locker)
            {
                _port = new SerialPort
                {
                    PortName = _settings.ComPortName,
                    BaudRate = _settings.ComPortBaudRate,
                    ReadTimeout = 1000
                };

                _port.Open();
                _continue = true;
                _readThread.Start(this);
            }
        }

        public void Disconnect()
        {
            lock (_locker)
            {
                _continue = false;

                if (_port != null)
                {
                    if (_port.IsOpen)
                        _port.Close();

                    _port = null;
                }

                _readThread.Join();
            }
        }

        public static void Read(object prm)
        {
            SerialTextDataSource _this = (SerialTextDataSource)prm;
            while (_this._continue)
            {
                try
                {
                    string str = _this._port.ReadLine();
                    Trace.WriteLine(str);

                    if (_this._dataParser.ParseRecord(_this._data, str))
                    {
                        var outData = new List<List<double>>();
                        foreach (var item in _this._data)
                            outData.Add(item.ToList());

                        _this.DataReceived?.Invoke(_this, new DataReceivedEventArgs(outData, _this._settings.SamplingRateHz));
                    }
                }
                catch (Exception) 
                { 
                }
            }
        }

        //void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        //int MAX_BYTES = 1024;

        //        while (true/*_port.BytesToRead > 0*/)
        //        {
        //            //int bytesToRead = _port.BytesToRead <= MAX_BYTES ? _port.BytesToRead : MAX_BYTES;

        //            //var buf = new byte[bytesToRead];
        //            //int actuallyRead = _port.ReadLine(buf, 0, bytesToRead);

        //            var str = _port.ReadLine();

        //            //if (actuallyRead == bytesToRead)
        //            //{
        //            //_logFile.Write(buf, 0, buf.Length);
        //            //var str = Encoding.ASCII.GetString(buf);
        //            Trace.WriteLine($"data: {str}");

        //            if (_dataParser.ParseRecord(_data, str))
        //            {
        //                var outData = new List<List<double>>();
        //                foreach (var item in _data)
        //                    outData.Add(item.ToList());

        //                DataReceived?.Invoke(this, new DataReceivedEventArgs(outData, _settings.SamplingRateHz));
        //            }
        //            //}
        //            //else
        //            //{
        //            //    Trace.WriteLine($"actuallyRead != bytesToRead ({actuallyRead} != {bytesToRead})");
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Error?.Invoke(this, ex.Message);
        //    }
        //}

    }
}
