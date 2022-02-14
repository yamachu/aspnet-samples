using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JSComponentGeneration.Build.Common;
using JSComponentGeneration.Shared;

namespace JSComponentGeneration.Build.React
{
    internal class ReactComponentWriter
    {
        private const string EventCallbackTypeName = "Microsoft.AspNetCore.Components.EventCallback";

        private const string ReactComponentTemplate =
@"import React from 'react';
import {{ useBlazor }} from './blazor-react';

interface {0}Props {{{1}
}}

export const {0}: React.VFC<{0}Props> = ({{{2}
}}) => {{
  const fragment = useBlazor('{3}', {{{4}
  }});

  return fragment;
}}
";

        public static void Write(string outputDirectory, RazorComponentDescriptor componentDescriptor)
        {
            var jsTypeNames = componentDescriptor.Parameters
                .Select(p => GetJavaScriptTypeName(p.TypeName))
                .ToArray();

            var identifierName = CasingUtilities.ToKebabCase(componentDescriptor.Name);
            var typedComponentParameterList = GetPropertyList(componentDescriptor.Parameters, jsTypeNames);
            var componentParameterList = GetParameterList(componentDescriptor.Parameters, 1);
            var useBlazorParameterList = GetParameterList(componentDescriptor.Parameters, 2);
            var componentContents = string.Format(
                CultureInfo.InvariantCulture,
                ReactComponentTemplate,
                componentDescriptor.Name,
                typedComponentParameterList,
                componentParameterList,
                identifierName,
                useBlazorParameterList);
            var reactComponentFile = $"{outputDirectory}/generated/{componentDescriptor.Name}.ts";

            File.WriteAllText(reactComponentFile, componentContents);
        }

        private static string GetParameterList(IReadOnlyList<BoundAttributeDescriptor> parameters, int indentLevel)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < parameters.Count; i++)
            {
                var camelCaseName = CasingUtilities.PascalToCamelCase(parameters[i].Name);

                stringBuilder.AppendLine();

                for (var j = 0; j < indentLevel; j++)
                {
                    stringBuilder.Append("  ");
                }

                stringBuilder.Append(camelCaseName);
                stringBuilder.Append(',');
            }

            return stringBuilder.ToString();
        }

        // From AngularComponentWriter.cs
        private static string GetPropertyList(IReadOnlyList<BoundAttributeDescriptor> parameters, string[] jsTypeNames)
        {
            Debug.Assert(parameters.Count == jsTypeNames.Length);

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < parameters.Count; i++)
            {
                var camelCaseName = CasingUtilities.PascalToCamelCase(parameters[i].Name);
                var typeName = jsTypeNames[i];

                stringBuilder.Append("\n  ");
                stringBuilder.Append(camelCaseName);
                stringBuilder.Append(": ");
                stringBuilder.Append(typeName);
                stringBuilder.Append(";");
            }

            return stringBuilder.ToString();
        }

        private static string GetJavaScriptTypeName(string cSharpTypeName)
            => cSharpTypeName switch
            {
                var x when x == typeof(byte).FullName => "number",
                var x when x == typeof(sbyte).FullName => "number",
                var x when x == typeof(int).FullName => "number",
                var x when x == typeof(uint).FullName => "number",
                var x when x == typeof(short).FullName => "number",
                var x when x == typeof(ushort).FullName => "number",
                var x when x == typeof(long).FullName => "number",
                var x when x == typeof(ulong).FullName => "number",
                var x when x == typeof(float).FullName => "number",
                var x when x == typeof(double).FullName => "number",
                var x when x == typeof(decimal).FullName => "number",
                var x when x == typeof(bool).FullName => "boolean",
                var x when x == typeof(char).FullName => "string",
                var x when x == typeof(string).FullName => "string",
                var x when x.StartsWith(EventCallbackTypeName) => GetJavaScriptEventCallbackType(x),
                _ => "object"
            };

        private static string GetJavaScriptEventCallbackType(string cSharpTypeName)
        {
            var parameterType = cSharpTypeName.Remove(0, EventCallbackTypeName.Length).TrimStart('<').TrimEnd('>') switch
            {
                var x when x == typeof(byte).FullName => "number",
                var x when x == typeof(sbyte).FullName => "number",
                var x when x == typeof(int).FullName => "number",
                var x when x == typeof(uint).FullName => "number",
                var x when x == typeof(short).FullName => "number",
                var x when x == typeof(ushort).FullName => "number",
                var x when x == typeof(long).FullName => "number",
                var x when x == typeof(ulong).FullName => "number",
                var x when x == typeof(float).FullName => "number",
                var x when x == typeof(double).FullName => "number",
                var x when x == typeof(decimal).FullName => "number",
                var x when x == typeof(bool).FullName => "boolean",
                var x when x == typeof(char).FullName => "string",
                var x when x == typeof(string).FullName => "string",
                var x when x == "" => "",
                _ => "object"
            };

            if (parameterType == "")
            {
                return "() => void";
            }

            return string.Format("(_: {0}) => void", parameterType);
        }
    }
}
