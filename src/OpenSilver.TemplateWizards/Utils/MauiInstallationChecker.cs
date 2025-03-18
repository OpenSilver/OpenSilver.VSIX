using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using System;
using System.Linq;
using System.Windows;
using EnvDTE;

namespace OpenSilver.TemplateWizards.Utils
{
    public class MauiInstallationChecker
    {
        private readonly IServiceProvider _serviceProvider;

        public MauiInstallationChecker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool IsMauiInstalled()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the current VS instance installation path
            var dte = (DTE)_serviceProvider.GetService(typeof(DTE));
            var currentVsPath = dte?.FullName;

            if (string.IsNullOrEmpty(currentVsPath))
            {
                MessageBox.Show("currentVsPath is null");
                return false;
            }

            var configuration = new SetupConfiguration();
            var setupInstances = configuration.EnumAllInstances();

            int fetched;
            var instances = new ISetupInstance[1];

            do
            {
                setupInstances.Next(1, instances, out fetched);
                if (fetched <= 0)
                    break;

                var instance = instances[0];
                var installationPath = instance.GetInstallationPath();

                // Match the Visual Studio instance by installation path
                if (!currentVsPath.StartsWith(installationPath, StringComparison.OrdinalIgnoreCase))
                    continue;

                var packages = ((ISetupInstance2)instance).GetPackages();

                bool mauiInstalled = packages.Any(pkg =>
                    pkg.GetId().Equals("Microsoft.VisualStudio.Workload.NetCrossPlat", StringComparison.OrdinalIgnoreCase));

                if (mauiInstalled)
                    return true;

            } while (fetched > 0);

            return false;
        }
    }
}
