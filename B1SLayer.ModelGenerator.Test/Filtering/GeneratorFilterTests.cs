using B1SLayer.ModelGenerator.Options;
using B1SLayer.ModelGenerator.Test.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using B1SLayer.ModelGenerator.Generators.CSharp;

namespace B1SLayer.ModelGenerator.Test.Filtering
{
    /// <summary>
    /// Tests für die Include/Exclude Filterlogik in GeneratorBase
    /// </summary>
    public class GeneratorFilterTests
    {
        private readonly DirectoryInfo _outputDir;
        private readonly CSharpEntityTypeGenerator _generator;

        public GeneratorFilterTests()
        {
            // Temporäres Verzeichnis für Testausgaben
            _outputDir = Directory.CreateTempSubdirectory("ModelGeneratorTest");
            _generator = new CSharpEntityTypeGenerator("Test.Ns", _outputDir);
        }

        private List<XElement> BuildTypes(params string[] names)
            => names.Select(n => XElementBuilder.EntityType(n)).ToList();

        private GeneratorOptions BuildOptions(string[] include, string[] exclude) => new()
        {
            MetadataFile = new FileInfo("dummy.xml"),
            Namespace = "Test.Ns",
            OutputDir = _outputDir,
            Include = include,
            Exclude = exclude,
            Language = TargetLanguage.CSharp
        };

        [Fact]
        public async Task GenerateAsync_NoFilter_ShouldGenerateAllTypes()
        {
            var types = BuildTypes("Document", "BusinessPartner", "Address");
            var options = BuildOptions([], []);

            await _generator.GenerateAsync(types, options);

            _outputDir.GetFiles("*.cs").Should().HaveCount(3);
        }

        [Fact]
        public async Task GenerateAsync_WithInclude_ShouldGenerateOnlyIncludedTypes()
        {
            var types = BuildTypes("Document", "BusinessPartner", "Address");
            var options = BuildOptions(["Document", "Address"], []);

            await _generator.GenerateAsync(types, options);

            var files = _outputDir.GetFiles("*.cs").Select(f => f.Name);
            files.Should().BeEquivalentTo("Document.cs", "Address.cs");
        }

        [Fact]
        public async Task GenerateAsync_WithExclude_ShouldSkipExcludedTypes()
        {
            var types = BuildTypes("Document", "BusinessPartner", "Address");
            var options = BuildOptions([], ["BusinessPartner"]);

            await _generator.GenerateAsync(types, options);

            var files = _outputDir.GetFiles("*.cs").Select(f => f.Name);
            files.Should().BeEquivalentTo("Document.cs", "Address.cs");
        }

        [Fact]
        public async Task GenerateAsync_IncludeAndExclude_ExcludeShouldWinOverInclude()
        {
            var types = BuildTypes("Document", "BusinessPartner", "Address");
            var options = BuildOptions(
                ["Document", "BusinessPartner", "Address"],
                ["Address"]);

            await _generator.GenerateAsync(types, options);

            var files = _outputDir.GetFiles("*.cs").Select(f => f.Name);
            files.Should().BeEquivalentTo("Document.cs", "BusinessPartner.cs");
        }

        [Fact]
        public async Task GenerateAsync_FilterIsCaseInsensitive()
        {
            var types = BuildTypes("Document", "BusinessPartner");
            var options = BuildOptions(["document", "BUSINESSPARTNER"], []);

            await _generator.GenerateAsync(types, options);

            _outputDir.GetFiles("*.cs").Should().HaveCount(2);
        }

        [Fact]
        public async Task GenerateAsync_EmptyTypeList_ShouldGenerateNoFiles()
        {
            var options = BuildOptions([], []);

            await _generator.GenerateAsync([], options);

            _outputDir.GetFiles("*.cs").Should().BeEmpty();
        }

    }
}
