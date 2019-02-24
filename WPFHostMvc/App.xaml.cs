using System.Windows;
using WPFHostMvc.WebHost;

namespace WPFHostMvc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await HostBuilder.Start();
        }
    }
}
