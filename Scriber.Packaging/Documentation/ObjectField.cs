using System;
using System.Text;

namespace Scriber.Packaging.Documentation
{
    public class ObjectField : DocumentationElement
    {
        public Type? Type { get; set; }

        public override string ToMarkdown()
        {
            var sb = new StringBuilder();

            sb.Append('`').Append(Name).Append("` ").AppendLine(Type!.FormattedName()).AppendLine();

            if (Description != null)
            {
                sb.AppendLine(MarkdownUtility.XmlToMarkdown(Description)).AppendLine();
            }

            return sb.ToString();
        }
    }
}
