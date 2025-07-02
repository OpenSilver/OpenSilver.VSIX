using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using OpenSilver.TemplateWizards.AppCustomizationWindow;
using OpenSilver.TemplateWizards.Shared;
using OpenSilver.TemplateWizards.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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

            var window = new LibraryCustomizationWindow();
            bool? result = window.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                if (replacementsDictionary.TryGetValue("$destinationdirectory$", out var destDir)
                    && Directory.Exists(destDir))
                {
                    var msgResult = MessageBox.Show(
                        $"The folder \"{destDir}\" has already been created. Do you want to delete it?",
                        "Delete Created Folder?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (msgResult == DialogResult.Yes)
                    {
                        try
                        {
                            Directory.Delete(destDir, recursive: true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $"Failed to delete the folder:\n{ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }

                throw new WizardBackoutException("Class library creation was cancelled by user");
            }

            replacementsDictionary.Add("$opensilverpackageversion$", GlobalConstants.OpenSilverPackageVersion);
            replacementsDictionary.Add("$targetframework$", EnumUtilities.GetEnumDescription(window.DotNetVersion));
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }
    }
}
