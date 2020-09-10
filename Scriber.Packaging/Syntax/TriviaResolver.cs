using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Scriber.Packaging.Names;

namespace Scriber.Packaging.Syntax
{
    public static class TriviaResolver
    {
        public static SyntaxTree Resolve(SyntaxTree tree)
        {
            var root = tree.GetRoot();
            var walker = new TriviaSyntaxRewriter();
            return walker.Visit(root).SyntaxTree;
        }

        private class TriviaSyntaxRewriter : CSharpSyntaxRewriter
        {
            public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
            {
                var packageAttribute = classDeclaration.AttributeLists.GetAttributeByName(PackageAttribute);
                var trivia = classDeclaration.GetTrivia();
                if (packageAttribute != null && trivia != null)
                {
                    var summaryElement = trivia.Element("summary");

                    if (summaryElement != null)
                    {
                        var summary = summaryElement.StringContent();
                        classDeclaration = SetDescription(packageAttribute, summary, classDeclaration);
                    }
                }

                var typeAttribute = classDeclaration.AttributeLists.GetAttributeByName(ObjectTypeAttribute);
                if (typeAttribute != null && trivia != null)
                {
                    var summaryElement = trivia.Element("summary");

                    if (summaryElement != null)
                    {
                        var summary = summaryElement.StringContent();
                        classDeclaration = SetDescription(typeAttribute, summary, classDeclaration);
                    }
                }

                return base.VisitClassDeclaration(classDeclaration);
            }

            public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax methodDeclaration)
            {
                var attribute = methodDeclaration.AttributeLists.GetAttributeByName(CommandAttribute, EnvironmentAttribute);
                var trivia = methodDeclaration.GetTrivia();
                if (attribute != null && trivia != null)
                {
                    var summaryElement = trivia.Element("summary");

                    if (summaryElement != null)
                    {
                        var summary = summaryElement.StringContent();
                        methodDeclaration = SetDescription(attribute, summary, methodDeclaration);
                    }
                    var paramElements = trivia.Elements("param");

                    foreach (var paramElement in paramElements)
                    {
                        var paramName = paramElement.GetAttribute("name");
                        var paramValue = paramElement.StringContent();
                        if (paramName != null)
                        {
                            var parameter = methodDeclaration.GetParameterByName(paramName);
                            if (parameter != null)
                            {
                                methodDeclaration = methodDeclaration.ReplaceNode(parameter, SetArgumentDescription(parameter, paramValue));
                            }
                        }
                    }
                }
                
                return base.VisitMethodDeclaration(methodDeclaration);
            }

            public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax propertyDeclaration)
            {
                var attribute = propertyDeclaration.AttributeLists.GetAttributeByName(ObjectFieldAttribute);
                var trivia = propertyDeclaration.GetTrivia();

                if (attribute != null && trivia != null)
                {
                    var summaryElement = trivia.Element("summary");

                    if (summaryElement != null)
                    {
                        var summary = summaryElement.StringContent();
                        propertyDeclaration = SetDescription(attribute, summary, propertyDeclaration);
                    }
                }

                return base.VisitPropertyDeclaration(propertyDeclaration);
            }

            private ParameterSyntax SetArgumentDescription(ParameterSyntax parameter, string value)
            {
                var argument = parameter.AttributeLists.GetAttributeByName(ArgumentAttribute);
                if (argument == null)
                {
                    argument = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName(ArgumentAttribute));
                    argument = SetDescription(argument, value);
                    var newList = SyntaxFactory.AttributeList().AddAttributes(argument);
                    parameter = parameter.AddAttributeLists(newList);
                }
                else if (!HasDescription(argument))
                {
                    parameter = parameter.ReplaceNode(argument, SetDescription(argument, value));
                }

                return parameter;
            }

            private T SetDescription<T>(AttributeSyntax attribute, string value, T node) where T : SyntaxNode
            {
                node = node.ReplaceNode(attribute, SetDescription(attribute, value));
                return node;
            }

            private AttributeSyntax SetDescription(AttributeSyntax attribute, string value)
            {
                if (!HasDescription(attribute))
                {
                    var newAttribute = attribute;
                    var argumentList = attribute.ArgumentList;

                    if (argumentList == null)
                    {
                        argumentList = SyntaxFactory.AttributeArgumentList();
                        newAttribute = newAttribute.WithArgumentList(argumentList);
                    }

                    var valueExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
                    var nameEquals = SyntaxFactory.NameEquals(DescriptionArgument);
                    var descriptionArgument = SyntaxFactory.AttributeArgument(nameEquals, null, valueExpression);
                    newAttribute = newAttribute.WithArgumentList(argumentList.AddArguments(descriptionArgument));
                    return newAttribute;
                }
                return attribute;
            }

            private bool HasDescription(AttributeSyntax attribute)
            {
                if (attribute.ArgumentList != null)
                {
                    foreach (var argument in attribute.ArgumentList.Arguments)
                    {
                        if (argument.NameEquals?.ToString() == DescriptionArgument)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
