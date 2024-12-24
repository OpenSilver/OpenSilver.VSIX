using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards
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

        private string GetAppXamlTheme(string selectedTheme)
        {
            if (selectedTheme == "Classic")
            {
                return "";
            }

            return $"    <Application.Theme>{Environment.NewLine}        <mt:ModernTheme CurrentPalette=\"{selectedTheme}\" xmlns:mt=\"http://opensilver.net/themes/modern\"/>{Environment.NewLine}    </Application.Theme>";
        }

        private string GetThemesNugetPackageLine(string version)
        {
            return $"{Environment.NewLine}    <PackageReference Include=\"OpenSilver.Themes.Modern\" Version=\"{version}\" />";
        }

        private string GetLoadingColors(string selectedTheme)
        {
            if (selectedTheme == "Light")
            {
                return ModernLightStyles;
            }

            if (selectedTheme == "Dark")
            {
                return ModernDarkStyles;
            }

            return "";
        }

        private string GetLoadingIndicatorCss(string selectedTheme)
        {
            if (selectedTheme == "Classic")
            {
                return "loading-indicator.css";
            }

            return "modern/loading-indicator.css";
        }

        private string GetLoadingIndicatorJs(string selectedTheme)
        {
            if (selectedTheme == "Classic")
            {
                return "";
            }

            return ModernLoadingJs;
        }

        private string GetLoadingIndicatorHtml(string selectedTheme)
        {
            if (selectedTheme == "Classic")
            {
                return ClassicLoadingAnimation;
            }

            return ModernLoadingAnimation;
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {

        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            XElement openSilverInfo = XElement.Parse(replacementsDictionary["$wizarddata$"]);

            XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();

            string openSilverType = openSilverInfo.Element(defaultNamespace + "Type").Value;
            bool isBusiness = openSilverInfo.Element(defaultNamespace + "IsBusiness")?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;


            AppConfigurationWindow window = new AppConfigurationWindow(openSilverType, isBusiness);

            // In the case of a class Library, the user has no other choices to make so we do not show the app configuration window.
            if (openSilverType != "Library")
            {
                bool? result = window.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
                }
            }

            if (openSilverType == "Application")
            {
                switch (window.DotNetVersion)
                {
                    case DotNetVersion.Net7:
                        replacementsDictionary.Add("$targetframework$", "net7.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "7.0.0");
                        break;
                    case DotNetVersion.Net8:
                        replacementsDictionary.Add("$targetframework$", "net8.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "8.0.11");
                        break;
                    case DotNetVersion.Net9:
                        replacementsDictionary.Add("$targetframework$", "net9.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "9.0.0");
                        break;
                }

                CopyNugetConfig(replacementsDictionary);
            }

            replacementsDictionary.Add("$opensilverpackageversion$", "3.1.2");
            replacementsDictionary.Add("$opensilversimulatorpackageversion$", "3.1.2");
            replacementsDictionary.Add("$opensilverwebassemblypackageversion$", "3.1.2");
            replacementsDictionary.Add("$openria46packageversion$", "3.1.0");
            replacementsDictionary.Add("$opensilverthememodern$", "3.1.*");

            replacementsDictionary.Add("$pageforeground$", IsClassic(window) ? "Black" : "{DynamicResource Theme_TextBrush}");
            replacementsDictionary.Add("$gridbackground$", IsClassic(window) ? "White" : "{DynamicResource Theme_BackgroundBrush}");
            replacementsDictionary.Add("$appxamltheme$", GetAppXamlTheme(window.SelectedTheme));
            replacementsDictionary.Add("$themesnugetpackage$", IsClassic(window) ? "" : GetThemesNugetPackageLine(replacementsDictionary["$opensilverthememodern$"]));

            replacementsDictionary.Add("$modernloadingcolors$", GetLoadingColors(window.SelectedTheme));
            replacementsDictionary.Add("$loadingindicatorcss$", GetLoadingIndicatorCss(window.SelectedTheme));
            replacementsDictionary.Add("$loadingindicatorjs$", GetLoadingIndicatorJs(window.SelectedTheme));
            replacementsDictionary.Add("$loadingindicatorhtml$", GetLoadingIndicatorHtml(window.SelectedTheme));
        }

        private bool IsClassic(AppConfigurationWindow window)
        {
            return window.SelectedTheme == "Classic";
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
