using System;
using System.Collections.Generic;
using System.Text;
using B1SLayer.ModelGenerator.Generators.Base;
using B1SLayer.ModelGenerator.Generators.CSharp;
using B1SLayer.ModelGenerator.Generators.TypeScript;
using B1SLayer.ModelGenerator.Options;

namespace B1SLayer.ModelGenerator.Generators
{
    /// <summary>
    /// Zentrale Factory für die Erstellung der Generator-Dictionaries.
    /// Verwaltet die registrierten <see cref="IGeneratorProfile"/>-Implementierungen
    /// und wählt anhand der gewählten Zielsprache das passende Profil aus.
    /// </summary>
    /// <remarks>
    /// Neue Zielsprachen können über <see cref="RegisterProfile"/> registriert werden
    /// ohne die Factory selbst zu verändern — Open/Closed Prinzip.
    /// </remarks>
    public static class GeneratorFactory
    {

        /// <summary>
        /// Interne Liste der registrierten Profile — initial mit den Standard-Profilen befüllt
        /// </summary>
        private static readonly List<IGeneratorProfile> Profiles =
        [
            new CSharpGeneratorProfile(),
            new TypeScriptGeneratorProfile(),
        ];

        /// <summary>
        /// Erstellt das Generator-Dictionary für die in den Optionen gewählte Zielsprache.
        /// </summary>
        /// <param name="options">Konfigurationsoptionen mit der gewählten Zielsprache</param>
        /// <returns>Dictionary mit einem Generator pro <see cref="GeneratorKind"/></returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="options"/> null ist</exception>
        /// <exception cref="ArgumentOutOfRangeException">Wenn kein Profil für die gewählte Sprache gefunden wurde</exception>
        public static IReadOnlyDictionary<GeneratorKind, GeneratorBase> Create(
            GeneratorOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var profile = Profiles.FirstOrDefault(p =>
                p.Language == options.Language);

            if (profile is null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(options.Language),
                    options.Language,
                    "Für die gewählte Zielsprache wurde kein Generator-Profil gefunden.");
            }

            return profile.CreateGenerators(options);
        }
    }
}
