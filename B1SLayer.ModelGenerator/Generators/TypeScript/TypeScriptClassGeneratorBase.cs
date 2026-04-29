using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.TypeScript
{

    /// <summary>
    /// Abstrakte Basisklasse für TypeScript-Klassen-Generatoren (ComplexType, EntityType).
    /// Enthält die gemeinsame Logik für die Type-Generierung
    /// sowie die EDM-zu-TypeScript Typ-Konvertierung.
    /// </summary>
    internal abstract class TypeScriptClassGeneratorBase : TypeScriptGeneratorBase
    {
        /// <summary>
        /// Initialisiert die Basisklasse mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        protected TypeScriptClassGeneratorBase(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Generiert einen TypeScript Type aus den EDMX-Metadaten
        /// </summary>
        /// <param name="name">Type-Name</param>
        /// <param name="properties">Liste der Property-XElements</param>
        /// <returns>Generierter TypeScript-Code als String</returns>
        protected string GenerateType(string name, IEnumerable<XElement> properties)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"export type {name} = {{");

            foreach (var prop in properties)
            {
                var propName = prop.Attribute("Name")!.Value;
                var propType = prop.Attribute("Type")!.Value;

                // Nullable-Default laut OData-Spezifikation ist true
                var nullable = prop.Attribute("Nullable")?.Value ?? "true";
                var tsType = MapEdmTypeToTypeScript(propType);

                // Nullable=true  → string | null
                // Nullable=false → string
                var fullType = nullable != "false" ? $"{tsType} | null" : tsType;

                sb.AppendLine($"\t{propName}: {fullType};");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>
        /// Konvertiert einen EDM-Typ in den entsprechenden TypeScript-Typ.
        /// Unterstützt Collection-Typen und Namespace-Prefixe.
        /// </summary>
        /// <param name="edmType">EDM-Typ z.B. Edm.String, Collection(SAPB1.TeamMember)</param>
        /// <returns>TypeScript-Typ als String</returns>
        protected static string MapEdmTypeToTypeScript(string edmType)
        {
            // Collection(Namespace.TypeName) → TypeName[]
            if (edmType.StartsWith("Collection(") && edmType.EndsWith(")"))
            {
                // Inneren Typ extrahieren z.B. SAPB1.TeamMember
                var innerType = edmType[11..^1];

                // Rekursiv auflösen
                var mappedInner = MapEdmTypeToTypeScript(innerType);
                return $"{mappedInner}[]";
            }

            // Namespace-Prefix entfernen z.B. SAPB1.TeamMember → TeamMember
            var cleanType = edmType.Contains('.')
                ? edmType[(edmType.LastIndexOf('.') + 1)..]
                : edmType;

            // EDM-Typ auf TypeScript-Typ mappen
            return cleanType switch
            {
                "String" => "string",
                "Int16" => "number",
                "Int32" => "number",
                "Int64" => "number",
                "Decimal" => "number",
                "Double" => "number",
                "Single" => "number",
                "Boolean" => "boolean",
                "DateTime" => "Date",
                "DateTimeOffset" => "Date",
                "Guid" => "string",
                "Binary" => "Uint8Array",
                _ => cleanType // Unbekannte Typen unverändert lassen
            };
        }
    }
}
