using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards
{
    internal class OpenSilverAppWizard : IWizard
    {
        private readonly PlatformType[] MauiPlatforms = new[]
        {
            PlatformType.Windows,
            PlatformType.Android,
            PlatformType.macOS,
            PlatformType.iOS
        };

        private static bool _isAppWizardRunning = false;
        internal static bool IsAppWizardRunning => _isAppWizardRunning;
        internal WizardSettings WizardSettings { get; set; }
        internal NewAppViewModel OpenSilverSettings { get; set; }
        internal DatabaseSettings DatabaseSettings { get; set; }
        internal SslPorts SslPorts { get; set; }
        internal Dictionary<string, string> ReplacementsDictionary { get; set; }
        internal DTE DTE;

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ReplacementsDictionary = replacementsDictionary;
            DTE = automationObject as DTE;
            var destinationDirectory = replacementsDictionary["$destinationdirectory$"];

            _isAppWizardRunning = true;

            // get the language from wizarddata from the caller template
            var openSilverInfo = XElement.Parse(ReplacementsDictionary["$wizarddata$"]);
            var defaultNamespace = openSilverInfo.GetDefaultNamespace();

            if (!Enum.TryParse(openSilverInfo.Element(defaultNamespace + "Language").Value, true, out Language language))
            {
                language = Language.CSharp;
            }
            ReplacementsDictionary["$languagecode$"] = ((LanguageCode)(int)language).ToString().ToLower();

            var window = new OpenSilverAppWindow(language);

            bool? result = window.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
            }

            try
            {
                OpenSilverSettings = window.ViewModel;

                // load default settings
                WizardSettings = Helpers.LoadEmbeddedResource<WizardSettings>("Settings.WizardSettings.json");
                // update the framework version with incoming from wizard
                var netVersion = $"{OpenSilverSettings.FrameworkVersion}.0".ToLower();
                WizardSettings.TargetFramework = netVersion;
                WizardSettings.BlazorTargetFramework = netVersion;
                WizardSettings.BlazorPackagesVersion = $"{(int)OpenSilverSettings.FrameworkVersion}.0.0";
                // apply default settings
                ReplacementsDictionary.Populate(WizardSettings);
                ReplacementsDictionary.Add("$mauiappstartparams$", string.Empty);

                if (OpenSilverSettings.TemplateType == TemplateType.Business)
                {
                    var frameworkVersion = (int)OpenSilverSettings.FrameworkVersion;
                    var projectName = ReplacementsDictionary["$safeprojectname$"];
                    ReplacementsDictionary["$mauiappstartparams$"] = "_services, _configuration";

                    var database = OpenSilverSettings.Database.ToString().ToLower();
                    var databaseSettings = Helpers.LoadEmbeddedResource<DatabaseSettings>($"Settings.{database}.json");
                    databaseSettings.DatabaseConnectionString = string.Format(databaseSettings.DatabaseConnectionString, projectName);
                    ReplacementsDictionary.Populate(databaseSettings);

                    SslPorts = new SslPorts
                    {
                        // iisexpress requires the ssl port be between 44300 and 44399
                        SslServerPort = Helpers.GetPortFromRange(44300, 44400),
                        SslClientPort = Helpers.GetPortFromRange(50000, 60000)
                    };

                    ReplacementsDictionary.Populate(SslPorts);
                }
            }
            catch (Exception ex)
            {
                if (Directory.Exists(destinationDirectory))
                {
                    try
                    {
                        (DTE.Solution as Solution2)?.Close();
                        Directory.Delete(destinationDirectory, true);
                    }
                    catch { }
                }

                throw new WizardCancelledException("OpenSilver project creation failed", ex);
            }
        }

        public void ProjectFinishedGenerating(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solution = (Solution2)DTE.Solution;
            var destinationDirectory = ReplacementsDictionary["$destinationdirectory$"];

            try
            {
                // 1. load the right template
                var projectName = ReplacementsDictionary["$safeprojectname$"];
                var slnPath = Path.Combine(destinationDirectory, $"{projectName}.sln");
                var templateName = (OpenSilverSettings.TemplateType == TemplateType.Business)
                    ? $"{(LanguageCode)(int)OpenSilverSettings.Language}{OpenSilverSettings.BusinessTemplateType}BusinessApp"
                    : "OpenSilverApplication";

                var prjTemplate = solution.GetProjectTemplate(templateName, $"{OpenSilverSettings.Language}");
                solution.AddFromTemplate(prjTemplate, destinationDirectory, projectName);

                // 2. remove Browser and/or Simulator if wanted
                var mustRemoveSimulator = !OpenSilverSettings.Platforms.FirstOrDefault(x => x.PlatformType == PlatformType.Simulator)?.IsSelected ?? false;
                var mustRemoveBrowser = !OpenSilverSettings.Platforms.FirstOrDefault(x => x.PlatformType == PlatformType.Web)?.IsSelected ?? false;
                if (mustRemoveSimulator || mustRemoveBrowser)
                {
                    foreach (Project prj in solution.Projects)
                    {
                        if ((mustRemoveSimulator && prj.Name.EndsWith(".Simulator", StringComparison.OrdinalIgnoreCase))
                           || (mustRemoveBrowser && prj.Name.EndsWith(".Browser", StringComparison.OrdinalIgnoreCase)))
                        {
                            solution.Remove(prj);
                            Directory.Delete(Path.GetDirectoryName(prj.FileName), true);
                        }
                    }
                }

                // 3. include maui template
                var selectedMauiPlatforms = OpenSilverSettings.Platforms
                    .Where(x => x.IsSelected)
                    .Where(x => MauiPlatforms.Contains(x.PlatformType));

                if (selectedMauiPlatforms.Any())
                {
                    var vsixPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var mauiTemplatePath = Path.Combine(vsixPath, @"ProjectTemplates\CSMauiAppTemplate\CSMauiAppTemplate.vstemplate");
                    var mauiProjectName = projectName + ".Maui";
                    var mauiProjectPath = Path.Combine(destinationDirectory, mauiProjectName);

                    solution.AddFromTemplate(mauiTemplatePath, mauiProjectPath, mauiProjectName);

                    // remove undesired platforms
                    var unSelectedMauiPlatforms = OpenSilverSettings.Platforms
                        .Where(x => !x.IsSelected)
                        .Where(x => MauiPlatforms.Contains(x.PlatformType));

                    if (unSelectedMauiPlatforms.Any())
                    {
                        foreach (Project prj in solution.Projects)
                        {
                            if (prj.Name.EndsWith(".Maui", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (ProjectItem prjItem in prj.ProjectItems)
                                {
                                    if (prjItem.Name.Equals("Platforms", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var prjText = File.ReadAllText(prj.FileName);
                                        foreach (ProjectItem platformItem in prjItem.ProjectItems)
                                        {
                                            var platformName = platformItem.Name;
                                            if (unSelectedMauiPlatforms.Any(x => string.Equals(x.PlatformType.ToString(), platformName, StringComparison.OrdinalIgnoreCase)))
                                            {
                                                platformItem.Delete();
                                                prjText = prjText.Replace($@"<!-- {platformName} -->", "<!--")
                                                                 .Replace($@"<!-- /{platformName} -->", "-->");
                                            }
                                        }
                                        File.WriteAllText(prj.FileName, prjText);
                                    }
                                }
                            }
                        }
                    }
                }

                // 4. finish loading the template
                solution.SaveAs(slnPath);

                // 5. fixing missing replacements in files
                var allFiles = Helpers.EnumerateFiles(destinationDirectory, WizardSettings.PatternsToReplace);

                foreach (var file in allFiles)
                {
                    try
                    {
                        var text = File.ReadAllText(file);
                        foreach (var item in ReplacementsDictionary)
                        {
                            text = text.Replace(item.Key, item.Value)
                                       .Replace("$ext_" + item.Key.Substring(1), item.Value);
                        }
                        File.WriteAllText(file, text);
                    }
                    catch { }
                }

                // 6. apply theme
                var languageCode = ReplacementsDictionary["$languagecode$"];
                AddThemeReferences(Path.Combine(destinationDirectory, projectName, $"{projectName}.{languageCode}proj"));
                AddThemePalette(Path.Combine(destinationDirectory, projectName, "App.xaml"), OpenSilverSettings.Theme.ToString());

                // 7. refresh the solution
                solution.Close();
                solution.Open(slnPath);
            }
            catch (Exception ex)
            {
                if (Directory.Exists(destinationDirectory))
                {
                    try
                    {
                        solution?.Close();
                        Directory.Delete(destinationDirectory, true);
                    }
                    catch { }
                }

                throw new WizardCancelledException("OpenSilver project creation failed", ex);
            }
        }

        private void AddThemePalette(string appXamlPath, string theme)
        {
            var appXamlContent = File.ReadAllText(appXamlPath);
            appXamlContent = appXamlContent.Replace(
            "</Application.Resources>",
                $"</Application.Resources>\n<Application.Theme>\n<mt:ModernTheme CurrentPalette=\"{theme}\" xmlns:mt=\"http://opensilver.net/themes/modern\"/>\n</Application.Theme>"
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
                var newElement = new XElement("PackageReference",
                    new XAttribute("Include", "OpenSilver.Themes.Modern"),
                    new XAttribute("Version", WizardSettings.OpenSilverThemeModern));

                itemGroupWithPackageReference.Add(newElement);

                csprojDocument.Save(path);
            }
        }

        #region IWizard Implementation

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        #endregion IWizard Implementation
    }
}