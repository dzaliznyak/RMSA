using MvvmCross;
using MvvmCross.ViewModels;
using Rmsa.Services;
using Rmsa.ViewModel;

namespace Rmsa.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            Mvx.IoCProvider.RegisterSingleton<IDataSourceService>(new DataSourceService());

            RegisterAppStart<DisplayViewModel>();
        }

        
    }
}
