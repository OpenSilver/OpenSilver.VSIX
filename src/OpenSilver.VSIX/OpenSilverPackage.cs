using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.ExtensionManager;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSilver.VSIX
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("OpenSilver VSIX New", "OpenSilver extension for Visual Studio", "1.0")]
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

            IVsExtensionManager extensionManager = (IVsExtensionManager)GetService(typeof(SVsExtensionManager));
            if (extensionManager != null)
            {
                foreach (var extension in extensionManager.GetInstalledExtensions())
                {
                    if (string.Equals(extension.Header.Identifier, OldExtensionId, StringComparison.OrdinalIgnoreCase))
                    {
                        // Old extension detected!
                        VsShellUtilities.ShowMessageBox(
                            this,
                            "An older version of the OpenSilver extension is installed. Please uninstall the old version to avoid conflicts.",
                            "Extension Conflict Detected",
                            OLEMSGICON.OLEMSGICON_WARNING,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        break;
                    }
                }
            }
        }
    }
}
