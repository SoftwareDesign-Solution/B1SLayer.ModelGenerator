using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.CSharp
{
    /// <summary>
    /// Generiert C# Klassen-Dateien aus EDMX ComplexType-Elementen
    /// </summary>
    internal class CSharpComplexTypeGenerator : CSharpClassGeneratorBase
    {
        /// <summary>
        /// Initialisiert den Generator mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        public CSharpComplexTypeGenerator(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Generiert eine C# Klasse aus einem EDMX ComplexType-Element
        /// </summary>
        /// <param name="type">ComplexType XElement</param>
        /// <returns>Generierter C# Klassen-Code</returns>
        protected override string Generate(XElement type)
            => GenerateClass(
                type.Attribute("Name")!.Value,
                type.Elements(EdmNs + "Property"));
    }
}
