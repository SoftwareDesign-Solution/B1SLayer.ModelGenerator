using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.TypeScript
{
    /// <summary>
    /// Generiert TypeScript Enum-Dateien aus EDMX EnumType-Elementen
    /// </summary>
    internal class TypeScriptEnumTypeGenerator : TypeScriptGeneratorBase
    {
        /// <summary>
        /// Initialisiert den Generator mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        public TypeScriptEnumTypeGenerator(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Generiert einen TypeScript Enum aus einem EDMX EnumType-Element
        /// </summary>
        /// <param name="type">EnumType XElement</param>
        /// <returns>Generierter TypeScript Enum-Code</returns>
        protected override string Generate(XElement type)
        {
            var name = type.Attribute("Name")!.Value;
            var members = type.Elements(EdmNs + "Member");

            var sb = new StringBuilder();
            sb.AppendLine($"export enum {name} {{");

            foreach (var member in members)
            {
                var memberName = member.Attribute("Name")!.Value;
                var memberValue = member.Attribute("Value")?.Value;

                // Mit Wert: MemberName = 1, ohne Wert: MemberName,
                sb.AppendLine(memberValue != null
                    ? $"\t{memberName} = {memberValue},"
                    : $"\t{memberName},");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
