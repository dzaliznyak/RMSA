using Rmsa.Core.Graph;
using Rmsa.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rmsa.Controls
{
    public partial class DisplayControl : UserControl
    {
        Point? _dragStart;
        SKMatrix _startMatrix;
        double _windowStartPosition;

        DisplayViewModel ViewModel;

        public DisplayControl()
        {
            InitializeComponent();
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as DisplayViewModel;

            if (ViewModel != null)
            {
                ViewModel.Display.DataChanged += Display_DataChanged;
            }

            Dispatcher.Invoke(() =>
            {
                inputCanvas.InvalidateVisual();
                resultCanvas.InvalidateVisual();
            });
        }

        void Display_DataChanged(object sender, EventArgs e)
        {
            //todo - skip frames 
            Dispatcher.Invoke(() =>
            {
                inputCanvas.InvalidateVisual();
                resultCanvas.InvalidateVisual();
            });
        }

        void InputCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (ViewModel == null)
                return;

            e.Surface.Canvas.Clear(Defines.DisplayBackgroundColor);

            foreach (var channel in ViewModel.Display.Channels)
                InputPlotter.Draw(
                    isActive: ViewModel.Settings.ActiveChannel == channel.ChannelNo,
                    ChannelDrawStyle.FromChannelNo(channel.ChannelNo),
                    channel.Data,
                    paintSurface: e,
                    ViewModel.Settings,
                    channel.Settings,
                    ViewModel.Settings.InputGraphMargin);
        }

        void ResultCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (ViewModel == null)
                return;

            e.Surface.Canvas.Clear(Defines.DisplayBackgroundColor);

            foreach (var channel in ViewModel.Display.Channels)
                ResultPlotter.Draw(
                    isActive: ViewModel.Settings.ActiveChannel == channel.ChannelNo,
                    ChannelDrawStyle.FromChannelNo(channel.ChannelNo),
                    channel.Result,
                    paintSurface: e,
                    ViewModel.Settings,
                    channel.Settings,
                    ViewModel.Settings.ResultGraphMargin);
        }

        void InputCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double zoom = 1 + (double)e.Delta / 1000;

                var channel = ViewModel.Display.GetChannel(ViewModel.Settings.ActiveChannel);
                channel.Settings.InputGraphSettings.ZoomY *= zoom;
            }
            else if (Keyboard.Modifiers == ModifierKeys.None)
            {
                float scaleFactor = (float)(1 + (double)e.Delta / 1000);
                var pos = Mouse.GetPosition((UIElement)sender);
               
                var curMatrix = SKMatrix.CreateScale(scaleFactor, scaleFactor, (float)pos.X, (float)pos.Y);
                ViewModel.Settings.Matrix = SKMatrix.Concat(curMatrix, ViewModel.Settings.Matrix);
            }
            ViewModel.Display.RefreshData();
        }

        void InputCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            element.CaptureMouse();
            _dragStart = e.GetPosition(element);

            _startMatrix = ViewModel.Settings.Matrix;

            var channel = ViewModel.Display.GetChannel(ViewModel.Settings.ActiveChannel);
            _windowStartPosition = channel.Settings.InputGraphSettings.WindowPosition;
        }

        void InputCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            element.ReleaseMouseCapture();
            _dragStart = null;
        }

        void InputCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStart == null)
                return;

            var element = (SKElement)sender;
            var position = e.GetPosition(element);

            if (Keyboard.Modifiers == ModifierKeys.None && e.LeftButton == MouseButtonState.Pressed)
            {
                // dragging the entire screen
                var offsetX = position.X - _dragStart.Value.X;
                var offsetY = position.Y - _dragStart.Value.Y;

                var curMatrix = SKMatrix.CreateTranslation((float)offsetX, (float)offsetY);
                ViewModel.Settings.Matrix = SKMatrix.Concat(curMatrix, _startMatrix);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control || e.RightButton == MouseButtonState.Pressed)
            {
                // moving the data window
                var marginWidth = ViewModel.Settings.InputGraphMargin.Width;

                var pixelOffset = position.X - _dragStart.Value.X;
                var dx = pixelOffset / (element.ActualWidth - marginWidth);

                var channel = ViewModel.Display.GetChannel(ViewModel.Settings.ActiveChannel);

                var newPosition = _windowStartPosition + dx;
                if (newPosition < 0)
                    newPosition = 0;
                else if (newPosition > 1.0)
                    newPosition = 1.0;
                channel.Settings.InputGraphSettings.WindowPosition = newPosition;
            }

            ViewModel.Display.RefreshData();
        }

    }
}
