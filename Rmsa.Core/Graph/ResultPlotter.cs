using Rmsa.Model;
using Rmsa.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;

namespace Rmsa.Core.Graph
{
    public enum ResultComponent
    {
        Real,
        Imaginary,
        Phase,
        Magnitude,
        MagnitudeDbv
    }

    public class ResultPlotter
    {
        class DrawInfo
        {
            public ChannelDrawStyle Style { get; internal set; }
            public int Width { get; internal set; }
            public int Height { get; internal set; }
            public double ZoomX { get; internal set; }
            public double ZoomY { get; internal set; }
            public SKCanvas Canvas { get; internal set; }
            public double[] Data { get; internal set; }
            public Margin Margin { get; internal set; }
            public double MaxAbsY { get; internal set; }
            public double XEnd { get; internal set; }
            public double XStart { get; internal set; }
            public double DX { get; internal set; }
            public bool IsActive { get; internal set; }
        }

        public static void Draw(bool isActive, ChannelDrawStyle channelDrawStyle, ResultData data, SKPaintSurfaceEventArgs paintSurface,
                                DisplayRuntimeState state, ChannelSettings channelSettings, Margin margin)
        {
            if (data == null || data.Data == null || data.Data.Length == 0)
                return;

            SKSurface surface = paintSurface.Surface;
            SKCanvas canvas = surface.Canvas;

            int width = paintSurface.Info.Width - (int)margin.Width;
            int height = paintSurface.Info.Height - (int)margin.Height;

            double maxAbsY = 0;

            if (data != null && channelSettings.IsVisible)
            {
                for (int i = 0; i < data.Data.Length; i++)
                {
                    if (Math.Abs(data.Data[i]) > maxAbsY)
                        maxAbsY = Math.Abs(data.Data[i]);
                }

                DrawInfo di = new()
                {
                    Style = channelDrawStyle,
                    IsActive = isActive,
                    Data = data.Data,
                    Canvas = canvas,
                    Margin = margin,
                    Width = width,
                    Height = height,
                    MaxAbsY = maxAbsY,
                    ZoomX = width / data.XEnd,
                    XStart = data.XStart,
                    XEnd = data.XEnd,
                    DX = data.DX,
                    ZoomY = height / 2 / maxAbsY,
                };

                DrawAxis(di);
                DrawGraph(data, di);
            }
        }

        static void DrawAxis(DrawInfo di)
        {
            // x - axis
            di.Canvas.DrawLine(X(0, di), Y(0, di), X(di.XEnd, di), Y(0, di), di.Style.RulerPaint);
            for (int i = 0; i < 8; i++)
            {
                double x = i * di.XEnd / 8;
                di.Canvas.DrawLine(X(x, di), Y_(-5, di), X(x, di), Y_(5, di), di.Style.RulerPaint);
                if (di.IsActive)
                    di.Canvas.DrawText($"{x}", X(x, di), Y_(-25, di), di.Style.RulerXTextDrawStyle);
            }

            // y - axis
            di.Canvas.DrawLine(X(0, di), Y(0, di), X(0, di), Y(di.MaxAbsY, di), di.Style.RulerPaint);
            double dy = di.MaxAbsY / 5;
            double y = 0.0;
            for (int i = 0; i <= 5; i++)
            {
                di.Canvas.DrawLine(X_(-5, di), Y(y, di), X_(5, di), Y(y, di), di.Style.RulerPaint);
                if (i > 0 && di.IsActive)
                {
                    di.Canvas.DrawText($"{y:N0}", X_(-10, di), Y(y, di) + di.Style.RulerYTextDrawStyle.TextSize / 2, di.Style.RulerYTextDrawStyle);
                }
                y += dy;
            }
        }

        static void DrawGraph(ResultData data, DrawInfo di)
        {
            double x0 = di.XStart;
            double y0 = data.Data[0];
            double x1;
            double y1;
            double dx = di.DX;

            for (int i = 1; i < data.Data.Length; i++)
            {
                x1 = x0 + dx;
                y1 = data.Data[i];

                di.Canvas.DrawLine(X(x1, di), Y(y1, di), X(x0, di), Y(y0, di), di.Style.ChannelPaint);

                x0 = x1;
                y0 = y1;
            }
        }

        static float X(double x, DrawInfo di)
        {
            return (float)(x * di.ZoomX + di.Margin.Left);
        }

        static float X_(double x, DrawInfo di)
        {
            return (float)(x + di.Margin.Left);
        }

        static float Y(double y, DrawInfo di)
        {
            return (float)(di.Height / 2 - y * di.ZoomY) + (float)di.Margin.Top;
        }

        static float Y_(double y, DrawInfo di)
        {
            return (float)(di.Height / 2 - y) + (float)di.Margin.Top;
        }

    }
}
