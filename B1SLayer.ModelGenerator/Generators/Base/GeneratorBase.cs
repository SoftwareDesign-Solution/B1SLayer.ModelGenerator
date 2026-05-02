using B1SLayer.ModelGenerator.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Generators.Base
{
    /// <summary>
    /// Abstrakte Basisklasse für alle Generatoren.
    /// Enthält die gemeinsame Logik für Filterung und Datei-Ausgabe.
    /// </summary>
    public abstract class GeneratorBase
    {
        // OData Namespace für den Zugriff auf Child-Elemente
        protected static readonly XNamespace EdmNs = "http://docs.oasis-open.org/odata/ns/edm";

        /// <summary>Ziel-Namespace für die generierten Klassen</summary>
        protected readonly string Namespace;

        /// <summary>Ausgabeverzeichnis für die generierten Dateien</summary>
        protected readonly DirectoryInfo OutputDir;

        /// <summary>
        /// Initialisiert die Basisklasse mit Namespace und Ausgabeverzeichnis
        /// </summary>
        /// <param name="ns">Ziel-Namespace</param>
        /// <param name="outputDir">Ausgabeverzeichnis</param>
        protected GeneratorBase(string ns, DirectoryInfo outputDir)
        {
            Namespace = ns;
            OutputDir = outputDir;
        }

        /// <summary>
        /// Filtert die Typen anhand von Include/Exclude und generiert je eine Datei pro Typ
        /// </summary>
        /// <param name="types">Liste der zu verarbeitenden XElements</param>
        /// <param name="options">Konfigurationsoptionen mit Include/Exclude</param>
        /// <param name="ct">Abbruch-Token</param>
        public async Task GenerateAsync(List<XElement> types, GeneratorOptions options, CancellationToken ct = default)
        {
            // Include/Exclude Filter anwenden
            var filtered = ApplyFilter(types, options.Include, options.Exclude);

            Console.WriteLine($"[{GetType().Name}] {filtered.Count} von {types.Count} Typen werden generiert.");

            // Pro Typ eine Datei generieren
            foreach (var type in filtered)
            {
                var code = Generate(type);
                var fileName = GetFileName(type);

                await File.WriteAllTextAsync(fileName, code, ct);

                Console.WriteLine($"[{GetType().Name}] Erstellt -> {Path.GetFileName(fileName)}");
            }
        }

        /// <summary>
        /// Generiert den Code für einen einzelnen Typ
        /// </summary>
        /// <param name="type">XElement des Typs</param>
        /// <returns>Generierter Code als String</returns>
        internal abstract string Generate(XElement type);

        /// <summary>
        /// Gibt den vollständigen Dateipfad für die generierte Datei zurück
        /// </summary>
        /// <param name="type">XElement des Typs</param>
        /// <returns>Vollständiger Dateipfad</returns>
        protected abstract string GetFileName(XElement type);

        /// <summary>
        /// Wendet Include- und Exclude-Filter auf die Typenliste an.
        /// Include hat Vorrang — nur explizit genannte Objekte werden berücksichtigt.
        /// Exclude wird danach angewendet.
        /// </summary>
        /// <param name="types">Ungefilterte Typenliste</param>
        /// <param name="include">Whitelist der zu generierenden Typen</param>
        /// <param name="exclude">Blacklist der nicht zu generierenden Typen</param>
        /// <returns>Gefilterte Typenliste</returns>
        private static List<XElement> ApplyFilter(List<XElement> types, string[] include, string[] exclude)
        {
            var result = types.AsEnumerable();

            if (include.Length > 0)
                result = result.Where(t => include.Contains(
                    t.Attribute("Name")?.Value, StringComparer.OrdinalIgnoreCase));

            if (exclude.Length > 0)
                result = result.Where(t => !exclude.Contains(
                    t.Attribute("Name")?.Value, StringComparer.OrdinalIgnoreCase));

            return result.ToList();
        }

    }
}
