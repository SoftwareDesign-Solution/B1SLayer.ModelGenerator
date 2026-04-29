using B1SLayer.ModelGenerator.Exceptions;
using B1SLayer.ModelGenerator.Generators;
using B1SLayer.ModelGenerator.Generators.CSharp;
using B1SLayer.ModelGenerator.Generators.TypeScript;
using B1SLayer.ModelGenerator.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace B1SLayer.ModelGenerator.Services
{
    /// <summary>
    /// Orchestriert die Codegenerierung — lädt das Schema und ruft die Generatoren auf
    /// </summary>
    public class GeneratorService
    {

        // OData V4 Namespace
        private static readonly XNamespace EdmNsV4 = "http://docs.oasis-open.org/odata/ns/edm";

        // OData V3 Namespace
        private static readonly XNamespace EdmNsV3 = "http://schemas.microsoft.com/ado/2009/11/edm";

        // <summary>
        /// Startet die Codegenerierung mit den angegebenen Optionen
        /// </summary>
        /// <param name="options">Konfigurationsoptionen</param>
        /// <param name="ct">Abbruch-Token</param>
        public async Task RunAsync(GeneratorOptions options, CancellationToken cancellationToken = default)
        {

            // Optionen validieren und Ausgabeverzeichnis erstellen
            ValidateOptions(options);

            // EDMX-Schema laden
            XElement schemaElement = LoadSchema(options.MetadataFile);

            var enumTypes = schemaElement.Elements(EdmNsV4 + "EnumType").ToList();
            var complexTypes = schemaElement.Elements(EdmNsV4 + "ComplexType").ToList();
            var entityTypes = schemaElement.Elements(EdmNsV4 + "EntityType").ToList();

            // Generatoren für die gewählte Zielsprache erstellen
            var generators = GeneratorFactory.Create(options);

            // Jeden Typ mit dem entsprechenden Generator verarbeiten
            await generators[GeneratorKind.EnumType].GenerateAsync(enumTypes, options, cancellationToken);
            await generators[GeneratorKind.EntityType].GenerateAsync(entityTypes, options, cancellationToken);
            await generators[GeneratorKind.ComplexType].GenerateAsync(complexTypes, options, cancellationToken);

        }

        /// <summary>
        /// Validiert die Optionen und erstellt das Ausgabeverzeichnis falls nötig
        /// </summary>
        /// <param name="options">Konfigurationsoptionen</param>
        private void ValidateOptions(GeneratorOptions options)
        {

            if (!options.MetadataFile.Exists)
                throw new GeneratorException($"Datei nicht gefunden: {options.MetadataFile.FullName}");

            options.OutputDir.Create();

        }

        /// <summary>
        /// Lädt und parst die EDMX-Datei und gibt das Schema-Element zurück
        /// </summary>
        /// <param name="file">EDMX-Datei</param>
        /// <param name="ct">Abbruch-Token</param>
        /// <returns>Schema XElement</returns>
        private XElement LoadSchema(FileInfo metadataFile)
        {

            var document = XDocument.Load(metadataFile.FullName);

            // Schema-Element für V3 und V4 suchen
            return document
                       .Descendants()
                       .FirstOrDefault(e => e.Name.LocalName == "Schema" &&
                                            (e.Name.Namespace == EdmNsV3 ||
                                             e.Name.Namespace == EdmNsV4))
                   ?? throw new GeneratorException("Kein gültiges Schema in der Metadatendatei gefunden.");

        }

    }
}
