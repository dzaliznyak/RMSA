using System;

namespace Rmsa.Core.Utils
{
    public class StreamFrameCalculator
    {
        public double SamplesPerUpdate { get; }
        public int DelayBetweenFrames { get; }

        public StreamFrameCalculator(double samplingRateHz, int frameWidth)
        {
            double requiredFps = samplingRateHz / frameWidth;
            double realFps = Math.Max(requiredFps, Defines.DesiredFps);
            SamplesPerUpdate = samplingRateHz / realFps;
            DelayBetweenFrames = (int)(1000.0 / realFps);
            if (DelayBetweenFrames == 0)
                DelayBetweenFrames = 1;
        }

    }
}
