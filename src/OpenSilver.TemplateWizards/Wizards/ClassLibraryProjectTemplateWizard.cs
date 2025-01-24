using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.Shared;
using System.Collections.Generic;

namespace OpenSilver.TemplateWizards.Wizards
{
    class ClassLibraryProjectTemplateWizard : IWizard
    {
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            replacementsDictionary.Add("$opensilverpackageversion$", GlobalConstants.OpenSilverPackageVersion);
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
