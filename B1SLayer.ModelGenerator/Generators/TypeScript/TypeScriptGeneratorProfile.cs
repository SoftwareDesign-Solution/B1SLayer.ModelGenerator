using B1SLayer.ModelGenerator.Generators.Base;
using B1SLayer.ModelGenerator.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace B1SLayer.ModelGenerator.Generators.TypeScript
{
    /// <summary>
    /// Generator-Profil für die Zielsprache TypeScript.
    /// Registriert alle verfügbaren TypeScript-Generatoren und ordnet sie
    /// dem entsprechenden <see cref="GeneratorKind"/> zu.
    /// </summary>
    /// <remarks>
    /// Um einen neuen TypeScript-Generator hinzuzufügen, muss lediglich ein
    /// neuer Eintrag im Dictionary ergänzt werden — alle anderen
    /// Klassen bleiben unverändert.
    /// </remarks>
    public sealed class TypeScriptGeneratorProfile : IGeneratorProfile
    {
        /// <inheritdoc/>
        public TargetLanguage Language => TargetLanguage.TypeScript;

        /// <summary>
        /// Erstellt das Generator-Dictionary für TypeScript.
        /// </summary>
        /// <param name="options">Konfigurationsoptionen mit Namespace und Ausgabeverzeichnis</param>
        /// <returns>Dictionary mit TypeScript-Generatoren pro <see cref="GeneratorKind"/></returns>
        public IReadOnlyDictionary<GeneratorKind, GeneratorBase> CreateGenerators(GeneratorOptions options)
        {
            var ns = options.Namespace;
            var dir = options.OutputDir;

            return new Dictionary<GeneratorKind, GeneratorBase>
            {
                [GeneratorKind.EnumType] = new TypeScriptEnumTypeGenerator(ns, dir),
                [GeneratorKind.ComplexType] = new TypeScriptComplexTypeGenerator(ns, dir),
                [GeneratorKind.EntityType] = new TypeScriptEntityTypeGenerator(ns, dir),
            };
        }
    }
}
