using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE80;
using OpenSilver.TemplateWizards.Shared;
using OpenSilver.TemplateWizards.AppCustomizationWindow;

namespace OpenSilver.TemplateWizards.Wizards
{
    class MauiHybridProjectTemplateWizard : IWizard
    {
        private static IEnumerable<Project> GetProjects(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // If this is a solution folder, it can contain other projects
            if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    var subProject = projectItem.SubProject;
                    if (subProject != null)
                    {
                        foreach (var nested in GetProjects(subProject))
                        {
                            yield return nested;
                        }
                    }
                }
            }
            else
            {
                yield return project;
            }
        }

        private static IEnumerable<Project> GetAllProjects(Solution2 solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (Project topLevelProject in solution.Projects)
            {
                foreach (Project p in GetProjects(topLevelProject))
                {
                    yield return p;
                }
            }
        }

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
            var allProjects = GetAllProjects(solution);

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
                return $"{Environment.NewLine}        <TargetFrameworks>{GetNetTarget(netTarget)}-windows10.0.19041.0</TargetFrameworks>";
            }
            else
            {
                return $"{Environment.NewLine}        <TargetFrameworks Condition=\"$([MSBuild]::IsOSPlatform('windows'))\">$(TargetFrameworks);{GetNetTarget(netTarget)}-windows10.0.19041.0</TargetFrameworks>";
            }
        }

        private string GetNetTarget(DotNetVersion netTarget)
        {
            if (netTarget == DotNetVersion.Net7)
            {
                return "net7.0";
            }

            if (netTarget == DotNetVersion.Net8)
            {
                return "net8.0";
            }

            if (netTarget == DotNetVersion.Net9)
            {
                return "net9.0";
            }

            return "";
        }
        private string GetMauiPlatformsBasic(DotNetVersion netTarget, MauiHybridPlatform mhp)
        {
            if (mhp == MauiHybridPlatform.None || mhp == MauiHybridPlatform.Windows)
            {
                return "";
            }

            var netVersion = GetNetTarget(netTarget);
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
