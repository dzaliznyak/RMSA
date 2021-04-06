using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Rmsa.Core.DataSource.DataParser;
using Rmsa.Core.Utils;
using Rmsa.Model;
using Rmsa.Services;
using System;
using System.Windows.Input;

namespace Rmsa.ViewModel
{
    public class DataSourceSettingsViewModel : MvxViewModel<DataSourceSettings>
    {
        readonly IMvxNavigationService _navigationService;
        readonly IDataSourceService _dataSourceService;

        public ICommand OnOkCommand { get; }
        public ICommand OnSelectFileCommand { get; }
        public MvxInteraction<InteractionParams> SelectFileInteraction { get; }

        
        string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                RaisePropertyChanged(nameof(IsHasError));
            }
        }

        public bool IsHasError => !string.IsNullOrEmpty(ErrorMessage);
        public bool IsFileParametersVisible => DataSourceType == DataSourceType.File;
        public bool IsComPortParametersVisible => DataSourceType == DataSourceType.ComPort;
        public bool IsGeneratorParametersVisible => DataSourceType == DataSourceType.Generator;
        public bool IsDataFormatVisible => DataSourceType == DataSourceType.File || DataSourceType == DataSourceType.ComPort;

        bool _isFrameWidthChangable;
        public bool IsFrameWidthChangable
        {
            get => _isFrameWidthChangable;
            set => SetProperty(ref _isFrameWidthChangable, value);
        }

        #region General parameters
        DataSourceType _dataSourceType;
        public DataSourceType DataSourceType 
        {
            get => _dataSourceType;
            set
            {
                SetProperty(ref _dataSourceType, value);
                RaisePropertyChanged(nameof(IsFileParametersVisible));
                RaisePropertyChanged(nameof(IsComPortParametersVisible));
                RaisePropertyChanged(nameof(IsGeneratorParametersVisible));
                RaisePropertyChanged(nameof(IsDataFormatVisible));
            }
        }

        DataMode _dataMode;
        public DataMode DataMode
        {
            get => _dataMode;
            set => SetProperty(ref _dataMode, value);
        }

        DataFormat _dataFormat;
        public DataFormat DataFormat
        {
            get => _dataFormat;
            set
            {
                SetProperty(ref _dataFormat, value);
                var helper = new DataParserHelper(_dataFormat);
                IsFrameWidthChangable = !helper.IsFixedFrameWidth;
                if (!IsFrameWidthChangable)
                    FrameWidth = helper.FrameWidth;
            }
        }

        int _frameWidth;
        public int FrameWidth
        {
            get => _frameWidth;
            set => SetProperty(ref _frameWidth, value);
        }

        double _samplingRateHz;
        public double SamplingRateHz
        {
            get => _samplingRateHz;
            set => SetProperty(ref _samplingRateHz, value);
        }
        #endregion General parameters

        // COM port settings
        public string ComPortName { get; set; }
        public int ComPortBaudRate { get; set; }


        // file settings
        string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }


        // signal generator settings
        public double InputFrequencyHz { get; set; }
        public double AmplitudeVrms { get; set; }
        public double DcOffsetV { get; set; }


        public DataSourceSettingsViewModel(IMvxNavigationService navigationService, IDataSourceService dataSourceService)
        {
            _navigationService = navigationService;
            _dataSourceService = dataSourceService;

            OnOkCommand = new MvxCommand(OnOk);
            OnSelectFileCommand = new MvxCommand(() => OnSelectFile());
            SelectFileInteraction = new MvxInteraction<InteractionParams>();
        }

        public override void Prepare(DataSourceSettings settings)
        {
            // General parameters
            DataSourceType = settings.DataSourceType;
            DataMode = settings.DataMode;
            DataFormat = settings.DataFormat;
            FrameWidth = settings.FrameWidth;
            SamplingRateHz = settings.SamplingRateHz;

            // COM port
            ComPortName = settings.ComPortName;
            ComPortBaudRate = settings.ComPortBaudRate;

            // File
            FileName = settings.FileName;

            // Generator
            InputFrequencyHz = settings.InputFrequencyHz;
            AmplitudeVrms = settings.AmplitudeVrms;
            DcOffsetV = settings.DcOffsetV;

            // fixing some fields depending on the data format
            var helper = new DataParserHelper(_dataFormat);
            IsFrameWidthChangable = !helper.IsFixedFrameWidth;
            if (!IsFrameWidthChangable)
                FrameWidth = helper.FrameWidth;
        }

        void OnSelectFile()
        {
            try
            {
                var prms = new InteractionParams();
                SelectFileInteraction.Raise(prms);
                if (prms.DialogResult == true)
                    FileName = prms.FileName;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        async void OnOk()
        {
            try
            {
                var newSettings = GetNewSettings();
                _dataSourceService.ApplyNewSettings(newSettings);
                await _navigationService.Close(this);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        DataSourceSettings GetNewSettings()
        {
            return new DataSourceSettings()
            {
                // General parameters
                DataSourceType = this.DataSourceType,
                DataMode = this.DataMode,
                DataFormat = this.DataFormat,
                FrameWidth = this.FrameWidth,
                SamplingRateHz = this.SamplingRateHz,

                // COM port
                ComPortName = this.ComPortName,
                ComPortBaudRate = this.ComPortBaudRate,

                // File
                FileName = this.FileName,

                // Generator
                //PointCount = this.PointCount,
                InputFrequencyHz = this.InputFrequencyHz,
                AmplitudeVrms = this.AmplitudeVrms,
                DcOffsetV = this.DcOffsetV
            };
        }

    }
}
