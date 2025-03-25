using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE80;
using OpenSilver.TemplateWizards.Shared;
using OpenSilver.TemplateWizards.Utils;

namespace OpenSilver.TemplateWizards.Wizards
{
    class MauiHybridProjectTemplateWizard : IWizard
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

            var rootProjectName = replacementsDictionary["$safeprojectname$"].TrimSuffix(".MauiHybrid");

            var _dte = automationObject as DTE;
            var solution = (Solution2)_dte.Solution;
            var allProjects = WizardUtilities.GetAllProjects(solution);

            var rootProject = allProjects.FirstOrDefault(p => p.Name == rootProjectName);
            var rootProjectExtension = System.IO.Path.GetExtension(rootProject?.FullName ?? "stub.csproj");

            replacementsDictionary.Add("$opensilvermauihybridpackageversion$", GlobalConstants.OpenSilverMauiHybridPackageVersion);
            replacementsDictionary.Add("$appidentifier$", "com.companyname." + rootProjectName.ToLowerInvariant());
            replacementsDictionary.Add("$rootprojectname$", rootProjectName);
            replacementsDictionary.Add("$rootprojectextension$", rootProjectExtension);

            var sharedDataStore = GlobalWizardDataStore.GetSharedData(replacementsDictionary["$destinationdirectory$"]);
            var mhp = (MauiHybridPlatform)sharedDataStore[WizardKeys.MauiTargets];
            var netTarget = (DotNetVersion)sharedDataStore[WizardKeys.NetTarget];

            replacementsDictionary.Add("$mauiplatformsbasic$", GetMauiPlatformsBasic(netTarget, mhp));
            replacementsDictionary.Add("$windowsTarget$", GetWindowsTarget(netTarget, mhp));
            replacementsDictionary.Add("$mauitargets$", mhp.ToString());
        }

        private string GetWindowsTarget(DotNetVersion netTarget, MauiHybridPlatform mhp)
        {
            if ((mhp & MauiHybridPlatform.Windows) != MauiHybridPlatform.Windows)
            {
                return "";
            }

            if (mhp == MauiHybridPlatform.Windows)
            {
                //Only windows
                //return $"{Environment.NewLine}        <TargetFrameworks Condition=\"$([MSBuild]::IsOSPlatform('windows'))\">{GetNetTarget(netTarget)}-windows10.0.19041.0</TargetFrameworks>";
                return $"{Environment.NewLine}        <TargetFrameworks>{WizardUtilities.GetNetTarget(netTarget)}-windows10.0.19041.0</TargetFrameworks>";
            }
            else
            {
                return $"{Environment.NewLine}        <TargetFrameworks Condition=\"$([MSBuild]::IsOSPlatform('windows'))\">$(TargetFrameworks);{WizardUtilities.GetNetTarget(netTarget)}-windows10.0.19041.0</TargetFrameworks>";
            }
        }

        private string GetMauiPlatformsBasic(DotNetVersion netTarget, MauiHybridPlatform mhp)
        {
            if (mhp == MauiHybridPlatform.None || mhp == MauiHybridPlatform.Windows)
            {
                return "";
            }

            var netVersion = WizardUtilities.GetNetTarget(netTarget);
            var targets = new List<string>();

            if ((mhp & MauiHybridPlatform.Android) == MauiHybridPlatform.Android)
            {
                targets.Add($"{netVersion}-android");
            }

            if ((mhp & MauiHybridPlatform.iOS) == MauiHybridPlatform.iOS)
            {
                targets.Add($"{netVersion}-ios");
            }

            if ((mhp & MauiHybridPlatform.Mac) == MauiHybridPlatform.Mac)
            {
                targets.Add($"{netVersion}-maccatalyst");
            }

            if (targets.Count == 0)
            {
                return "";
            }

            return $"{Environment.NewLine}        <TargetFrameworks>{string.Join(";", targets)}</TargetFrameworks>";
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
