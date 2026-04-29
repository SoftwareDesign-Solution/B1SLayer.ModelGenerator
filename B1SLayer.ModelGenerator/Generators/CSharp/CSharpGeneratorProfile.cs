using B1SLayer.ModelGenerator.Generators.Base;
using B1SLayer.ModelGenerator.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace B1SLayer.ModelGenerator.Generators.CSharp
{
    /// <summary>
    /// Generator-Profil für die Zielsprache C#.
    /// Registriert alle verfügbaren C#-Generatoren und ordnet sie
    /// dem entsprechenden <see cref="GeneratorKind"/> zu.
    /// </summary>
    /// <remarks>
    /// Um einen neuen C#-Generator hinzuzufügen, muss lediglich ein
    /// neuer Eintrag im Dictionary ergänzt werden — alle anderen
    /// Klassen bleiben unverändert.
    /// </remarks>
    public sealed class CSharpGeneratorProfile : IGeneratorProfile
    {
        /// <inheritdoc/>
        public TargetLanguage Language => TargetLanguage.CSharp;

        /// <summary>
        /// Erstellt das Generator-Dictionary für C#.
        /// </summary>
        /// <param name="options">Konfigurationsoptionen mit Namespace und Ausgabeverzeichnis</param>
        /// <returns>Dictionary mit C#-Generatoren pro <see cref="GeneratorKind"/></returns>
        public IReadOnlyDictionary<GeneratorKind, GeneratorBase> CreateGenerators(GeneratorOptions options)
        {
            var ns = options.Namespace;
            var dir = options.OutputDir;

            return new Dictionary<GeneratorKind, GeneratorBase>
            {
                [GeneratorKind.EnumType] = new CSharpEnumTypeGenerator(ns, dir),
                [GeneratorKind.ComplexType] = new CSharpComplexTypeGenerator(ns, dir),
                [GeneratorKind.EntityType] = new CSharpEntityTypeGenerator(ns, dir),
            };
        }
    }
}
