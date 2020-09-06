using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis.CSharp;
using Scriber.Packaging.Documentation;
using Scriber.Packaging.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Scriber.Packaging
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = args.Length > 0 ? args[0] : System.Environment.CurrentDirectory;
            var configPath = Path.Combine(directory, "scriber.packaging.json");
            var config = new Config();

            if (File.Exists(configPath))
            {
                var text = File.ReadAllText(configPath);
                config = System.Text.Json.JsonSerializer.Deserialize<Config>(text);
            }

            if (SetMsBuildExePath())
            {
                var csproj = Directory.GetFiles(directory, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

                if (csproj != null)
                {
                    var instance = new ProjectInstance(csproj);
                    ProcessProject(config, instance);
                }
            }
        }

        private static void ProcessProject(Config config, ProjectInstance instance)
        {
            if (config.Build?.Enabled ?? true)
            {
                RestorePackages(instance.FullPath);
            }

            Dictionary<string, string>? oldContents = null;
            if (config.Rewrite?.Enabled ?? true)
            {
                var files = Builder.FindCompilationFiles(instance);

                var dict = new Dictionary<string, string>();
                foreach (var file in files)
                {
                    var fullPath = Path.Combine(instance.Directory, file);
                    dict[fullPath] = ProcessFile(fullPath);
                }
                oldContents = Builder.ReplaceFileContents(dict);
            }

            if (config.Build?.Enabled ?? true)
            {
                var dlls = Builder.Build(instance);

                if (config.Documentation?.Enabled ?? true)
                {
                    var assemblies = dlls.Select(e => new AssemblyResolver(e).InitialAssembly).ToHashSet();
                    var elements = DocumentationBuilder.Build(assemblies).ToArray();
                    DocumentationWriter.Write(instance, config.Documentation, elements);
                }
            }

            if (oldContents != null)
            {
                Builder.ReplaceFileContents(oldContents);
            }
        }

        private static string ProcessFile(string file)
        {
            var text = File.ReadAllText(file);
            var ast = CSharpSyntaxTree.ParseText(text, null, file);
            ast = TriviaResolver.Resolve(ast);
            return ast.GetRoot().ToString();
        }

        private static bool SetMsBuildExePath()
        {
            try
            {
                var startInfo = new ProcessStartInfo("dotnet", "--list-sdks")
                {
                    RedirectStandardOutput = true
                };

                var process = Process.Start(startInfo);
                process.WaitForExit(1000);

                var output = process.StandardOutput.ReadToEnd();
                var sdkPaths = Regex.Matches(output, "([0-9]+.[0-9]+.[0-9]+) \\[(.*)\\]")
                    .OfType<Match>()
                    .Select(m => Path.Combine(m.Groups[2].Value, m.Groups[1].Value, "MSBuild.dll"));

                var sdkPath = sdkPaths.Last();
                System.Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", sdkPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not get dotnet sdk: " + ex.Message);
                return false;
            }
        }

        private static void RestorePackages(string solutionPath)
        {
            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"restore {solutionPath}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
