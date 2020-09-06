using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Scriber.Packaging.Syntax
{
    public static class SyntaxUtility
    {
        public static ParameterSyntax? GetParameterByName(this MethodDeclarationSyntax method, string name)
        {
            foreach (var param in method.ParameterList.Parameters)
            {
                if (param.Identifier.ValueText == name)
                {
                    return param;
                }
            }
            return null;
        }

        public static DocumentationCommentTriviaSyntax? GetTrivia(this CSharpSyntaxNode node)
        {
            if (node.HasLeadingTrivia)
            {
                var leading = node.GetLeadingTrivia();
                var doc = leading.ToSyntaxTriviaList().FirstOrDefault(e => e.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
                if (doc != null && doc.GetStructure() is DocumentationCommentTriviaSyntax commentTrivia)
                {
                    return commentTrivia;
                }
            }
            return null;
        }

        public static AttributeSyntax? GetAttributeByName(this SyntaxList<AttributeListSyntax> attributeLists, params string[] names)
        {
            var hashSet = new HashSet<string>(names);
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    if (hashSet.Contains(attribute.Name.ToString()))
                    {
                        return attribute;
                    }
                }
            }
            return null;
        }

        public static string? GetNameArgument(this AttributeSyntax attribute)
        {
            if (attribute.ArgumentList != null)
            {
                foreach (var argument in attribute.ArgumentList.Arguments)
                {
                    if (argument.NameEquals == null || argument.NameEquals?.Name?.ToString() == "Name")
                    {
                        return argument.Expression.ToString().Trim('"');
                    }
                }
            }
            return null;
        }

        public static string? GetDescription(this AttributeSyntax attribute)
        {
            if (attribute.ArgumentList != null)
            {
                foreach (var argument in attribute.ArgumentList.Arguments)
                {
                    if (argument.NameEquals?.Name?.ToString() == Names.DescriptionArgument)
                    {
                        return argument.Expression.ToString().Trim('"');
                    }
                }
            }
            return null;
        }
    }
}
