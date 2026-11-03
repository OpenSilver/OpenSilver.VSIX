using EnvDTE;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OpenSilver.TemplateWizards.Wizards
{
    /// <summary>
    /// Filters the items of an application project depending on the template kind selected in <see cref="AppCustomizationWizard"/>.
    /// The files of each kind are listed in the WizardData of the .vstemplate:
    /// <code>
    /// &lt;TemplateKindFiles&gt;
    ///   &lt;Wpf&gt;App.Wpf.xaml.cs;MainWindow.xaml;MainWindow.xaml.cs&lt;/Wpf&gt;
    ///   &lt;Silverlight&gt;App.xaml.cs;MainPage.xaml;MainPage.xaml.cs&lt;/Silverlight&gt;
    /// &lt;/TemplateKindFiles&gt;
    /// </code>
    /// Files that are not listed are always added.
    /// </summary>
    class TemplateKindWizard : IWizard
    {
        private readonly HashSet<string> _excludedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            if (!replacementsDictionary.TryGetValue("$ext_templatekind$", out var templateKindValue) ||
                !Enum.TryParse(templateKindValue, out TemplateKind templateKind))
            {
                templateKind = AppCustomizationWizard.LastSelectedTemplateKind;
            }

            if (!replacementsDictionary.TryGetValue("$wizarddata$", out var wizardData) || string.IsNullOrWhiteSpace(wizardData))
            {
                return;
            }

            var files = XElement.Parse(wizardData);
            var ns = files.GetDefaultNamespace();
            var excludedKind = templateKind == TemplateKind.Wpf ? TemplateKind.Silverlight : TemplateKind.Wpf;
            var excludedList = files.Element(ns + excludedKind.ToString())?.Value ?? "";

            foreach (var file in excludedList.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim()))
            {
                _excludedFiles.Add(file);
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return !_excludedFiles.Contains(Path.GetFileName(filePath));
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }
    }
}
