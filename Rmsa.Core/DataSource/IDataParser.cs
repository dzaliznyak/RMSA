using Rmsa.Core.Utils;
using System.Collections.Generic;

namespace Rmsa.Core.DataSource
{
    public interface IDataParser
    {
        bool ParseRecord(List<CircularBuffer> data, string record);
        bool ParseRecord(List<List<double>> data, string record);
        bool ParseBuffer(List<List<double>> data, byte[] inBuf);
    }
}