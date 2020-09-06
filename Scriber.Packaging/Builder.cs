using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scriber.Packaging
{
    public static class Builder
    {
        public static string[] FindCompilationFiles(ProjectInstance instance)
        {
            return instance.Items.Where(e => e.ItemType == "Compile" && e.EvaluatedInclude.EndsWith(".cs")).Select(e => e.EvaluatedInclude).ToArray();
        }

        public static Dictionary<string, string> ReplaceFileContents(Dictionary<string, string> files)
        {
            var dict = new Dictionary<string, string>();
            var encoding = new System.Text.UTF8Encoding(true);
            foreach (var key in files.Keys)
            {
                var oldContent = File.ReadAllText(key);
                dict[key] = oldContent;
                File.WriteAllText(key, files[key], encoding);
            }
            return dict;
        }

        public static string[] Build(ProjectInstance instance)
        {
            var logger = new ConsoleLogger();
            var buildParameters = new BuildParameters()
            {
                DetailedSummary = true,
                Loggers = new ILogger[] { logger }
            };
            var result = BuildManager.DefaultBuildManager.Build(buildParameters, new BuildRequestData(instance, instance.DefaultTargets.ToArray()));
            var singleResult = result.ResultsByTarget[instance.DefaultTargets.First()];
            return singleResult.Items.Select(e => e.ItemSpec).Where(e => e.EndsWith(".dll")).ToArray();
        }
    }
}
