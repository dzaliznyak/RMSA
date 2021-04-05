using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using Rmsa.ViewModel;

namespace Rmsa.Wpf.Views
{
    [MvxWindowPresentation(Identifier = nameof(DisplaySettingsView), Modal = true)]
    public partial class DisplaySettingsView : MvxWindow<DisplaySettingsViewModel>
    {
        public DisplaySettingsView()
        {
            InitializeComponent();
        }
    }
}
