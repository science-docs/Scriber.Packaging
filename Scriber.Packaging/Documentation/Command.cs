using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scriber.Packaging.Documentation
{
    public class Command : DocumentationElement
    {
        public List<Argument> Arguments { get; } = new List<Argument>();

        public override string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.Append("### ").AppendLine(Name).AppendLine();

            if (Description != null)
            {
                sb.AppendLine(MarkdownUtility.XmlToMarkdown(Description)).AppendLine();
            }

            sb.AppendLine("```").Append('@').Append(Name).Append('(');

            sb.Append(string.Join(", ", Arguments.Select(e => e.Type!.FormattedName() + " " + e.Name)));

            sb.AppendLine(")").AppendLine("```").AppendLine();

            if (Arguments.Count > 0)
            {
                sb.AppendLine("#### Arguments").AppendLine();
                foreach (var argument in Arguments)
                {
                    sb.AppendLine(argument.ToMarkdown());
                }
            }

            return sb.ToString();
        }
    }
}
