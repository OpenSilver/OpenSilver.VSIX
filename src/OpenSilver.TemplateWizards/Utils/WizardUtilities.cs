using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using OpenSilver.TemplateWizards.Shared;
using System.Collections.Generic;

namespace OpenSilver.TemplateWizards.Utils
{
    /// <summary>
    /// Utility class providing common functionality for wizards
    /// </summary>
    public static class WizardUtilities
    {
        /// <summary>
        /// Gets all projects within a given project, including those in solution folders
        /// </summary>
        /// <param name="project">The project to search</param>
        /// <returns>Enumeration of all projects</returns>
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

        /// <summary>
        /// Gets all projects within a solution
        /// </summary>
        /// <param name="solution">The solution to search</param>
        /// <returns>Enumeration of all projects in the solution</returns>
        public static IEnumerable<Project> GetAllProjects(Solution2 solution)
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

        /// <summary>
        /// Converts a DotNetVersion enum value to its corresponding target framework moniker string
        /// </summary>
        /// <param name="netTarget">The .NET version to convert</param>
        /// <returns>The target framework moniker as a string (e.g., "net8.0")</returns>
        public static string GetNetTarget(DotNetVersion netTarget)
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
    }
}
