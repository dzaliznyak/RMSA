using DSPLib;
using MvvmCross.ViewModels;
using Rmsa.Core.Graph;
using Rmsa.Core.Utils;
using Rmsa.Transform;
using System;
using System.ComponentModel;
using System.Numerics;

namespace Rmsa.Model
{
    public class Channel : MvxViewModel
    {
        InputData _data;
        ResultData _result;
        string _errorMessage;

        public ChannelSettings Settings { get; }
        public string Name => Settings.ChannelNo.ToString();
        public DataChannelNo ChannelNo => Settings.ChannelNo;

        public InputData Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public ResultData Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string BaseColor => ChannelDrawStyle.BaseColor(ChannelNo);


        public Channel(ChannelSettings settings)
        {
            Settings = settings;
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChannelSettings.IsVisible))
            {
                RaisePropertyChanged(e.PropertyName);
            }
        }

        internal void SetData(InputData data)
        {
            try
            {
                Data = data;
                Recalculate();
                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{Name}: {ex.Message}";
                throw new ChannelException(ErrorMessage);
            }
        }

        internal void RefreshData()
        {
            try
            {
                if (_data != null)
                {
                    Recalculate();
                    ErrorMessage = null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{Name}: {ex.Message}";
                throw new ChannelException(ErrorMessage);
            }
        }

        void Recalculate()
        {
            // get window data
            var data = _data.GetWindowData(Settings.InputGraphSettings.WindowPosition, Settings.InputGraphSettings.WindowWidth);

            // Apply window to the time series data
            double[] wc = DSP.Window.Coefficients(Settings.ResultGraphSettings.WindowType, (uint)data.Length);
            double windowScaleFactor = DSP.Window.ScaleFactor.Signal(wc);
            double[] windowedTimeSeries = DSP.Math.Multiply(data, wc);

            // transform ------------------------------
            Complex[] complexResult = Transform(windowedTimeSeries, _data.SamplingRateHz, out double startX, out double dx);

            // convert to result type
            double[] res = Utils.ComplexToResultType(complexResult, Settings.ResultGraphSettings.ResultType, windowScaleFactor);

            // Calculate the frequency span
            //double[] fSpan = fft.FrequencySpan(_data.FsSamplingRateHz);
            //Result = new ResultData(res, fSpan[0], fSpan[1] - fSpan[0]);

            Result = new ResultData(res, startX, dx, _data.ChannelNo);
        }

        Complex[] Transform(double[] data, double samplingRateHz, out double startX, out double dx)
        {
            Complex[] complexResult;
            startX = 0;
            dx = 1;

            if (Settings.ResultGraphSettings.Algorithm == TransformAlgorithm.FFT)
            {
                FFT fft = new FFT();
                fft.Initialize((uint)data.Length, Settings.ResultGraphSettings.ZeroPadding);
                complexResult = fft.Execute(data);
                double[] fSpan = fft.FrequencySpan(samplingRateHz, out startX, out dx);
            }
            else if (Settings.ResultGraphSettings.Algorithm == TransformAlgorithm.DFT)
            {
                Complex[] input = Utils.RToComplex(data);
                complexResult = Dft.Transform(input);
            }
            else if (Settings.ResultGraphSettings.Algorithm == TransformAlgorithm.NOP)
            {
                complexResult = Utils.RToComplex(data);
            }
            else
            {
                throw new Exception("Invalid algorithm");
            }

            return complexResult;
        }


    }
}
