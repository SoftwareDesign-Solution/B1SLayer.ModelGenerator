using B1SLayer.ModelGenerator.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using B1SLayer.ModelGenerator.Generators.CSharp;
using FluentAssertions;

namespace B1SLayer.ModelGenerator.Test.Generators.CSharp
{
    /// <summary>
    /// Tests für den CSharpEnumTypeGenerator
    /// </summary>
    public class CSharpEnumTypeGeneratorTests
    {
        private readonly DirectoryInfo _outputDir = new(Path.GetTempPath());
        private readonly CSharpEnumTypeGenerator _generator;

        public CSharpEnumTypeGeneratorTests()
        {
            _generator = new CSharpEnumTypeGenerator("Example.Dto", _outputDir);
        }

        [Fact]
        public void Generate_ShouldContainCorrectNamespace()
        {
            var element = XElementBuilder.EnumType("BoStatus");
            var code = _generator.Generate(element);

            code.Should().Contain("namespace Example.Dto");
        }

        [Fact]
        public void Generate_ShouldContainEnumName()
        {
            var element = XElementBuilder.EnumType("BoStatus");
            var code = _generator.Generate(element);

            code.Should().Contain("public enum BoStatus");
        }

        [Fact]
        public void Generate_WithMembersAndValues_ShouldContainAllMembers()
        {
            var element = XElementBuilder.EnumType("BoStatus",
                ("Open", "1"),
                ("Closed", "2"),
                ("Cancel", "3"));

            var code = _generator.Generate(element);

            code.Should().Contain("Open = 1,");
            code.Should().Contain("Closed = 2,");
            code.Should().Contain("Cancel = 3,");
        }

        [Fact]
        public void Generate_WithMembersWithoutValues_ShouldContainMembersWithoutAssignment()
        {
            var element = XElementBuilder.EnumType("BoStatus",
                ("Open", null),
                ("Closed", null));

            var code = _generator.Generate(element);

            code.Should().Contain("Open,");
            code.Should().Contain("Closed,");
            code.Should().NotContain("=");
        }

        [Fact]
        public void Generate_ShouldProduceValidEnumSyntax()
        {
            var element = XElementBuilder.EnumType("BoYesNoEnum",
                ("tYES", "1"),
                ("tNO", "0"));

            var code = _generator.Generate(element);

            // Grundlegende Struktur prüfen
            code.Should().Contain("{");
            code.Should().Contain("}");
            code.Should().MatchRegex(@"public enum BoYesNoEnum\s*\{");
        }

    }
}
