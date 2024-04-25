using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace OpenSilver.TemplateWizards
{
    internal class BusinessAppWizard : IWizard
    {
        private DTE _dte;
        private NewProjectViewModel viewModel;
        private Dictionary<string, string> _replacementsDictionary;

        private static readonly string LogFileName = $@"c:\logs\{DateTime.Now:yyyyMMdd-HHmmss}.log";

        private static void Log(string message = "", [CallerMemberName] string name = "")
            => File.AppendAllLines(LogFileName, new[] { $"{DateTime.Now:yyyyMMdd-HHmmss} ({name}) {message}" });

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            Log();
            ThreadHelper.ThrowIfNotOnUIThread();

            _dte = automationObject as DTE;
            _replacementsDictionary = replacementsDictionary;
            var destinationDirectory = replacementsDictionary["$destinationdirectory$"];

            try
            {
                var window = new BusinessApplicationWindow();

                var result = window.ShowDialog();
                if (!result.HasValue || !result.Value)
                {
                    throw new WizardBackoutException("OpenSilver project creation was cancelled by user");
                }

                viewModel = window.ViewModel;

                var frameworkVersion = (int)viewModel.FrameworkVersion;

                replacementsDictionary.Add("$blazortargetframework$", $"net{frameworkVersion}.0");
                replacementsDictionary.Add("$blazorpackagesversion$", $"{frameworkVersion}.0.0");
                replacementsDictionary.Add("$opensilverpackagename$", "OpenSilver");
                replacementsDictionary.Add("$opensilverpackageversion$", "2.1.0");
                replacementsDictionary.Add("$openria46packageversion$", "2.2.0-preview-2024-03-15-145535-bada268a");
                replacementsDictionary.Add("$openria54packageversion$", "5.4.3");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                if (Directory.Exists(destinationDirectory))
                {
                    Directory.Delete(destinationDirectory, true);
                }

                throw;
            }
        }

        public void ProjectFinishedGenerating(Project project)
        {
            Log();

            var destinationDirectory = _replacementsDictionary["$destinationdirectory$"];
            var projectName = _replacementsDictionary["$safeprojectname$"];
            var slnPath = Path.Combine(destinationDirectory, $"{projectName}.sln");
            var projectType = viewModel.Backend.ToString();
            var languageCode = viewModel.LanguageCode.ToString();
            var language = viewModel.LanguageCode == LanguageCode.CS
                ? "CSharp"
                : viewModel.LanguageCode == LanguageCode.VB
                ? "VisualBasic"
                : "FSharp";

            // 1. loading the right template
            ThreadHelper.ThrowIfNotOnUIThread();
            var solution = (Solution2)_dte.Solution;

            var prjTemplate = solution.GetProjectTemplate($"{languageCode}{projectType}BusinessApp", language);
            solution.AddFromTemplate(prjTemplate, destinationDirectory, projectName);
            solution.SaveAs(slnPath);

            // 2. fixing missing replacements in proj files
            var projFiles = Directory.GetFiles(destinationDirectory, $"*.{languageCode}proj", SearchOption.AllDirectories);

            foreach (var proj in projFiles)
            {
                var text = File.ReadAllText(proj);
                foreach (var item in _replacementsDictionary)
                {
                    text = text.Replace(item.Key, item.Value)
                        .Replace("$ext_" + item.Key.Substring(1), item.Value);
                }
                File.WriteAllText(proj, text);
            }

            // 3. refresh the solution
            solution.Close();
            solution.Open(slnPath);
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

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}