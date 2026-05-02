using B1SLayer.ModelGenerator.Generators.CSharp;
using B1SLayer.ModelGenerator.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;

namespace B1SLayer.ModelGenerator.Test.Generators.CSharp
{
    /// <summary>
    /// Tests für den CSharpEntityTypeGenerator
    /// </summary>
    public class CSharpEntityTypeGeneratorTests
    {
        private readonly DirectoryInfo _outputDir = new(Path.GetTempPath());
        private readonly CSharpEntityTypeGenerator _generator;

        public CSharpEntityTypeGeneratorTests()
        {
            _generator = new CSharpEntityTypeGenerator("Example.Dto", _outputDir);
        }

        [Fact]
        public void Generate_ShouldContainCorrectNamespace()
        {
            var element = XElementBuilder.EntityType("Document");
            var code = _generator.Generate(element);

            code.Should().Contain("namespace Example.Dto");
        }

        [Fact]
        public void Generate_ShouldContainClassName()
        {
            var element = XElementBuilder.EntityType("Document");
            var code = _generator.Generate(element);

            code.Should().Contain("public class Document");
        }

        [Fact]
        public void Generate_WithProperties_ShouldContainAllProperties()
        {
            var element = XElementBuilder.EntityType("Document",
                ("DocEntry", "Edm.Int32", "false"),
                ("CardCode", "Edm.String", "false"),
                ("DocDate", "Edm.DateTime", "false"));

            var code = _generator.Generate(element);

            code.Should().Contain("public int DocEntry { get; set; }");
            code.Should().Contain("public string CardCode { get; set; }");
            code.Should().Contain("public DateTime DocDate { get; set; }");
        }

        [Fact]
        public void Generate_WithNullableProperties_ShouldAddQuestionMark()
        {
            var element = XElementBuilder.EntityType("Document",
                ("DocNum", "Edm.Int32", "true"),
                ("DocDate", "Edm.DateTime", "true"));

            var code = _generator.Generate(element);

            code.Should().Contain("public int? DocNum { get; set; }");
            code.Should().Contain("public DateTime? DocDate { get; set; }");
        }

        [Fact]
        public void Generate_WithCollectionProperty_ShouldReturnListType()
        {
            var element = XElementBuilder.EntityType("Document",
                ("DocumentLines", "Collection(SAPB1.DocumentLine)", "false"));

            var code = _generator.Generate(element);

            code.Should().Contain("public List<DocumentLine> DocumentLines { get; set; }");
        }

        [Fact]
        public void Generate_WithNamespacedType_ShouldStripNamespace()
        {
            var element = XElementBuilder.EntityType("Team",
                ("TeamMember", "SAPB1.TeamMember", "false"));

            var code = _generator.Generate(element);

            code.Should().Contain("public TeamMember TeamMember { get; set; }");
            code.Should().NotContain("SAPB1.");
        }
    }
}
