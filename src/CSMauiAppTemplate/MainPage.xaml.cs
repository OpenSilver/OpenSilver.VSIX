using Microsoft.Extensions.Configuration;
using OpenSilver.Maui;

namespace $safeprojectname$
{
    public partial class MainPage : ContentPage
    {
        private IWebViewManagerService _webViewManagerService;
        private IServiceProvider _services;
        private IConfiguration _configuration;

        public MainPage(IWebViewManagerService webViewManagerService, IServiceProvider services, IConfiguration configuration)
        {
            _webViewManagerService = webViewManagerService;
            _services = services;
            _configuration = configuration;

            InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object? sender, EventArgs e)
        {
            await _webViewManagerService.InitializeWebViewAsync(blazorWebView, () => { var app = new $ext_safeprojectname$.App($mauiappstartparams$); });
        }
    }
}