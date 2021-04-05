namespace Rmsa.Core.Graph
{
    public class Margin
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public double Width => Left + Right;
        public double Height => Top + Bottom;

        public Margin(double left, double  top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}