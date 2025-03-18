using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow.Models
{
    public class InstallMauiViewModel : INotifyPropertyChanged
    {
        private string _installerScreenshotPath;
        private bool _canClose;

        public InstallMauiViewModel()
        {
            // Default values
            InstallerScreenshotPath = "/OpenSilver.TemplateWizards;component/Assets/Images/vs-workloads.png";
            WindowTitle = "OpenSilver Configuration";
            HeaderText = "Please install the .NET MAUI workload";
            InstructionText = "This feature requires the .NET MAUI workload to be installed on your machine. To do so, run the Visual Studio Installer from the Start Menu, and install the .NET Multi-platform App UI development workload, as shown in the following screenshot:";
            UseTargetPlatformAnywayText = "Use this target platform anyway";
            CloseButtonText = "Close";
            CanClose = true;
        }

        // Properties
        public string InstallerScreenshotPath
        {
            get => _installerScreenshotPath;
            set
            {
                if (_installerScreenshotPath != value)
                {
                    _installerScreenshotPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WindowTitle { get; set; }
        public string HeaderText { get; set; }
        public string InstructionText { get; set; }
        public string UseTargetPlatformAnywayText { get; set; }
        public string CloseButtonText { get; set; }

        public bool CanClose
        {
            get => _canClose;
            set
            {
                if (_canClose != value)
                {
                    _canClose = value;
                    OnPropertyChanged();
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
