using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;

namespace OpenSilver.TemplateWizards
{

    public static class Helpers
    {
        public static void Populate(this Dictionary<string, string> replacementsDictionary, object instance, bool includeExtraExtItem = true)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (replacementsDictionary == null)
            {
                throw new ArgumentNullException(nameof(replacementsDictionary));
            }

            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var key = property.Name.ToLower();
                var value = $"{property.GetValue(instance)}";
                replacementsDictionary.Add($"${key}$", value);
                if (includeExtraExtItem)
                {
                    replacementsDictionary.Add($"$ext_{key}$", value);
                }
            }
        }

        public static T LoadEmbeddedResource<T>(string resourceName)
        {
            var json = LoadEmbeddedResource(resourceName);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string LoadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;

            using (var stream = assembly.GetManifestResourceStream($"{assemblyName}.{resourceName}"))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Embedded resource not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static IEnumerable<string> EnumerateFiles(string path, string[] patterns)
        {
            foreach (var pattern in patterns)
                foreach (var file in Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories))
                    yield return file;
        }

        private static readonly Random rng = new Random();

        public static int GetPortFromRange(int start, int end)
        {
            int port;
            do
            {
                port = rng.Next(start, end);
            } while (!TryAllocatePort(port));
            return port;
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
    }
}