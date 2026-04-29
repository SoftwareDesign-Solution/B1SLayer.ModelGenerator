using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using B1SLayer.ModelGenerator.Generators.Base;

namespace B1SLayer.ModelGenerator.Generators.TypeScript
{
    internal abstract class TypeScriptGeneratorBase : GeneratorBase
    {
        /// <summary>
        /// Initialisiert die TypeScript-Basisklasse mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        protected TypeScriptGeneratorBase(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Gibt den Dateipfad mit .ts Erweiterung zurück
        /// </summary>
        /// <param name="type">XElement des Typs</param>
        /// <returns>Vollständiger Dateipfad mit .ts Erweiterung</returns>
        protected override string GetFileName(XElement type)
            => Path.Combine(OutputDir.FullName, $"{type.Attribute("Name")!.Value}.ts");
    }
}
