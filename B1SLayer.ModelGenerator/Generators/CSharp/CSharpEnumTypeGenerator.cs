using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.CSharp
{
    /// <summary>
    /// Generiert C# Enum-Dateien aus EDMX EnumType-Elementen
    /// </summary>
    internal class CSharpEnumTypeGenerator : CSharpGeneratorBase
    {
        /// <summary>
        /// Initialisiert den Generator mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        public CSharpEnumTypeGenerator(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Generiert einen C# Enum aus einem EDMX EnumType-Element
        /// </summary>
        /// <param name="type">EnumType XElement</param>
        /// <returns>Generierter C# Enum-Code</returns>
        internal override string Generate(XElement type)
        {
            var name = type.Attribute("Name")!.Value;
            var members = type.Elements(EdmNs + "Member");

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic enum {name}");
            sb.AppendLine("\t{");

            foreach (var member in members)
            {
                var memberName = member.Attribute("Name")!.Value;
                var memberValue = member.Attribute("Value")?.Value;

                // Mit Wert: MemberName = 1, ohne Wert: MemberName,
                sb.AppendLine(memberValue != null
                    ? $"\t\t{memberName} = {memberValue},"
                    : $"\t\t{memberName},");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
