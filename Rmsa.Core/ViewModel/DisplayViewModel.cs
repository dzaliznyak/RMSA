using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Rmsa.Core.DataSource;
using Rmsa.Model;
using Rmsa.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace Rmsa.ViewModel
{
    [DataContract]
    public class DisplayViewModel : MvxViewModel
    {
        readonly IMvxNavigationService _navigationService;
        readonly IDataSourceService _dataSourceService;
        string _errorMessage;


        public DisplayRuntimeState Settings { get; private set; }

        public Display Display { get; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                RaisePropertyChanged(nameof(IsHasError));
            }
        }

        public bool IsHasError
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage) || Display.IsHasError;
            }
        }

        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenDataSourceSettingsCommand { get; }

        public DisplayViewModel(IMvxNavigationService navigationService, IDataSourceService dataSourceService)
        {
            _navigationService = navigationService;
            _dataSourceService = dataSourceService;

            Display = new Display();
            Display.PropertyChanged += Display_PropertyChanged;

            OpenSettingsCommand = new MvxCommand(() => OpenSettingsDialog());
            OpenDataSourceSettingsCommand = new MvxCommand(() => OpenDataSourceSettingsDialog());

            _dataSourceService.DataSourceChanged += DataSourceService_DataSourceChanged;
        }

        void Display_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Display.ErrorMessage) || e.PropertyName == nameof(Display.ChannelErrors))
                RaisePropertyChanged(nameof(IsHasError));
            else if (e.PropertyName == nameof(Display.Channels))
                Settings.ActiveChannel = DataChannelNo.CH1;
        }

        void DataSourceService_DataSourceChanged(object sender, IDataSource dataSource)
        {
            ErrorMessage = null;
            Display.SetDataSource(dataSource);
        }

        public override void Prepare()
        {
            try
            {
                if (!Directory.Exists(Defines.ConfigDir))
                    Directory.CreateDirectory(Defines.ConfigDir);

                if (!Directory.Exists(Defines.LogsDir))
                    Directory.CreateDirectory(Defines.LogsDir);

                LoadSettingsFromFile();

                Settings.PropertyChanged += Settings_PropertyChanged;
                
                _dataSourceService.Init();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DisplayRuntimeState.ActiveChannel) ||
                e.PropertyName == nameof(DisplayRuntimeState.IsAutoZoom))
            {
                Display.RefreshData();
            }
        }

        async void OpenSettingsDialog()
        {
            await _navigationService.Navigate<DisplaySettingsViewModel, Display>(Display);
        }

        async void OpenDataSourceSettingsDialog()
        {
            await _navigationService.Navigate<DataSourceSettingsViewModel, DataSourceSettings>(_dataSourceService.Settings);
        }

        void LoadSettingsFromFile()
        {
            if (File.Exists(Defines.SettingsFileName))
            {
                var serialized = File.ReadAllText(Defines.SettingsFileName);
                Settings = JsonConvert.DeserializeObject<DisplayRuntimeState>(serialized);
            }
            else
            {
                Settings = new DisplayRuntimeState();
            }
        }

        public void SaveSettingsToFile()
        {
            var serialized = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(Defines.SettingsFileName, serialized);

            foreach (var channel in Display.Channels)
            {
                string fileName = string.Format(Defines.ChannelSettingsFileName, channel.Settings.ChannelNo);
                serialized = JsonConvert.SerializeObject(channel.Settings, Formatting.Indented);
                File.WriteAllText(fileName, serialized);
            }
        }
    }
}
