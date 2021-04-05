using System;
using System.Collections.Generic;
using System.Linq;

namespace Rmsa.Core.Graph
{
    public class InputData
    {
        public DataChannelNo ChannelNo { get; }
        public int SampleCount { get; }
        public double XStart { get; }
        public double XEnd { get; }
        public double DX { get; }
        public double MaxAbsY { get; private set; }
        public List<double> Data { get; private set; }
        public double SamplingRateHz { get; private set; }
        public double Fps { get; }

        public InputData(List<double> data, DataChannelNo channelNo, double samplingRateHz, double fps)
        {
            Data = data;
            ChannelNo = channelNo;
            SamplingRateHz = samplingRateHz;
            Fps = fps;
            SampleCount = data.Count;
            XStart = 0;
            XEnd = data.Count;
            DX = (XEnd - XStart) / (SampleCount - 1);

            if (data.Count > 0)
                MaxAbsY = Data.Max(t => Math.Abs(t));
        }

        internal double[] GetWindowData(double windowPosition, int windowWidth)
        {
            int windowStartPosition = (int)(Data.Count * windowPosition);
            int len = windowWidth < Data.Count ? windowWidth : Data.Count;

            if (windowStartPosition > Data.Count - len)
                windowStartPosition = Data.Count - len;

            if (windowStartPosition < 0)
                windowStartPosition = 0;

            var windowData = new double[len];

            Data.CopyTo(windowStartPosition, windowData, 0, len);

            return windowData;
        }

    }
}
