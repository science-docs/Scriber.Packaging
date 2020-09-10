using System;
using System.Collections.Generic;
using System.Reflection;
using static Scriber.Packaging.Names;

namespace Scriber.Packaging.Documentation
{
    public static class DocumentationBuilder
    {
        public static IEnumerable<DocumentationElement> Build(IEnumerable<Assembly> assemblies)
        {
            var visitor = new DocumentationAssemblyVisitor();
            foreach (var asm in assemblies)
            {
                visitor.Visit(asm);
            }
            return visitor.Elements;
        }

        private class DocumentationAssemblyVisitor
        {
            public IEnumerable<DocumentationElement> Elements => elementMap.Values;

            private readonly Dictionary<string, DocumentationElement> elementMap = new Dictionary<string, DocumentationElement>();

            public void Visit(Assembly assembly)
            {
                foreach (var type in assembly.GetTypes())
                {
                    VisitType(type);
                }
            }

            public void VisitType(Type type)
            {
                var packageAttribute = Utility.GetAttribute(type, EngineNamespace + PackageAttribute + "Attribute");

                if (packageAttribute != null)
                {
                    var name = (string)packageAttribute.Name ?? type.FormattedName();
                    var description = (string)packageAttribute.Description;
                    var package = new Package
                    {
                        Name = name,
                        Description = description
                    };
                    if (elementMap.TryGetValue(name, out var packageElement) && packageElement is Package pack)
                    {
                        package = pack;
                    }
                    else
                    {
                        elementMap[name] = package;
                    }

                    foreach (var method in type.GetMethods())
                    {
                        VisitPackageMethod(package, method);
                    }
                }

                var typeAttribute = Utility.GetAttribute(type, EngineNamespace + ObjectTypeAttribute + "Attribute");

                if (typeAttribute != null)
                {
                    var name = (string)typeAttribute.Name ?? type.FormattedName();
                    var description = (string)typeAttribute.Description;
                    var objectType = new ObjectType
                    {
                        Name = name,
                        Description = description
                    };
                    
                    elementMap.Add(name, objectType);

                    foreach (var property in type.GetProperties())
                    {
                        VisitObjectFieldProperty(objectType, property);
                    }
                }
            }

            public void VisitPackageMethod(Package package, MethodInfo method)
            {
                var commandAttribute = Utility.GetAttribute(method, EngineNamespace + CommandAttribute + "Attribute");

                if (commandAttribute != null)
                {
                    var name = (string)commandAttribute.Name ?? method.Name;
                    var description = (string)commandAttribute.Description;
                    var command = new Command
                    {
                        Name = name,
                        Description = description
                    };
                    package.Commands.Add(command);

                    var parameters = method.GetParameters();

                    if (parameters.Length > 0)
                    {
                        var startIndex = 0;

                        if (parameters[0].ParameterType.Name == "CompilerState")
                        {
                            startIndex = 1;
                        }

                        for (int i = startIndex; i < parameters.Length; i++)
                        {
                            VisitArgument(command, parameters[i]);
                        }
                    }
                }

                var envAttribute = Utility.GetAttribute(method, EngineNamespace + EnvironmentAttribute + "Attribute");

                if (envAttribute != null)
                {
                    var name = (string)envAttribute.Name ?? method.Name;
                    var description = (string)envAttribute.Description;
                    var env = new Environment
                    {
                        Name = name,
                        Description = description
                    };
                    package.Environments.Add(env);

                    var parameters = method.GetParameters();

                    if (parameters.Length > 0)
                    {
                        var startIndex = 1;

                        if (parameters[0].ParameterType.Name == "CompilerState")
                        {
                            startIndex = 2;
                        }

                        for (int i = startIndex; i < parameters.Length; i++)
                        {
                            VisitArgument(env, parameters[i]);
                        }
                    }
                }
            }

            public void VisitArgument(Command command, ParameterInfo parameter)
            {
                var argumentAttribute = Utility.GetAttribute(parameter, EngineNamespace + ArgumentAttribute + "Attribute");

                var name = (string?)argumentAttribute?.Name ?? parameter.Name;
                var description = (string?)argumentAttribute?.Description;
                var overrides = (Type[]?)argumentAttribute?.Overrides;
                var argument = new Argument
                {
                    Name = name,
                    Description = description,
                    Type = parameter.ParameterType,
                    Overrides = overrides
                };

                command.Arguments.Add(argument);
            }

            public void VisitObjectFieldProperty(ObjectType type, PropertyInfo property)
            {
                var objectFieldAttribute = Utility.GetAttribute(property, EngineNamespace + ObjectFieldAttribute + "Attribute");

                var name = (string?)objectFieldAttribute?.Name ?? property.Name;
                var description = (string?)objectFieldAttribute?.Description;
                var objectField = new ObjectField
                {
                    Name = name,
                    Description = description
                };

                type.Fields.Add(objectField);
            }
        }
    }
}
