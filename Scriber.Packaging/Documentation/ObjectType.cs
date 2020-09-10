using System.Collections.Generic;
using System.Text;

namespace Scriber.Packaging.Documentation
{
    public class ObjectType : DocumentationElement
    {
        public List<ObjectField> Fields { get; } = new List<ObjectField>();

        public override string ToMarkdown()
        {
            var sb = new StringBuilder();

            sb.Append("# ").AppendLine(Name).AppendLine();

            if (Description != null)
            {
                sb.AppendLine(MarkdownUtility.XmlToMarkdown(Description)).AppendLine();
            }

            if (Fields.Count > 0)
            {
                sb.AppendLine("## Fields").AppendLine();

                foreach (var field in Fields)
                {
                    sb.AppendLine(field.ToMarkdown());
                }
            }

            return sb.ToString();
        }
    }
}
