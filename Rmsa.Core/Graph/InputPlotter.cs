using Rmsa.Model;
using Rmsa.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Rmsa.Core.Graph
{
    public class InputPlotter
    {
        class DrawInfo
        {
            public ChannelDrawStyle Style { get; internal set; }
            public double Width { get; internal set; }
            public double Height { get; internal set; }
            public SKCanvas Canvas { get; internal set; }
            public Margin Margin { get; internal set; }
            public double WindowPosition { get; internal set; }
            public int WindowWidth { get; internal set; }
            public double MaxY { get; internal set; }
            public bool IsActive { get; internal set; }
        }

        public static void Draw(bool isActive, ChannelDrawStyle channelDrawStyle, InputData data, SKPaintSurfaceEventArgs paintSurface, 
                           DisplayRuntimeState state, ChannelSettings channelSettings, 
                           Margin margin)
        {
            if (data == null || data.Data == null || data.Data.Count == 0)
                return;

            SKSurface surface = paintSurface.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Save();

            double width = paintSurface.Info.Width - (int)margin.Width;
            double height = paintSurface.Info.Height - (int)margin.Height;

            if (data != null && channelSettings.IsVisible)
            {
                var maxY = state.IsAutoZoom ? data.MaxAbsY : channelSettings.InputGraphSettings.ZoomY;

                DrawInfo di = new DrawInfo
                {
                    Style = channelDrawStyle,
                    IsActive = isActive,
                    Canvas = canvas,
                    Margin = margin,
                    MaxY = maxY,
                    Width = width,
                    Height = height,
                    WindowPosition = channelSettings.InputGraphSettings.WindowPosition,
                    WindowWidth = channelSettings.InputGraphSettings.WindowWidth
                };

                DrawAxis(data, di);

                var translateX = di.Margin.Left;
                var translateY = di.Height / 2 + di.Margin.Top;
                canvas.Translate((float)translateX, (float)translateY);

                var scaleY = height / 2 / maxY;
                var scaleX = (float)(width / data.XEnd);
                canvas.Scale((float)scaleX, (float)scaleY);

                DrawTransformWindow(data, di);
                DrawGraph(data, di);
            }

            canvas.Restore();
        }

        static void DrawTransformWindow(InputData data, DrawInfo di)
        {
            var pos = data.XEnd * di.WindowPosition;
            if (pos > data.XEnd - di.WindowWidth)
                pos = data.XEnd - di.WindowWidth;
            di.Canvas.DrawLine((float)pos, (float)-di.MaxY, (float)pos, (float)di.MaxY, di.Style.WindowPaint);
            di.Canvas.DrawLine((float)(pos + di.WindowWidth), (float)-di.MaxY, (float)(pos + di.WindowWidth), (float)di.MaxY, di.Style.WindowPaint);
        }

        static void DrawAxis(InputData data, DrawInfo di)
        {
            // x - axis
            di.Canvas.DrawLine(X(0, di), Y(0, di), X(di.Width, di), Y(0, di), di.Style.RulerPaint);
            for (int i = 0; i < 8; i++)
            {
                double xPos = i * di.Width / 8;
                double xVal = i * data.XEnd / 8;
                di.Canvas.DrawLine(X(xPos, di), Y(-5, di), X(xPos, di), Y(5, di), di.Style.RulerPaint);
                if (di.IsActive)
                    di.Canvas.DrawText($"{xVal:N0}", X(xPos, di), Y(-25, di), di.Style.RulerXTextDrawStyle);
            }

            // y - axis
            di.Canvas.DrawLine(X(0, di), Y(0, di), X(0, di), Y(di.Height / 2, di), di.Style.RulerPaint);
            double dy = di.Height / 2 / 5;
            double dyVal = di.MaxY / 5;
            double y = 0.0;
            double yVal = 0.0;
            double x = -10;
            for (int i = 0; i <= 5; i++)
            {
                di.Canvas.DrawLine(X(-5, di), Y(y, di), X(5, di), Y(y, di), di.Style.RulerPaint);
                if (i > 0 && di.IsActive)
                {
                    di.Canvas.DrawText($"{yVal:N0}", X(x, di), Y(y, di) + di.Style.RulerYTextDrawStyle.TextSize / 2, di.Style.RulerYTextDrawStyle);
                }
                y += dy;
                yVal += dyVal;
            }

            // FPS
            if (di.IsActive)
            {
                double xPos = di.Width;
                double yPos = -di.Height/2;
                di.Canvas.DrawText($"FPS: {data.Fps:N0}", X(xPos, di), Y(yPos, di), di.Style.RulerYTextDrawStyle);
            }
        }

        static void DrawGraph(InputData data, DrawInfo di)
        {
            double x0 = data.XStart;
            double y0 = data.Data[0];
            double x1;
            double y1;
            double dx = data.DX;

            for (int i = 1; i < data.SampleCount; i++)
            {
                x1 = x0 + dx;
                y1 = data.Data[i];

                di.Canvas.DrawLine((float)x1, (float)-y1, (float)x0, (float)-y0, di.Style.ChannelPaint);

                x0 = x1;
                y0 = y1;
            }
        }

        static float X(double x, DrawInfo di)
        {
            return (float)(x + di.Margin.Left);
        }

        static float Y(double y, DrawInfo di)
        {
            return (float)(di.Height / 2 - y)  + (float)di.Margin.Top;
        }

    }
}
