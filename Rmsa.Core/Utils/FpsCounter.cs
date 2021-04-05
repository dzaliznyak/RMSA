using System;

namespace Rmsa.Core.Utils
{
    public class FpsCounter
    {
        readonly CircularBuffer _lastFps;
        DateTime _prevDataTimestamp = DateTime.MinValue;

        public FpsCounter(int size = 20)
        {
            _lastFps = new CircularBuffer(size);
        }

        public void Reset()
        {
            _lastFps.Clear();
            _prevDataTimestamp = DateTime.MinValue;
        }

        public void Update()
        {
            var now = DateTime.Now;
            var fps = 1000 / (now - _prevDataTimestamp).TotalMilliseconds;
            _prevDataTimestamp = now;
            _lastFps.Put(fps);
        }

        public double Avg()
        {
            return _lastFps.Avg();
        }
    }
}
