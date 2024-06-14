using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

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
                replacementsDictionary.Add("$opensilverpackageversion$", "2.2.0");
                replacementsDictionary.Add("$openria46packageversion$", "2.2.0");
                replacementsDictionary.Add("$openria54packageversion$", "5.4.3");

                var rng = new Random();

                int sslClientPort;
                int sslServerPort;

                do
                {
                    sslClientPort = rng.Next(50000, 60000);
                } while (!TryAllocatePort(sslClientPort));

                do
                {
                    // iisexpress requires the ssl port be between 44300 and 44399
                    sslServerPort = rng.Next(44300, 44400);
                } while (!TryAllocatePort(sslServerPort));

                replacementsDictionary.Add("$sslclientport$", sslClientPort.ToString());
                replacementsDictionary.Add("$sslserverport$", sslServerPort.ToString());

                var useDatabase = "";
                var dbPackageName = "";
                var dbPackageVersion = "";
                var dbConnectionString = "";
                var projectName = replacementsDictionary["$safeprojectname$"];

                if (viewModel.Database == Database.Sqlite)
                {
                    useDatabase = "UseSqlite";
                    dbPackageName = "Microsoft.EntityFrameworkCore.Sqlite";
                    dbPackageVersion = "8.0.2";
                    dbConnectionString = $"DataSource=Database\\\\{projectName}.db;Cache=Shared";
                }
                else if (viewModel.Database == Database.SqlServer)
                {
                    useDatabase = "UseSqlServer";
                    dbPackageName = "Microsoft.EntityFrameworkCore.SqlServer";
                    dbPackageVersion = "8.0.2";
                    dbConnectionString = $"Server=(localdb)\\\\mssqllocaldb;Database={projectName};Trusted_Connection=True;MultipleActiveResultSets=true";
                }
                else if (viewModel.Database == Database.PostgreSQL)
                {
                    useDatabase = "UseNpgsql";
                    dbPackageName = "Npgsql.EntityFrameworkCore.PostgreSQL";
                    dbPackageVersion = "8.0.2";
                    dbConnectionString = $"Host=localhost;Database={projectName};Username=postgres;Password=postgres";
                }

                replacementsDictionary.Add("$usedatabase$", useDatabase);
                replacementsDictionary.Add("$databasepackagename$", dbPackageName);
                replacementsDictionary.Add("$databasepackageversion$", dbPackageVersion);
                replacementsDictionary.Add("$databaseconnectionstring$", dbConnectionString);
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

        public void SetProjectProperty(Project project, string name, object value)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var supressUI = _dte.SuppressUI;
            _dte.SuppressUI = true;
            try
            {
                Property prop = project.Properties.Item(name);
                if (prop != null) prop.Value = value;
            }
            finally
            {
                _dte.SuppressUI = supressUI;
            }
        }

        public void ProjectFinishedGenerating(Project project)
        {
            Log();

            XElement openSilverInfo = XElement.Parse(_replacementsDictionary["$wizarddata$"]);
            XNamespace defaultNamespace = openSilverInfo.GetDefaultNamespace();

            var language = openSilverInfo.Element(defaultNamespace + "Language").Value;
            var languageCode
                = string.Equals(language, "CSharp", StringComparison.OrdinalIgnoreCase) ? "CS"
                : string.Equals(language, "VisualBasic", StringComparison.OrdinalIgnoreCase) ? "VB"
                : "FS";

            var destinationDirectory = _replacementsDictionary["$destinationdirectory$"];
            var projectName = _replacementsDictionary["$safeprojectname$"];
            var slnPath = Path.Combine(destinationDirectory, $"{projectName}.sln");
            var projectType = viewModel.Backend.ToString();

            // 1. loading the right template
            ThreadHelper.ThrowIfNotOnUIThread();
            var solution = (Solution2)_dte.Solution;

            var prjTemplate = solution.GetProjectTemplate($"{languageCode}{projectType}BusinessApp", language);
            solution.AddFromTemplate(prjTemplate, destinationDirectory, projectName);
            solution.SaveAs(slnPath);

            // 2. fixing missing replacements in files
            var patternsToReplace = new string[] { "*.config", "*.??proj", "*.cs", "*.vb", "*.fs", "*.xaml", "*.as?x", "*.json", "*.htm", "*.html", "*.js", "*.css" };
            var allFiles = EnumerateFiles(destinationDirectory, patternsToReplace);

            foreach (var proj in allFiles)
            {
                try
                {
                    var text = File.ReadAllText(proj);
                    foreach (var item in _replacementsDictionary)
                    {
                        text = text.Replace(item.Key, item.Value)
                            .Replace("$ext_" + item.Key.Substring(1), item.Value);
                    }
                    File.WriteAllText(proj, text);
                }
                catch { }
            }

            // 3. enable iis for migration projects
            if (viewModel.Backend == Backend.Migration)
            {
                var sslServerPort = _replacementsDictionary["$sslserverport$"];

                foreach (Project prj in _dte.Solution.Projects)
                {
                    if (prj.Name.EndsWith(".Web"))
                    {
                        // This will show a warning to user about changing to https url. No problem, because or else the user should do it himself
                        SetProjectProperty(prj, "WebApplication.IISUrl", $"https://localhost:{sslServerPort}/");
                        prj.Save();
                        break;
                    }
                }
            }

            // 4. refresh the solution
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

        // https://github.com/dotnet/templating/blob/main/src/Microsoft.TemplateEngine.Orchestrator.RunnableProjects/Macros/GeneratePortNumberConfig.cs
        private static bool TryAllocatePort(int testPort)
        {
            Socket testSocket = null;
            try
            {
                if (Socket.OSSupportsIPv4)
                {
                    testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                else if (Socket.OSSupportsIPv6)
                {
                    testSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                }

                if (testSocket is null)
                {
                    return false;
                }
                IPEndPoint endPoint = new IPEndPoint(testSocket.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any, testPort);
                testSocket.Bind(endPoint);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                testSocket?.Dispose();
            }
        }

        public static IEnumerable<string> EnumerateFiles(string path, string[] patterns)
        {
            foreach (var pattern in patterns)
                foreach (var file in Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories))
                    yield return file;
        }
    }
}