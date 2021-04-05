namespace Rmsa.Core.Graph
{
    public class ResultData
    {
        public DataChannelNo ChannelNo { get; }
        public double XStart { get; }
        public double XEnd { get; }
        public double DX { get; }
        public double[] Data { get; }

        public ResultData(double[] data, double xStart, double dx, DataChannelNo channelNo)
        {
            XStart = xStart;
            DX = dx;
            XEnd = xStart + dx * data.Length;
            Data = data;
            ChannelNo = channelNo;
        }
    }
}
