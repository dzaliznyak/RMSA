using MvvmCross.ViewModels;
using Rmsa.Model;
using System.Collections.Generic;

namespace Rmsa.ViewModel
{
    public class DisplaySettingsViewModel : MvxViewModel<Display>
    {
        Display _display;
        public IEnumerable<Channel> Channels => _display.Channels;

        public override void Prepare(Display display)
        {
            _display = display;
        }
    }
}
