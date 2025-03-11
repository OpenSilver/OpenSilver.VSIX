using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.ExtensionManager;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OpenSilver.VSIX
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("OpenSilver SDK", "OpenSilver SDK for Visual Studio", "3.0")]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class OpenSilverPackage : AsyncPackage
    {
        private const string OldExtensionId = "OpenSilver.64201874-6ead-4828-b5af-31a76285b73d";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            DetectOldExtension();
        }

        private void DetectOldExtension()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                IVsExtensionManager extensionManager = (IVsExtensionManager)GetService(typeof(SVsExtensionManager));
                if (extensionManager != null)
                {
                    foreach (var extension in extensionManager.GetInstalledExtensions())
                    {
                        if (string.Equals(extension.Header.Identifier, OldExtensionId, StringComparison.OrdinalIgnoreCase))
                        {
                            int result = VsShellUtilities.ShowMessageBox(
                                this,
                                "An older version of the OpenSilver extension is installed. Please uninstall the old version to avoid conflicts.\n\n" +
                                "Would you like to open a page with detailed instructions on how to proceed?",
                                "Extension Conflict Detected",
                                OLEMSGICON.OLEMSGICON_WARNING,
                                OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
                                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                            if (result == (int)Microsoft.VisualStudio.VSConstants.MessageBoxResult.IDYES)
                            {
                                Process.Start(new ProcessStartInfo("https://opensilver.net/uninstall-old-extension")
                                {
                                    UseShellExecute = true
                                });
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    this,
                    "An error occurred while checking for old extensions: " + ex.Message,
                    "Error",
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}
