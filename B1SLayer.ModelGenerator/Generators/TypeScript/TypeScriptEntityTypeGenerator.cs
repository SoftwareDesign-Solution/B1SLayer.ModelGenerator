using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.TypeScript
{
    /// <summary>
    /// Generiert TypeScript Type-Dateien aus EDMX EntityType-Elementen
    /// </summary>
    internal class TypeScriptEntityTypeGenerator : TypeScriptClassGeneratorBase
    {
        /// <summary>
        /// Initialisiert den Generator mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        public TypeScriptEntityTypeGenerator(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Generiert einen TypeScript Type aus einem EDMX EntityType-Element
        /// </summary>
        /// <param name="type">EntityType XElement</param>
        /// <returns>Generierter TypeScript Type-Code</returns>
        protected override string Generate(XElement type)
            => GenerateType(
                type.Attribute("Name")!.Value,
                type.Elements(EdmNs + "Property"));
    }
}
