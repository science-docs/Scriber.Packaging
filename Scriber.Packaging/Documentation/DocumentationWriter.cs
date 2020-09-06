using Microsoft.Build.Execution;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scriber.Packaging.Documentation
{
    public static class DocumentationWriter
    {
        public static void Write(ProjectInstance instance, DocumentationConfig? config, IEnumerable<DocumentationElement> elements)
        {
            var output = Path.Combine(instance.Directory, config?.OutputFolder ?? "docs");
            var dir = Path.Combine(output, "Packages");
            Directory.CreateDirectory(dir);
            Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
            foreach (var package in elements.OfType<Package>().OrderBy(e => e.Name))
            {
                File.WriteAllText(Path.Combine(dir, package.Name! + ".md"), package.ToMarkdown());
            }
        }
    }
}
