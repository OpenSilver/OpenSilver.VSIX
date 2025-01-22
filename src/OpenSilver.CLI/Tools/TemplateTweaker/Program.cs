using System;
using System.IO;

namespace TemplateTweaker
{
    public static class Program
    {
        public static void Main()
        {
            // If you have multiple "common" folders, list them all here:
            var sourceFolders = new[] 
            {
                "CommonProjectTemplates/OpenSilverApplication.MauiHybrid"
                // Add more sources if needed...
            };

            // List all the template folders that should receive the "common" content:
            var destinationFolders = new[]
            {
                "C#/OpenSilverApplication"
                // Add more destinations if needed...
            };

            foreach (var sourceFolder in sourceFolders)
            {
                if (!Directory.Exists(sourceFolder))
                {
                    Console.WriteLine($"[WARN] Source folder not found: {sourceFolder}");
                    continue;
                }

                // We'll copy the entire directory named sourceFolder into each destination.
                // For example, if sourceFolder = "CommonProjectTemplates",
                // it ends up as e.g. Template1/CommonProjectTemplates
                var subfolderName = Path.GetFileName(sourceFolder);

                foreach (var destFolder in destinationFolders)
                {
                    if (!Directory.Exists(destFolder))
                    {
                        Console.WriteLine($"[WARN] Destination folder not found: {destFolder}");
                        continue;
                    }

                    var finalPath = Path.Combine(destFolder, subfolderName);

                    // 1) Remove any existing folder to avoid stale files
                    if (Directory.Exists(finalPath))
                    {
                        Console.WriteLine($"Removing existing folder: {finalPath}");
                        Directory.Delete(finalPath, recursive: true);
                    }

                    // 2) Copy the folder
                    Console.WriteLine($"Copying '{sourceFolder}' into '{destFolder}'...");
                    DirectoryCopy(sourceFolder, finalPath);
                }
            }

            Console.WriteLine("All done!");
        }

        /// <summary>
        /// Recursively copies a directory (all subdirs/files) to a new location.
        /// </summary>
        private static void DirectoryCopy(string sourceDir, string destDir)
        {
            // Create destination directory
            Directory.CreateDirectory(destDir);

            // Copy all files in the current level
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, overwrite: true);
            }

            // Copy all subdirectories (recursively)
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var subDirName = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destDir, subDirName);
                DirectoryCopy(subDir, destSubDir);
            }
        }
    }
}
