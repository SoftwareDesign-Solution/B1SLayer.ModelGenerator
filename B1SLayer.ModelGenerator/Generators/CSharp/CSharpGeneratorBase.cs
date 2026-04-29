using B1SLayer.ModelGenerator.Generators.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.CSharp
{
    /// <summary>
    /// Abstrakte Basisklasse für alle C#-Generatoren.
    /// Definiert den Dateinamen mit .cs Erweiterung.
    /// </summary>
    public abstract class CSharpGeneratorBase : GeneratorBase
    {
        /// <summary>
        /// Initialisiert die C#-Basisklasse mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        protected CSharpGeneratorBase(string ns, DirectoryInfo outputDir)
            : base(ns, outputDir) { }

        /// <summary>
        /// Gibt den Dateipfad mit .cs Erweiterung zurück
        /// </summary>
        /// <param name="type">XElement des Typs</param>
        /// <returns>Vollständiger Dateipfad mit .cs Erweiterung</returns>
        protected override string GetFileName(XElement type)
            => Path.Combine(OutputDir.FullName, $"{type.Attribute("Name")!.Value}.cs");
    }
}
