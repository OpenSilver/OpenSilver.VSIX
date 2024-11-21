using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace $ext_safeprojectname$.Browser
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<CookieHandler>();

            builder.Services
                .AddHttpClient("openria")
                .AddHttpMessageHandler<CookieHandler>();

            var host = builder.Build();
            await host.RunAsync();
        }

        public static void RunApplication(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Application.RunApplication(() =>
            {
                var app = new $ext_safeprojectname$.App(serviceProvider, configuration);
            });
        }

        private class CookieHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}