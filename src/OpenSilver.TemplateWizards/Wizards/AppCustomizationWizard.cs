using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using OpenSilver.TemplateWizards.AppCustomizationWindow.Models;
using OpenSilver.TemplateWizards.Shared;
using OpenSilver.TemplateWizards.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards.Wizards
{
    class AppCustomizationWizard : IWizard
    {
        private const string NugetConfig = "NuGet.Config";
        private const string ClassicLoadingAnimation = @"
        <div class=""loading-indicator-wrapper"">
            <div class=""loading-indicator"">
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-ball""></div>
                <div class=""loading-indicator-text""></div>
            </div>
        </div>";
        private const string ModernLoadingAnimation = @"
        <div class=""opensilver-loading-indicator"">
            <div class=""opensilver-loader-container"">
                <div class=""opensilver-loader"">
                    <div class=""opensilver-loader-progress"">
                        <div class=""opensilver-loader-progress-bar""></div>
                    </div>
                </div>
                <div class=""opensilver-counter-container"">
                    <span class=""opensilver-odometer""></span>
                    <span class=""opensilver-odometer""></span>
                    <span class=""opensilver-odometer""></span>
                </div>
            </div>
        </div>";
        private const string ModernLightStyles = @"
    <style>
        :root {
            --opensilver-loading-background-color: #fff;
            --opensilver-loading-progress-bg: #f0f0f0;
            --opensilver-loading-progress-bar-color: #696969;
            --opensilver-loading-counter-color: #555;
            --opensilver-loading-progress-border-color: #c0c0c0;
        }
    </style>";
        private const string ModernDarkStyles = @"
    <style>
        :root {
            --opensilver-loading-background-color: #201f1f;
            --opensilver-loading-progress-bg: #505050;
            --opensilver-loading-progress-bar-color: #fff;
            --opensilver-loading-counter-color: #848181;
            --opensilver-loading-progress-border-color: #505050;
        }
    </style>";
        private const string ModernLoadingJs = @"
            const script = document.createElement('script');
            script.setAttribute('type', 'application/javascript');
            script.setAttribute('src', 'modern/loading-animation.js?date=' + new Date().toISOString());
            document.head.appendChild(script);";

        private Dictionary<string, string> _replacementsDictionary;
        private DTE _dte;

        private MauiHybridPlatform _mauiHybridPlatform;
        private bool _isPhotinoSelected;
        private DotNetVersion _dotNetVersion;

        private static string GetVsixFullPath(string filename)
        {
            var vsixDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(vsixDir))
            {
                return null;
            }

            return Path.Combine(
                vsixDir,
                filename
            );
        }

        private static bool IsProjectAndSolutionSameDir(Dictionary<string, string> replacementsDictionary)
        {
            var solutionNameKey = "$specifiedsolutionname$";
            var solutionName = replacementsDictionary.ContainsKey(solutionNameKey) ? replacementsDictionary[solutionNameKey] : null;
            return string.IsNullOrEmpty(solutionName);
        }

        private static void CopyNugetConfig(Dictionary<string, string> replacementsDictionary)
        {
            try
            {
                var directoryKey = IsProjectAndSolutionSameDir(replacementsDictionary) ? "$destinationdirectory$" : "$solutiondirectory$";
                var solutionDir = replacementsDictionary.ContainsKey(directoryKey) ? replacementsDictionary[directoryKey] : null;
                var vsixNugetConfig = GetVsixFullPath(NugetConfig);
                if (solutionDir != null && vsixNugetConfig != null)
                {
                    File.Copy(vsixNugetConfig, Path.Combine(solutionDir, NugetConfig));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("NuGet.Config has not been created.\r\nError Message:\r\n" + ex.Message);
            }
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {

        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        private string GetAppXamlTheme(ThemeOptions selectedTheme)
        {
            if (IsClassic(selectedTheme))
            {
                return "";
            }

            return $"    <Application.Theme>{Environment.NewLine}        <mt:ModernTheme CurrentPalette=\"{selectedTheme.Name}\" xmlns:mt=\"http://opensilver.net/themes/modern\"/>{Environment.NewLine}    </Application.Theme>";
        }

        private string GetThemesNugetPackageLine(string version)
        {
            return $"{Environment.NewLine}    <PackageReference Include=\"OpenSilver.Themes.Modern\" Version=\"{version}\" />";
        }

        private string GetLoadingColors(ThemeOptions selectedTheme)
        {
            if (selectedTheme == ThemeOptions.Light)
            {
                return ModernLightStyles;
            }

            if (selectedTheme == ThemeOptions.Dark)
            {
                return ModernDarkStyles;
            }

            return "";
        }

        private string GetLoadingIndicatorCss(ThemeOptions selectedTheme)
        {
            if (IsClassic(selectedTheme))
            {
                return "loading-indicator.css";
            }

            return "modern/loading-indicator.css";
        }

        private string GetLoadingIndicatorJs(ThemeOptions selectedTheme)
        {
            if (IsClassic(selectedTheme))
            {
                return "";
            }

            return ModernLoadingJs;
        }

        private string GetLoadingIndicatorHtml(ThemeOptions selectedTheme)
        {
            if (IsClassic(selectedTheme))
            {
                return ClassicLoadingAnimation;
            }

            return ModernLoadingAnimation;
        }

        private void AddMauiHybrid()
        {
            try
            {
                var projectName = _replacementsDictionary["$safeprojectname$"];
                var destinationDirectory = _replacementsDictionary["$destinationdirectory$"];

                var templateName = "OpenSilverMauiHybridTemplate";
                var solution = (Solution2)_dte.Solution;

                var mauiHybridProjectName = projectName + ".MauiHybrid";
                var mauiHybridDestinationDirectory = Path.Combine(destinationDirectory, mauiHybridProjectName);

                var sharedDataStore = GlobalWizardDataStore.GetSharedData(mauiHybridDestinationDirectory);
                sharedDataStore[WizardKeys.MauiTargets] = _mauiHybridPlatform;
                sharedDataStore[WizardKeys.NetTarget] = _dotNetVersion;

                var prjTemplate = solution.GetProjectTemplate(templateName, "CSharp");
                solution.AddFromTemplate(prjTemplate, mauiHybridDestinationDirectory, mauiHybridProjectName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        private void AddPhotino()
        {
            try
            {
                var projectName = _replacementsDictionary["$safeprojectname$"];
                var destinationDirectory = _replacementsDictionary["$destinationdirectory$"];

                var templateName = "OpenSilverPhotinoTemplate";
                var solution = (Solution2)_dte.Solution;

                var photinoProjectName = projectName + ".Photino";
                var photinoDestinationDirectory = Path.Combine(destinationDirectory, photinoProjectName);

                var sharedDataStore = GlobalWizardDataStore.GetSharedData(photinoDestinationDirectory);
                sharedDataStore[WizardKeys.NetTarget] = _dotNetVersion;

                var prjTemplate = solution.GetProjectTemplate(templateName, "CSharp");
                solution.AddFromTemplate(prjTemplate, photinoDestinationDirectory, photinoProjectName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {

        }

        public void RunFinished()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (_mauiHybridPlatform != MauiHybridPlatform.None)
            {
                AddMauiHybrid();
            }

            if (_isPhotinoSelected)
            {
                AddPhotino();
            }
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            _replacementsDictionary = replacementsDictionary;
            _dte = automationObject as DTE;

            var isBusiness = false;
            var wizardData = replacementsDictionary["$wizarddata$"];
            if (!string.IsNullOrWhiteSpace(wizardData))
            {
                XElement openSilverInfo = XElement.Parse(wizardData);
                XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();
                isBusiness = openSilverInfo.Element(defaultNamespace + "IsBusiness")?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
            }

            AppConfigurationWindow window = new AppConfigurationWindow(_dte, isBusiness);

            bool? result = window.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
            }

            switch (window.DotNetVersion)
            {
                case DotNetVersion.Net7:
                    replacementsDictionary.Add("$blazorpackagesversion$", "7.0.0");
                    break;
                case DotNetVersion.Net8:
                    replacementsDictionary.Add("$blazorpackagesversion$", "8.0.11");
                    break;
                case DotNetVersion.Net9:
                    replacementsDictionary.Add("$blazorpackagesversion$", "9.0.0");
                    break;
                case DotNetVersion.Net10:
                    replacementsDictionary.Add("$blazorpackagesversion$", "10.0.0-rc.1.25451.107");
                    break;
            }

            CopyNugetConfig(replacementsDictionary);

            _mauiHybridPlatform = window.MauiHybridPlatform;
            _isPhotinoSelected = window.IsPhotinoSelected;
            _dotNetVersion = window.DotNetVersion;

            replacementsDictionary.Add("$targetframework$", EnumUtilities.GetEnumDescription(window.DotNetVersion));
            replacementsDictionary.Add("$opensilverpackageversion$", GlobalConstants.OpenSilverPackageVersion);
            replacementsDictionary.Add("$opensilversimulatorpackageversion$", GlobalConstants.OpenSilverSimulatorPackageVersion);
            replacementsDictionary.Add("$opensilverwebassemblypackageversion$", GlobalConstants.OpenSilverWebAssemblyPackageVersion);
            replacementsDictionary.Add("$openria46packageversion$", GlobalConstants.OpenRia46PackageVersion);
            replacementsDictionary.Add("$opensilverthememodern$", GlobalConstants.OpenSilverThemeModernPackageVersion);

            replacementsDictionary.Add("$pageforeground$", IsClassic(window.SelectedTheme) ? "Black" : "{DynamicResource Theme_TextBrush}");
            replacementsDictionary.Add("$gridbackground$", IsClassic(window.SelectedTheme) ? "White" : "{DynamicResource Theme_BackgroundBrush}");
            replacementsDictionary.Add("$appxamltheme$", GetAppXamlTheme(window.SelectedTheme));
            replacementsDictionary.Add("$themesnugetpackage$", IsClassic(window.SelectedTheme) ? "" : GetThemesNugetPackageLine(replacementsDictionary["$opensilverthememodern$"]));

            replacementsDictionary.Add("$modernloadingcolors$", GetLoadingColors(window.SelectedTheme));
            replacementsDictionary.Add("$loadingindicatorcss$", GetLoadingIndicatorCss(window.SelectedTheme));
            replacementsDictionary.Add("$loadingindicatorjs$", GetLoadingIndicatorJs(window.SelectedTheme));
            replacementsDictionary.Add("$loadingindicatorhtml$", GetLoadingIndicatorHtml(window.SelectedTheme));
        }

        private bool IsClassic(ThemeOptions theme)
        {
            return theme == ThemeOptions.Classic;
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
