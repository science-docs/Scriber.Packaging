using System.Linq;
using System.Text;
using System.Xml;

namespace Scriber.Packaging.Documentation
{
    public static class MarkdownUtility
    {
        public static string XmlToMarkdown(string content)
        {
            var xmlContent = "<markdown>" + content + "</markdown>";
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            var root = doc.DocumentElement;
            var markdown = XmlToMarkdown(root.ChildNodes);
            return markdown;
        }

        private static string XmlToMarkdown(XmlNodeList nodes)
        {
            var sb = new StringBuilder();
            foreach (var node in nodes.OfType<XmlNode>())
            {
                sb.Append(XmlToMarkdown(node));
            }
            return sb.ToString();
        }

        private static string XmlToMarkdown(XmlNode node)
        {
            return node switch
            {
                XmlText text => text.Data,
                XmlElement element => XmlToMarkdown(element),
                _ => "",
            };
        }

        /// <summary>
        /// <code lang="cs">var x = 1 + 5;</code>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static string XmlToMarkdown(XmlElement element)
        {
            switch (element.LocalName)
            {
                case "b":
                    return "**" + XmlToMarkdown(element.ChildNodes) + "**";
                case "i":
                    return "*" + XmlToMarkdown(element.ChildNodes) + "*";
                case "c":
                    return "`" + XmlToMarkdown(element.ChildNodes) + "`";
                case "code":
                    const string token = "```";
                    var sb = new StringBuilder();
                    sb.AppendLine().AppendLine();
                    sb.Append(token).AppendLine(element.GetAttribute("lang"));
                    sb.AppendLine(XmlToMarkdown(element.ChildNodes));
                    sb.AppendLine(token).AppendLine();
                    return sb.ToString();
                case "para":
                case "p":
                    sb = new StringBuilder();
                    sb.AppendLine().AppendLine();
                    sb.AppendLine(XmlToMarkdown(element.ChildNodes));
                    sb.AppendLine().AppendLine();
                    return sb.ToString();
                default:
                    return XmlToMarkdown(element.ChildNodes);
            }
        }
    }
}
