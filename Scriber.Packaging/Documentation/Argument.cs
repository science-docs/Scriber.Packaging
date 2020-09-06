using System;
using System.Text;

namespace Scriber.Packaging.Documentation
{
    public class Argument : DocumentationElement
    {
        public Type? Type { get; set; }
        public Type[]? Overrides { get; set; }

        public override string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.Append('`').Append(Name).Append("` ").AppendLine(Type!.FormattedName()).AppendLine();
            
            if (Description != null)
            {
                sb.AppendLine().AppendLine(MarkdownUtility.XmlToMarkdown(Description));
            }

            return sb.ToString();
        }
    }
}
