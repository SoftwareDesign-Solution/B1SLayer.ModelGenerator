using System;
using System.Collections.Generic;
using System.Text;

namespace B1SLayer.ModelGenerator.Options
{

    /// <summary>
    /// Enthält alle Konfigurationsoptionen für die Codegenerierung
    /// </summary>
    public class GeneratorOptions
    {
        /// <summary>Pfad zur EDMX-Metadatendatei</summary>
        public FileInfo MetadataFile { get; init; } = default!;

        /// <summary>Ziel-Namespace für die generierten Klassen</summary>
        public string Namespace { get; init; } = default!;

        /// <summary>Ausgabeverzeichnis für die generierten Dateien</summary>
        public DirectoryInfo OutputDir { get; init; } = default!;

        /// <summary>Liste der Objekte die nicht generiert werden sollen</summary>
        public string[] Exclude { get; init; } = [];

        /// <summary>Liste der Objekte die ausschließlich generiert werden sollen</summary>
        public string[] Include { get; init; } = [];

        /// <summary>Zielsprache für die Codegenerierung</summary>
        public TargetLanguage Language { get; init; }
    }
}
