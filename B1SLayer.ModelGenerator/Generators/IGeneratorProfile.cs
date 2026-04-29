using B1SLayer.ModelGenerator.Generators.Base;
using B1SLayer.ModelGenerator.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace B1SLayer.ModelGenerator.Generators
{
    /// <summary>
    /// Definiert den Vertrag für ein Generator-Profil.
    /// Jede Zielsprache implementiert dieses Interface und ist damit
    /// vollständig eigenständig — die Factory muss nicht geändert werden
    /// wenn eine neue Sprache hinzukommt.
    /// </summary>
    public interface IGeneratorProfile
    {
        /// <summary>
        /// Die Zielsprache für die dieses Profil zuständig ist
        /// </summary>
        TargetLanguage Language { get; }

        /// <summary>
        /// Erstellt das Generator-Dictionary für die jeweilige Zielsprache.
        /// Der <see cref="GeneratorKind"/> dient als typsicherer Schlüssel
        /// anstelle von fehleranfälligen Index-Zugriffen.
        /// </summary>
        /// <param name="options">Konfigurationsoptionen mit Namespace und Ausgabeverzeichnis</param>
        /// <returns>Dictionary mit einem Generator pro unterstütztem <see cref="GeneratorKind"/></returns>
        IReadOnlyDictionary<GeneratorKind, GeneratorBase> CreateGenerators(GeneratorOptions options);
    }
}
