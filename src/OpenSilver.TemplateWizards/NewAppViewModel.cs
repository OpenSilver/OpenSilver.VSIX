using System.Collections.ObjectModel;

namespace OpenSilver.TemplateWizards
{
    public class NewAppViewModel : ViewModelBase
    {
        private Language _language = Language.CSharp;
        private TemplateType _templateType = TemplateType.Empty;
        private BusinessTemplateType _businesstemplateType = BusinessTemplateType.Modern;
        private DatabaseType _database = DatabaseType.Sqlite;
        private FrameworkVersion _frameworkVersion = FrameworkVersion.Net8;

        private ObservableCollection<PlatformItem> _platforms = new ObservableCollection<PlatformItem>(new[]
        {
            new PlatformItem{ PlatformType = PlatformType.Web, Description = "The default, Blazor-based launcher project", IsSelected = true },
            new PlatformItem{ PlatformType = PlatformType.Simulator, Description = "Windows launcher project, designed to easy of debugging and diagnosing", IsSelected = true },
            new PlatformItem{ PlatformType = PlatformType.Windows, Description = "Maui Windows launcher project" },
            new PlatformItem{ PlatformType = PlatformType.Android, Description = "Maui Android launcher project" },
            new PlatformItem{ PlatformType = PlatformType.macOS, Description = "Maui macOs launcher project", IsEnabled = false },
            new PlatformItem{ PlatformType = PlatformType.iOS, Description = "Maui iOS launcher project", IsEnabled = false }
        });

        public TemplateType TemplateType
        {
            get => _templateType;
            set => SetPropertyValue(ref _templateType, value);
        }
        public BusinessTemplateType BusinessTemplateType
        {
            get => _businesstemplateType;
            set => SetPropertyValue(ref _businesstemplateType, value);
        }

        public DatabaseType Database
        {
            get => _database;
            set => SetPropertyValue(ref _database, value);
        }

        public FrameworkVersion FrameworkVersion
        {
            get => _frameworkVersion;
            set => SetPropertyValue(ref _frameworkVersion, value);
        }

        public Language Language
        {
            get => _language;
            set
            {
                SetPropertyValue(ref _language, value);
                RaisePropertyChanged(nameof(FSharpIsNotSelected));
            }
        }

        public bool FSharpIsNotSelected => Language != Language.FSharp;

        public ObservableCollection<PlatformItem> Platforms
        {
            get => _platforms;
            set => SetPropertyValue(ref _platforms, value);
        }
    }
}