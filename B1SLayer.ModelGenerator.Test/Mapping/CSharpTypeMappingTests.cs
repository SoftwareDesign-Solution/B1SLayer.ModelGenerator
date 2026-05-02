using B1SLayer.ModelGenerator.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using B1SLayer.ModelGenerator.Generators.CSharp;
using FluentAssertions;

namespace B1SLayer.ModelGenerator.Test.Mapping
{
    /// <summary>
    /// Tests für die EDM → C# Typ-Konvertierung
    /// </summary>
    public class CSharpTypeMappingTests
    {
        private readonly DirectoryInfo _outputDir = new(Path.GetTempPath());

        // Hilfsmethod um MapEdmTypeToCSharp über den Generator aufzurufen
        private string Map(string edmType, string nullable)
        {
            var element = XElementBuilder.EntityType("Test",
                ("Prop", edmType, nullable));

            var generator = new CSharpEntityTypeGenerator("Test.Ns", _outputDir);
            var code = generator.Generate(element);

            // Property-Zeile extrahieren: "public TYPE Prop { get; set; }"
            var line = code.Split('\n')
                .First(l => l.Contains("Prop"));

            // Typ extrahieren
            return line.Trim()
                .Replace("public ", "")
                .Replace(" Prop { get; set; }", "")
                .Trim();
        }

        [Theory]
        [InlineData("Edm.String", "false", "string")]
        [InlineData("Edm.String", "true", "string")]   // Referenztyp → kein ?
        [InlineData("Edm.Int32", "false", "int")]
        [InlineData("Edm.Int32", "true", "int?")]
        [InlineData("Edm.Int16", "true", "short?")]
        [InlineData("Edm.Int64", "true", "long?")]
        [InlineData("Edm.Decimal", "true", "decimal?")]
        [InlineData("Edm.Double", "true", "double?")]
        [InlineData("Edm.Single", "true", "float?")]
        [InlineData("Edm.Boolean", "false", "bool")]
        [InlineData("Edm.Boolean", "true", "bool?")]
        [InlineData("Edm.DateTime", "false", "DateTime")]
        [InlineData("Edm.DateTime", "true", "DateTime?")]
        [InlineData("Edm.DateTimeOffset", "true", "DateTimeOffset?")]
        [InlineData("Edm.Guid", "false", "Guid")]
        [InlineData("Edm.Guid", "true", "Guid?")]
        [InlineData("Edm.Binary", "false", "byte[]")]
        public void MapEdmTypeToCSharp_ShouldReturnCorrectType(
            string edmType, string nullable, string expected)
        {
            Map(edmType, nullable).Should().Be(expected);
        }

        [Fact]
        public void MapEdmTypeToCSharp_Collection_ShouldReturnListOfInnerType()
        {
            Map("Collection(SAPB1.DocumentLine)", "false")
                .Should().Be("List<DocumentLine>");
        }

        [Fact]
        public void MapEdmTypeToCSharp_CollectionOfEdmType_ShouldReturnListOfCSharpType()
        {
            Map("Collection(Edm.String)", "false")
                .Should().Be("List<string>");
        }

        [Fact]
        public void MapEdmTypeToCSharp_NamespacedType_ShouldStripNamespace()
        {
            Map("SAPB1.TeamMember", "false")
                .Should().Be("TeamMember");
        }

        [Fact]
        public void MapEdmTypeToCSharp_NullableNamespacedType_ShouldAddQuestionMark()
        {
            // Nur wenn es ein Wertetyp ist — bei komplexen Typen kein ?
            Map("SAPB1.TeamMember", "true")
                .Should().Be("TeamMember?");
        }

        [Fact]
        public void MapEdmTypeToCSharp_DefaultNullable_ShouldTreatAsNullable()
        {
            // Kein Nullable-Attribut → Default ist true laut OData-Spezifikation
            var element = XElementBuilder.EntityType("Test",
                ("Prop", "Edm.Int32", null)); // null = Attribut nicht gesetzt

            var generator = new CSharpEntityTypeGenerator("Test.Ns", _outputDir);
            var code = generator.Generate(element);

            code.Should().Contain("int?");
        }
    }
}
