using MvvmCross.ViewModels;
using System.Runtime.Serialization;

namespace Rmsa.Model
{
    [DataContract]
    public class ChannelSettings : MvxViewModel
    {
        bool _isVisible = true;

        [DataMember]
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }


        [DataMember] public DataChannelNo ChannelNo { get; }
        [DataMember] public InputGraphSettings InputGraphSettings { get; set; } = new InputGraphSettings();
        [DataMember] public ResultGraphSettings ResultGraphSettings { get; set; } = new ResultGraphSettings();

        public ChannelSettings(DataChannelNo channelNo)
        {
            ChannelNo = channelNo;
        }
    }
}
