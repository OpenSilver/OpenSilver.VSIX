using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenSilver.Simulator;
using System;

namespace $ext_safeprojectname$.Simulator
{
    internal static class Startup
    {
        [STAThread]
        private static int Main(string[] args)
        {
            using (var host = Host.CreateDefaultBuilder(args).Build())
            {
                host.Start();

                return SimulatorLauncher.Start(
                    () => new App(host.Services, _config),
                    typeof(App).Assembly);
            }
        }

        private static readonly IConfiguration _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }
}