using Microsoft.Extensions.Configuration;
using OpenSilver.Maui;

namespace $safeprojectname$
{
    public partial class App : Application
    {
        public App(IWebViewManagerService webViewManagerService, IServiceProvider services, IConfiguration configuration)
        {
            InitializeComponent();

            MainPage = new MainPage(webViewManagerService, services, configuration);
        }
    }
}