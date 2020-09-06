using System;
using System.Collections.Generic;
using System.Text;

namespace Scriber.Packaging.Documentation
{
    public class Package : DocumentationElement
    {
        public List<Command> Commands { get; } = new List<Command>();
        public List<Environment> Environments { get; } = new List<Environment>();

        public override string ToMarkdown()
        {
            var sb = new StringBuilder();

            sb.Append("# ").AppendLine(Name).AppendLine();

            if (Description != null)
            {
                sb.AppendLine(MarkdownUtility.XmlToMarkdown(Description)).AppendLine();
            }

            if (Commands.Count > 0)
            {
                sb.AppendLine("## Commands").AppendLine();

                foreach (var command in Commands)
                {
                    sb.AppendLine(command.ToMarkdown());
                }
            }

            if (Environments.Count > 0)
            {
                sb.AppendLine("## Environments").AppendLine();

                foreach (var env in Environments)
                {
                    sb.AppendLine(env.ToMarkdown());
                }
            }

            return sb.ToString();
        }
    }
}
