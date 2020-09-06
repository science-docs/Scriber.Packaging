using System;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Scriber.Packaging
{
    public static class Utility
    {
        public static string FormattedName(this Type type)
        {
            var name = type.Name;
            if (type.IsGenericType)
            {
                name = name.Substring(0, name.IndexOf('`'));
            }

            var typeAttribute = GetAttribute(type, "Scriber.Engine.ObjectTypeAttribute");

            if (typeAttribute != null)
            {
                name = (string)typeAttribute.Name;
            }

            if (type.IsGenericType)
            {
                if (name == "Argument" && type.GenericTypeArguments.Length == 1)
                {
                    return type.GenericTypeArguments.First().FormattedName();
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append(name);
                    sb.Append('<');
                    sb.Append(string.Join(", ", type.GenericTypeArguments.Select(e => e.FormattedName())));
                    sb.Append(">");
                    return sb.ToString();
                }
            }
            else
            {
                return name;
            }
        }

        public static dynamic? GetAttribute(MemberInfo member, string name)
        {
            foreach (var attr in member.GetCustomAttributes<Attribute>())
            {
                if (attr.GetType().FullName == name)
                {
                    return attr;
                }
            }
            return null;
        }

        public static dynamic? GetAttribute(ParameterInfo member, string name)
        {
            foreach (var attr in member.GetCustomAttributes<Attribute>())
            {
                if (attr.GetType().FullName == name)
                {
                    return attr;
                }
            }
            return null;
        }
    }
}
