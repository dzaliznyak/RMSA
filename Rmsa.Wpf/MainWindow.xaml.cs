using MvvmCross.Platforms.Wpf.Views;
using Rmsa.ViewModel;
using Rmsa.Wpf.Views;
using System;
using System.ComponentModel;
using System.Windows;

namespace Rmsa.Wpf
{
    public partial class MainWindow : MvxWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void MvxWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                var vm = ((DisplayView)Content).DataContext as DisplayViewModel;
                vm.SaveSettingsToFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
