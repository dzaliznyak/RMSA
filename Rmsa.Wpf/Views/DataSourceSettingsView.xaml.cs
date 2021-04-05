using Microsoft.Win32;
using MvvmCross.Base;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using Rmsa.Core.Utils;
using Rmsa.ViewModel;
using System.Windows;

namespace Rmsa.View
{
    [MvxWindowPresentation(Identifier = nameof(DataSourceSettingsView), Modal = true)]
    public partial class DataSourceSettingsView : MvxWindow<DataSourceSettingsViewModel>
    {
        public DataSourceSettingsView()
        {
            InitializeComponent();
        }

        void MvxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectFileInteraction.Requested += SelectFileInteraction_Requested;
        }

        void SelectFileInteraction_Requested(object sender, MvxValueEventArgs<InteractionParams> e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Text files|*.txt|Log files|*.log|All Files|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                e.Value.DialogResult = true;
                e.Value.FileName = ofd.FileName;
            }
        }


    }
}
