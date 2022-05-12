using Rmsa.Core.Utils;
using Rmsa.Model;
using System;
using System.Collections.Generic;

namespace Rmsa.Core.DataSource.DataParser
{
    public class SrulDataParser : IDataParser
    {
        enum State
        {
            WaitStart,
            WaitEnd
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        readonly DataSourceSettings _settings;
        readonly byte[] _resultBuf = new byte[1024 + 512 + 6];

        State _state = State.WaitStart;
        int _start;
        int _resultBufIndex = 0;

        int RemainedBytes => _resultBuf.Length - _resultBufIndex;


        public SrulDataParser(DataSourceSettings settings)
        {
            _settings = settings;
        }

        public bool ParseRecord(List<List<double>> data, string record)
        {
            throw new NotSupportedException();
        }

        public bool ParseRecord(List<CircularBuffer> data, string record)
        {
            throw new NotSupportedException();
        }

        public bool ParseBuffer(List<List<double>> data, byte[] inBuf)
        {
            if (data.Count == 0)
            {
                // 2 channels
                data.Add(new List<double>());
                data.Add(new List<double>());
            }

            ProcessBytes(data, inBuf, 0);

            return data[0].Count > 0 && data[1].Count > 0;
        }

        void ProcessBytes(List<List<double>> data, byte[] inBuf, int fromPosition)
        {
            switch (_state)
            {
                case State.WaitStart:
                    _start = FindStart(inBuf, 0);
                    if (_start != -1)
                    {
                        _state = State.WaitEnd;
                        ProcessBytes(data, inBuf, _start);
                    }
                    break;
                case State.WaitEnd:
                    if (inBuf.Length - fromPosition < RemainedBytes)
                    {
                        // take all buffer until the end
                        int length = inBuf.Length - fromPosition;
                        Buffer.BlockCopy(inBuf, fromPosition, _resultBuf, _resultBufIndex, length);
                        _resultBufIndex += length;
                    }
                    else
                    {
                        int length = RemainedBytes;
                        Buffer.BlockCopy(inBuf, fromPosition, _resultBuf, _resultBufIndex, length);
                        OnBufferCompleated(data);
                        _state = State.WaitStart;
                        ProcessBytes(data, inBuf, fromPosition + length);
                    }
                    break;
                default:
                    break;
            }
        }

        void OnBufferCompleated(List<List<double>> data)
        {
            int channelIndex = (char)_resultBuf[1538] == 'L' ? 0 : 1;

            var size = _resultBuf.Length / 3; // 24bit integers
            for (var index = 0; index < size - 2; index++)
            {
                data[channelIndex].Add(((sbyte)_resultBuf[index * 3 + 0] < 0 ? unchecked((int)0xff000000) : 0x00000000) |
                              _resultBuf[index * 3 + 0] << 16 |
                              _resultBuf[index * 3 + 1] << 8 |
                              _resultBuf[index * 3 + 2]);
            }

            //data[channelIndex].Add(0);
            //data[channelIndex].Add(0);

            _resultBufIndex = 0;
            Array.Clear(_resultBuf, 0, _resultBuf.Length);
        }

        static int FindStart(byte[] buf, int fromPosition)
        {
            for (int i = fromPosition; i < buf.Length; i++)
            {
                //todo
                if (buf[i] == 0x0D && buf[i + 1] == 0x0A)
                    return i + 2;
            }
            return -1;
        }

    }
}
