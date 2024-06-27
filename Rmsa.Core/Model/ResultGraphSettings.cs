using DSPLib;

namespace Rmsa.Model
{
    public class ResultGraphSettings
    {
        public WindowType WindowType { get; set; } = WindowType.None;
        public ResultType ResultType { get; set; } = ResultType.LogMagnitude;
        public TransformAlgorithm Algorithm { get; set; } = TransformAlgorithm.NOP;
        public uint ZeroPadding { get; set; } = 0;
    }
}
