namespace OpenSilver.TemplateWizards.Models
{
    public class WizardSettings
    {
        public string TargetFramework { get; set; }
        public string BlazorTargetFramework { get; set; }
        public string BlazorPackagesVersion { get; set; }
        public string OpenSilverPackageName { get; set; }
        public string OpenSilverType { get; set; }
        public string OpenSilverPackageVersion { get; set; }
        public string OpenSilverThemeModern { get; set; }
        public string OpenSilverWebAssemblyPackageVersion { get; set; }
        public string OpenSilverSimulatorPackageVersion { get; set; }
        public string OpenRia46PackageVersion { get; set; }
        public string OpenRia54PackageVersion { get; set; }
        public string OpenRia54AspNetCorePackageVersion { get; set; }
        public string MauiWindowsTargetVersion { get; set; }
        public string MauiWindowsMinVersion { get; set; }
        public string MauiAndroidVersion { get; set; }
        public string[] PatternsToReplace { get; set; }
    }
}