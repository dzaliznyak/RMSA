using System;
using System.Collections.Generic;
using System.Linq;

namespace Rmsa.Core.Utils
{
    public class CircularBuffer
    {
        readonly double[] _buf;
        readonly int _size;

        int _currentPosition;

        public int Size => _size;

        public CircularBuffer(int size)
        {
            _size = size;
            _buf = new double[size];
        }

        public void Put(double value)
        {
            _buf[_currentPosition] = value;

            if (++_currentPosition > (_size - 1))
                _currentPosition = 0;
        }

        public IEnumerable<double> Get()
        {
            int index = _currentPosition;
            for (int i = 0; i < _size; i++)
            {
                var value = _buf[index];
                if (++index > (_size - 1))
                    index = 0;

                yield return value;
            }
        }

        public double Avg()
        {
            return _buf.Average();
        }

        internal void Clear()
        {
            Array.Clear(_buf, 0, _buf.Length);
        }

        internal List<double> ToList()
        {
            var copy = new double[_buf.Length];
            Array.Copy(_buf, _currentPosition, copy, 0, length: _buf.Length - _currentPosition);
            Array.Copy(_buf, 0, copy, copy.Length - _currentPosition, length: _currentPosition);
            return new List<double>(copy);
        }
    }
}
