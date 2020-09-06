using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scriber.Packaging.Syntax
{
    public static class XmlSyntaxUtility
    {
        public static XmlElementSyntax? Element(this DocumentationCommentTriviaSyntax trivia, string name)
        {
            return trivia.Elements(name).FirstOrDefault();
        }

        public static IEnumerable<XmlElementSyntax> Elements(this DocumentationCommentTriviaSyntax trivia, string name)
        {
            var content = trivia.Content;
            var items = content.OfType<XmlElementSyntax>().ToArray();
            return items.Where(e => e.StartTag.Name.LocalName.ValueText == name);
        }

        public static string StringContent(this XmlElementSyntax xml)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var element in xml.Content)
            {
                if (element is XmlTextSyntax textSyntax)
                {
                    foreach (var token in textSyntax.TextTokens)
                    {
                        sb.Append(token.ValueText);
                    }
                }
                else if (element is XmlElementSyntax elementSyntax)
                {
                    sb.Append(elementSyntax.StartTag.ToFullString());
                    sb.Append(elementSyntax.StringContent());
                    sb.Append(elementSyntax.EndTag.ToFullString());
                }
            }

            return sb.ToString().Trim();
        }

        public static string? GetAttribute(this XmlElementSyntax xmlElement, string attributeName)
        {
            foreach (var attribute in xmlElement.StartTag.Attributes.OfType<XmlNameAttributeSyntax>())
            {
                if (attribute.Name.LocalName.ValueText == attributeName)
                {
                    return attribute.Identifier.Identifier.ValueText;
                }
            }
            return null;
        }
    }
}
