using B1SLayer.ModelGenerator.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using B1SLayer.ModelGenerator.Generators.CSharp;
using FluentAssertions;

namespace B1SLayer.ModelGenerator.Test.Generators.CSharp
{
    /// <summary>
    /// Tests für den CSharpComplexTypeGenerator
    /// </summary>
    public class CSharpComplexTypeGeneratorTests
    {
        private readonly DirectoryInfo _outputDir = new(Path.GetTempPath());
        private readonly CSharpComplexTypeGenerator _generator;

        public CSharpComplexTypeGeneratorTests()
        {
            _generator = new CSharpComplexTypeGenerator("Example.Dto", _outputDir);
        }

        [Fact]
        public void Generate_ShouldContainClassName()
        {
            var element = XElementBuilder.ComplexType("BPAddress");
            var code = _generator.Generate(element);

            code.Should().Contain("public class BPAddress");
        }

        [Fact]
        public void Generate_WithProperties_ShouldContainAllProperties()
        {
            var element = XElementBuilder.ComplexType("BPAddress",
                ("Street", "Edm.String", "true"),
                ("City", "Edm.String", "true"),
                ("ZipCode", "Edm.String", "false"));

            var code = _generator.Generate(element);

            code.Should().Contain("public string Street { get; set; }");
            code.Should().Contain("public string City { get; set; }");
            code.Should().Contain("public string ZipCode { get; set; }");
        }
    }
}
