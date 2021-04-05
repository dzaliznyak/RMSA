using SkiaSharp;

namespace Rmsa.Core.Graph
{
    public class ChannelDrawStyle
    {
        static readonly ChannelDrawStyle[] _arr = new ChannelDrawStyle[8];
        static readonly SKColor[] _baseColors = new SKColor[8]
        {
            SKColors.Yellow,
            SKColors.Cyan,
            SKColors.Fuchsia,
            SKColors.Orange,
            SKColors.LightGreen,
            SKColors.Magenta,
            SKColors.LightBlue,
            SKColors.AliceBlue,
        };

        public SKPaint ChannelPaint;
        public SKPaint RulerPaint;
        public SKPaint WindowPaint;
        public SKPaint RulerXTextDrawStyle;
        public SKPaint RulerYTextDrawStyle;

        public ChannelDrawStyle(SKColor baseColor)
        {
            RulerXTextDrawStyle = new SKPaint
            {
                TextSize = 12,
                TextAlign = SKTextAlign.Center,
                Color = baseColor,
            };

            RulerYTextDrawStyle = new SKPaint
            {
                TextSize = 12,
                TextAlign = SKTextAlign.Right,
                Color = baseColor,
            };

            RulerPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White,
                StrokeWidth = 1,
                IsAntialias = true,
            };

            WindowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = baseColor,
                StrokeWidth = 0,
                IsAntialias = false,
                //PathEffect = SKPathEffect.CreateDash(new float[] { 5, 5 }, 1)
            };

            ChannelPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = baseColor,
                StrokeWidth = 0,
                IsAntialias = true,
            };
        }

        public static ChannelDrawStyle FromChannelNo(DataChannelNo channelNo)
        {
            int i = (int)channelNo - 1;
            if (_arr[i] == null)
            {
                _arr[i] = new ChannelDrawStyle(_baseColors[i]);
            }
            return _arr[i];
        }

        public static string BaseColor(DataChannelNo channelNo)
        {
            int i = (int)channelNo - 1;
            return _baseColors[i].ToString();
        }
    }
}
