using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OpenSilver.TemplateWizards
{
    class AppCustomizationWizard : IWizard
    {
        private const string NugetConfig = "NuGet.Config";
        private Dictionary<string, string> _replacementsDictionary;
        private string _selectedTheme;
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


        private string GetFileFullPath(string fileRelativePath, ProjectTemplate template)
        {
            var solutionDir = _replacementsDictionary["$destinationdirectory$"];
            string projectName = _replacementsDictionary["$safeprojectname$"];
            if (template != ProjectTemplate.OpenSilver)
            {
                projectName += $".{template.ToString()}";
            }
            string fullPath = $"{solutionDir}\\{projectName}\\{fileRelativePath}";
            return fullPath;
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {

        }

        public void ProjectFinishedGenerating(Project project)
        {
            if (_selectedTheme.Equals("Classic", StringComparison.OrdinalIgnoreCase))
                return;


            string projectName = _replacementsDictionary["$safeprojectname$"];
            string language = GetCurrentProgrammingLanguage();

            string appXamlPath = GetFileFullPath("App.xaml", ProjectTemplate.OpenSilver);

            string OpenSilverCsprojPath = GetFileFullPath($"{projectName}.{language}proj", ProjectTemplate.OpenSilver);

            if (File.Exists(appXamlPath) && File.Exists(OpenSilverCsprojPath))
            {

                AddThemePalette(appXamlPath);
                AddThemeReferences(OpenSilverCsprojPath);
            }
        }

        private string GetCurrentProgrammingLanguage()
        {
            var openSilverInfo = XElement.Parse(_replacementsDictionary["$wizarddata$"]);
            XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();
            string Language = openSilverInfo.Element(defaultNamespace + "LanguageCode").Value;
            return Language;
        }

        private void AddThemePalette(string appXamlPath)
        {
            var appXamlContent = File.ReadAllText(appXamlPath);
            appXamlContent = appXamlContent.Replace(
            "</Application.Resources>",
                $"</Application.Resources>\n<Application.Theme>\n<mt:ModernTheme CurrentPalette=\"{_selectedTheme}\" xmlns:mt=\"http://opensilver.net/themes/modern\"/>\n</Application.Theme>"
            );
            XDocument xdoc = XDocument.Parse(appXamlContent);
            xdoc.Save(appXamlPath, SaveOptions.None);
        }

        private void AddThemeReferences(string path)
        {
            var csprojDocument = XDocument.Load(path);
            var itemGroupWithPackageReference = csprojDocument.Descendants("ItemGroup")
                            .FirstOrDefault(ig => ig.Elements("PackageReference").Any());

            if (itemGroupWithPackageReference != null)
            {
                // Create the new element to insert
                ;
                var newElement = new XElement("PackageReference",
                    new XAttribute("Include", "OpenSilver.Themes.Modern"),
                    new XAttribute("Version", _replacementsDictionary["$opensilverthememodern$"]));

                itemGroupWithPackageReference.Add(newElement);

                csprojDocument.Save(path);
            }
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


            AppConfigurationWindow window = new AppConfigurationWindow(openSilverType);

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
                        replacementsDictionary.Add("$blazorpackagesversion$", "8.0.0");
                        break;
                    case DotNetVersion.Net9:
                        replacementsDictionary.Add("$targetframework$", "net9.0");
                        replacementsDictionary.Add("$blazorpackagesversion$", "9.0.0");
                        break;
                }

                CopyNugetConfig(replacementsDictionary);
            }

            replacementsDictionary.Add("$opensilverpackageversion$", "3.1.0");
            replacementsDictionary.Add("$opensilversimulatorpackageversion$", "3.1.0");
            replacementsDictionary.Add("$opensilverwebassemblypackageversion$", "3.1.0");
            replacementsDictionary.Add("$openria46packageversion$", "3.1.0");
            replacementsDictionary.Add("$opensilverthememodern$", "3.1.0");

            _replacementsDictionary = replacementsDictionary;
            _selectedTheme = window.SelectedTheme;
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
