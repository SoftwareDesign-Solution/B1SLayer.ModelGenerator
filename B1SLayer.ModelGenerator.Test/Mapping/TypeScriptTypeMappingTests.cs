using B1SLayer.ModelGenerator.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using B1SLayer.ModelGenerator.Generators.TypeScript;
using FluentAssertions;

namespace B1SLayer.ModelGenerator.Test.Mapping
{
    /// <summary>
    /// Tests für die EDM → TypeScript Typ-Konvertierung
    /// </summary>
    public class TypeScriptTypeMappingTests
    {
        private readonly DirectoryInfo _outputDir = new(Path.GetTempPath());

        private string Map(string edmType, string nullable)
        {
            var element = XElementBuilder.EntityType("Test",
                ("Prop", edmType, nullable));

            var generator = new TypeScriptEntityTypeGenerator("Test.Ns", _outputDir);
            var code = generator.Generate(element);

            // Property-Zeile extrahieren: "Prop: type;"
            return code.Split('\n')
                .First(l => l.Contains("Prop"))
                .Trim()
                .TrimEnd(';');
        }

        [Theory]
        [InlineData("Edm.String", "false", "Prop: string")]
        [InlineData("Edm.String", "true", "Prop: string | null")]
        [InlineData("Edm.Int32", "false", "Prop: number")]
        [InlineData("Edm.Int32", "true", "Prop: number | null")]
        [InlineData("Edm.Int16", "true", "Prop: number | null")]
        [InlineData("Edm.Int64", "true", "Prop: number | null")]
        [InlineData("Edm.Decimal", "true", "Prop: number | null")]
        [InlineData("Edm.Boolean", "false", "Prop: boolean")]
        [InlineData("Edm.Boolean", "true", "Prop: boolean | null")]
        [InlineData("Edm.DateTime", "false", "Prop: Date")]
        [InlineData("Edm.DateTime", "true", "Prop: Date | null")]
        [InlineData("Edm.DateTimeOffset", "true", "Prop: Date | null")]
        [InlineData("Edm.Guid", "false", "Prop: string")]
        [InlineData("Edm.Guid", "true", "Prop: string | null")]
        [InlineData("Edm.Binary", "false", "Prop: Uint8Array")]
        public void MapEdmTypeToTypeScript_ShouldReturnCorrectType(
            string edmType, string nullable, string expected)
        {
            Map(edmType, nullable).Should().Be(expected);
        }

        [Fact]
        public void MapEdmTypeToTypeScript_Collection_ShouldReturnArrayType()
        {
            Map("Collection(SAPB1.DocumentLine)", "false")
                .Should().Be("Prop: DocumentLine[]");
        }

        [Fact]
        public void MapEdmTypeToTypeScript_NullableCollection_ShouldReturnNullableArrayType()
        {
            Map("Collection(SAPB1.DocumentLine)", "true")
                .Should().Be("Prop: DocumentLine[] | null");
        }

        [Fact]
        public void MapEdmTypeToTypeScript_NamespacedType_ShouldStripNamespace()
        {
            Map("SAPB1.TeamMember", "false")
                .Should().Be("Prop: TeamMember");
        }

        [Fact]
        public void MapEdmTypeToTypeScript_DefaultNullable_ShouldTreatAsNullable()
        {
            // Kein Nullable-Attribut → Default ist true laut OData-Spezifikation
            var element = XElementBuilder.EntityType("Test",
                ("Prop", "Edm.Int32", null));

            var generator = new TypeScriptEntityTypeGenerator("Test.Ns", _outputDir);
            var code = generator.Generate(element);

            code.Should().Contain("number | null");
        }

    }
}
