using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JSComponentGeneration.Build.Common;
using JSComponentGeneration.Shared;

namespace JSComponentGeneration.Build.Vue
{
    internal class VueComponentWriter
    {
        private const string EventCallbackTypeName = "Microsoft.AspNetCore.Components.EventCallback";

        private const string VueComponentTemplate =
@"<script setup lang='ts'>
import {{ getCurrentInstance }} from 'vue';
import {{ useBlazor }} from './blazor-vue';

interface Props {{{2}
}}

const props = defineProps<Props>();
const instance = getCurrentInstance();

const {0} = useBlazor(instance, '{1}', props)

</script>

<template>
    <{0} />
</template>
";

        public static void Write(string outputDirectory, RazorComponentDescriptor componentDescriptor)
        {
            var jsTypeNames = componentDescriptor.Parameters
                .Select(p => GetJavaScriptTypeName(p.TypeName))
                .ToArray();

            var identifierName = CasingUtilities.ToKebabCase(componentDescriptor.Name);
            var typedComponentParameterList = GetPropertyList(componentDescriptor.Parameters, jsTypeNames);
            var componentContents = string.Format(
                CultureInfo.InvariantCulture,
                VueComponentTemplate,
                componentDescriptor.Name,
                identifierName,
                typedComponentParameterList);
            var vueComponentFile = $"{outputDirectory}/{componentDescriptor.Name}.vue";

            File.WriteAllText(vueComponentFile, componentContents);
        }

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
                var x when x == $"{typeof(byte).FullName}?" => "number | undefined",
                var x when x == $"{typeof(sbyte).FullName}?" => "number | undefined",
                var x when x == $"{typeof(int).FullName}?" => "number | undefined",
                var x when x == $"{typeof(uint).FullName}?" => "number | undefined",
                var x when x == $"{typeof(short).FullName}?" => "number | undefined",
                var x when x == $"{typeof(ushort).FullName}?" => "number | undefined",
                var x when x == $"{typeof(long).FullName}?" => "number | undefined",
                var x when x == $"{typeof(ulong).FullName}?" => "number | undefined",
                var x when x == $"{typeof(float).FullName}?" => "number | undefined",
                var x when x == $"{typeof(double).FullName}?" => "number | undefined",
                var x when x == $"{typeof(decimal).FullName}?" => "number | undefined",
                var x when x == $"{typeof(bool).FullName}?" => "boolean | undefined",
                var x when x == $"{typeof(char).FullName}?" => "string | undefined",
                var x when x == $"{typeof(string).FullName}?" => "string | undefined",
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
                var x when x == $"{typeof(byte).FullName}?" => "number | undefined",
                var x when x == $"{typeof(sbyte).FullName}?" => "number | undefined",
                var x when x == $"{typeof(int).FullName}?" => "number | undefined",
                var x when x == $"{typeof(uint).FullName}?" => "number | undefined",
                var x when x == $"{typeof(short).FullName}?" => "number | undefined",
                var x when x == $"{typeof(ushort).FullName}?" => "number | undefined",
                var x when x == $"{typeof(long).FullName}?" => "number | undefined",
                var x when x == $"{typeof(ulong).FullName}?" => "number | undefined",
                var x when x == $"{typeof(float).FullName}?" => "number | undefined",
                var x when x == $"{typeof(double).FullName}?" => "number | undefined",
                var x when x == $"{typeof(decimal).FullName}?" => "number | undefined",
                var x when x == $"{typeof(bool).FullName}?" => "boolean | undefined",
                var x when x == $"{typeof(char).FullName}?" => "string | undefined",
                var x when x == $"{typeof(string).FullName}?" => "string | undefined",
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
