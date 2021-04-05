using MvvmCross.ViewModels;
using Rmsa.Core.Graph;
using System.Runtime.Serialization;

namespace Rmsa.ViewModel
{
    [DataContract]
    public class DisplayRuntimeState : MvxViewModel
    {
        DataChannelNo _activeChannel = DataChannelNo.CH1;
        bool _isAutoZoom = true;

        [DataMember]
        public bool IsAutoZoom
        {
            get => _isAutoZoom;
            set => SetProperty(ref _isAutoZoom, value);
        }

        [DataMember]
        public DataChannelNo ActiveChannel
        {
            get => _activeChannel;
            set 
            {
                _activeChannel = value;
                RaisePropertyChanged();
            }
        }

        [DataMember] public Margin InputGraphMargin { get; set; } = new Margin(70, 10, 10, 10);
        [DataMember] public Margin ResultGraphMargin { get; set; } = new Margin(70, 10, 10, 10);

    }
}
