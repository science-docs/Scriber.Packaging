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
            var packageDirectory = Path.Combine(output, "Packages");
            if (elements.OfType<Package>().Any())
            {
                Directory.CreateDirectory(packageDirectory);
                Directory.Delete(packageDirectory, true);
                Directory.CreateDirectory(packageDirectory);
            }
            var typeDirectory = Path.Combine(output, "Types");
            if (elements.OfType<ObjectType>().Any())
            {
                Directory.CreateDirectory(typeDirectory);
                Directory.Delete(typeDirectory, true);
                Directory.CreateDirectory(typeDirectory);
            }
            foreach (var package in elements.OfType<Package>().OrderBy(e => e.Name))
            {
                File.WriteAllText(Path.Combine(packageDirectory, package.Name + ".md"), package.ToMarkdown());
            }
            foreach (var type in elements.OfType<ObjectType>().OrderBy(e => e.Name))
            {
                File.WriteAllText(Path.Combine(typeDirectory, type.Name + ".md"), type.ToMarkdown());
            }
        }
    }
}
