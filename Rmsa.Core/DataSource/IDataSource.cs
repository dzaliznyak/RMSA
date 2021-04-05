using System;
using System.Collections.Generic;

namespace Rmsa.Core.DataSource
{
    public class DataReceivedEventArgs : EventArgs
    {
        public DataReceivedEventArgs(List<List<double>> data, double samplingRateHz)
            : base()
        {
            Data = data;
            SamplingRateHz = samplingRateHz;
        }

        public List<List<double>> Data { get; private set; }
        public double SamplingRateHz { get; private set; }
    }

    public interface IDataSource
    {
        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler<string> Error;

        void Connect();
        void Disconnect();
    }
}