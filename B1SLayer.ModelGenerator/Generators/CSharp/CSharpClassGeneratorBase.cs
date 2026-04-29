using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.CSharp
{
    /// <summary>
    /// Abstrakte Basisklasse für C#-Klassen-Generatoren (ComplexType, EntityType).
    /// Enthält die gemeinsame Logik für die Klassen- und Property-Generierung
    /// sowie die EDM-zu-CSharp Typ-Konvertierung.
    /// </summary>
    internal abstract class CSharpClassGeneratorBase : CSharpGeneratorBase
    {
        /// <summary>
        /// Initialisiert die Basisklasse mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        protected CSharpClassGeneratorBase(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Generiert eine C#-Klasse mit Properties aus den EDMX-Metadaten
        /// </summary>
        /// <param name="name">Klassenname</param>
        /// <param name="properties">Liste der Property-XElements</param>
        /// <returns>Generierter C#-Code als String</returns>
        protected string GenerateClass(string name, IEnumerable<XElement> properties)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"\tpublic class {name}");
            sb.AppendLine("\t{");

            foreach (var prop in properties)
            {
                var propName = prop.Attribute("Name")!.Value;
                var propType = prop.Attribute("Type")!.Value;

                // Nullable-Default laut OData-Spezifikation ist true
                var nullable = prop.Attribute("Nullable")?.Value ?? "true";
                var csharpType = MapEdmTypeToCSharp(propType, nullable);

                sb.AppendLine($"\t\tpublic {csharpType} {propName} {{ get; set; }}");
            }

            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Konvertiert einen EDM-Typ in den entsprechenden C#-Typ.
        /// Unterstützt Collection-Typen, Namespace-Prefixe und Nullable-Typen.
        /// </summary>
        /// <param name="edmType">EDM-Typ z.B. Edm.String, Collection(SAPB1.TeamMember)</param>
        /// <param name="nullable">Nullable-Attributwert aus EDMX</param>
        /// <returns>C#-Typ als String</returns>
        protected static string MapEdmTypeToCSharp(string edmType, string nullable)
        {

            // Collection(Namespace.TypeName) → List<TypeName>
            if (edmType.StartsWith("Collection(") && edmType.EndsWith(")"))
            {
                var innerType = edmType[11..^1];                                  // Namespace.TypeName
                var mappedInner = MapEdmTypeToCSharp(innerType, "false"); // rekursiv, inner ist nie nullable
                return $"List<{mappedInner}>";                                          // Nullable spielt keine Rolle bei Listen
            }

            // Namespace-Prefix entfernen z.B. SAPB1.DocumentLine → DocumentLine
            var cleanType = edmType.Contains('.')
                ? edmType[(edmType.LastIndexOf('.') + 1)..]
                : edmType;

            var isNullable = nullable != "false";

            // EDM-Typ auf C#-Typ mappen
            var baseType = cleanType.Replace("Edm.", "") switch
            {
                "String" => "string",
                "Int16" => "short",
                "Int32" => "int",
                "Int64" => "long",
                "Decimal" => "decimal",
                "Double" => "double",
                "Single" => "float",
                "Boolean" => "bool",
                "DateTime" => "DateTime",
                "DateTimeOffset" => "DateTimeOffset",
                "Guid" => "Guid",
                "Binary" => "byte[]",
                _ => cleanType // Unbekannte Typen unverändert lassen
            };

            // Nur Wertetypen bekommen ein ? — Referenztypen sind bereits nullable
            var isValueType = baseType is not ("string" or "byte[]" or "object");

            return isNullable && isValueType
                ? $"{baseType}?"
                : baseType;
        }
    }
}
