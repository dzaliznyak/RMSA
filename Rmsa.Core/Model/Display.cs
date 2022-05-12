using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Rmsa.Core.DataSource;
using Rmsa.Core.Graph;
using Rmsa.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Rmsa.Model
{
    public class Display : MvxViewModel
    {
        readonly ConcurrentDictionary<DataChannelNo, Channel> _channels = new();
        readonly FpsCounter _fps = new();

        IDataSource _dataSource;
        string _errorMessage;

        public event EventHandler DataChanged;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                RaisePropertyChanged(nameof(IsHasError));
            }
        }

        public IReadOnlyCollection<Channel> Channels => (IReadOnlyCollection<Channel>)_channels.Values;

        public IEnumerable<string> ChannelErrors
        {
            get
            {
                return Channels.Select(t => t.ErrorMessage).Where(t => !string.IsNullOrEmpty(t));
            }
        }

        public bool IsHasError
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage) || ChannelErrors.Any();
            }
        }

        public Display()
        {
            CreateChannels();
        }

        public void RefreshData()
        {
            foreach (var channel in Channels)
                channel.RefreshData();
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Settings 
        static ChannelSettings LoadChannelSettings(DataChannelNo channelNo)
        {
            string fileName = string.Format(Defines.ChannelSettingsFileName, channelNo);
            ChannelSettings settings;

            if (File.Exists(fileName))
            {
                var serialized = File.ReadAllText(fileName);
                settings = JsonConvert.DeserializeObject<ChannelSettings>(serialized);
            }
            else
            {
                settings = new ChannelSettings(channelNo);
            }
            return settings;
        }
        #endregion Settings 

        #region DataSource
        internal void SetDataSource(IDataSource dataSource)
        {
            if (_dataSource != null)
            {
                _dataSource.DataReceived -= DataSource_DataReceived;
                _dataSource.Error -= DataSource_Error;
                _dataSource = null;
            }

            _dataSource = dataSource;
            _dataSource.DataReceived += DataSource_DataReceived;
            _dataSource.Error += DataSource_Error;

            ErrorMessage = null;
            _fps.Reset();

            ResetChannels();
        }

        void DataSource_Error(object sender, string e)
        {
            ErrorMessage = e;
        }

        void DataSource_DataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                _fps.Update();
                //System.Diagnostics.Trace.WriteLine($"FPS: {FPS}");

                for (int i = 0; i < e.Data.Count; i++)
                {
                    DataChannelNo channelNo = (DataChannelNo)i + 1;
                    Channel channel = GetChannel(channelNo);
                    var data = new InputData(e.Data[i], channelNo, e.SamplingRateHz, _fps.Avg());
                    channel.SetData(data);
                }

                DataChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (ChannelException)
            {
                RaisePropertyChanged(nameof(ChannelErrors));
                RaisePropertyChanged(nameof(IsHasError));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        #endregion DataSource

        void CreateChannels()
        {
            if (_channels.IsEmpty)
            {
                // create at least one channel
                GetChannel(DataChannelNo.CH1);
            }
        }

        public Channel GetChannel(DataChannelNo channelNo)
        {
            bool newCreated = false;
            var channel = _channels.GetOrAdd(channelNo, (chNo) =>
            {
                var channel = new Channel(LoadChannelSettings(chNo));
                channel.PropertyChanged += Channel_PropertyChanged;
                newCreated = true;

                if (chNo == DataChannelNo.CH1)
                    channel.Settings.IsVisible = true;

                return channel;
            });

            if (newCreated)
                RaisePropertyChanged(nameof(Channels));

            return channel;
        }

        void ResetChannels()
        {
            foreach (var channel in Channels)
            {
                channel.PropertyChanged -= Channel_PropertyChanged;
            }
            _channels.Clear();
            RaisePropertyChanged(nameof(Channels));
        }

        void Channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Channel.ErrorMessage))
            {
                RaisePropertyChanged(nameof(ChannelErrors));
            }
            else if (e.PropertyName == nameof(ChannelSettings.IsVisible))
            {
                DataChanged?.Invoke(this, EventArgs.Empty);
            }
        }

    }


}
