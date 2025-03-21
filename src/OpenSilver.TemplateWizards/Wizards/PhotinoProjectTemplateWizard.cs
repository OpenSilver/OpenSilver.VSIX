using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.Shared;
using OpenSilver.TemplateWizards.Utils;
using System.Collections.Generic;
using System.Linq;

namespace OpenSilver.TemplateWizards.Wizards
{
    class PhotinoProjectTemplateWizard : IWizard
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

            var rootProjectName = replacementsDictionary["$safeprojectname$"].TrimSuffix(".Photino");

            var _dte = automationObject as DTE;
            var solution = (Solution2)_dte.Solution;
            var allProjects = WizardUtilities.GetAllProjects(solution);

            var rootProject = allProjects.FirstOrDefault(p => p.Name == rootProjectName);
            var rootProjectExtension = System.IO.Path.GetExtension(rootProject?.FullName ?? "stub.csproj");

            replacementsDictionary.Add("$opensilverphotinopackageversion$", GlobalConstants.OpenSilverPhotinoPackageVersion);
            replacementsDictionary.Add("$rootprojectname$", rootProjectName);
            replacementsDictionary.Add("$rootprojectextension$", rootProjectExtension);

            var sharedDataStore = GlobalWizardDataStore.GetSharedData(replacementsDictionary["$destinationdirectory$"]);
            var netTarget = (DotNetVersion)sharedDataStore[WizardKeys.NetTarget];

            replacementsDictionary.Add("$targetframework$", WizardUtilities.GetNetTarget(netTarget));
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
