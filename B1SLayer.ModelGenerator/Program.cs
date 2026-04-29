using B1SLayer.ModelGenerator.Exceptions;
using B1SLayer.ModelGenerator.Options;
using B1SLayer.ModelGenerator.Services;
using System.CommandLine;


// Pflichtparameter: Pfad zur EDMX-Metadatendatei
var metadataFileOption = new Option<FileInfo>("--metadatafile")
{
    Description = "Pfad zur Metadatendatei",
    Required = true
};

// Pflichtparameter: Ziel-Namespace für die generierten Klassen
var namespaceOption = new Option<string>("--namespace")
{
    Description = "Ziel-Namespace",
    Required = true
};

// Pflichtparameter: Ausgabeverzeichnis für die generierten Dateien
var outputDirOption = new Option<DirectoryInfo>("--outputdir")
{
    Description = "Ausgabeverzeichnis", 
    Required = true
};

// Pflichtparameter: Zielsprache für die Codegenerierung
// Erlaubte Werte: CSharp, cs, TypeScript, ts
var languageOption = new Option<TargetLanguage>("--language")
{
    Required = true,
    Description = "Zielsprache: CSharp (cs) oder TypeScript (ts)",
    CustomParser = result =>
    {
        var value = result.Tokens.FirstOrDefault()?.Value ?? string.Empty;
        return value.ToLowerInvariant() switch
        {
            "csharp" or "cs" => TargetLanguage.CSharp,
            "typescript" or "ts" => TargetLanguage.TypeScript,
            _ => throw new ArgumentException($"Ungültige Sprache '{value}'. Erlaubt: CSharp, cs, TypeScript, ts")
        };
    }
};

// Optionaler Parameter: Objekte die nicht generiert werden sollen
// Unterstützt komma- und leerzeichengetrennte Werte
var excludeOption = new Option<string[]>("--exclude")
{
    Description = "Objekte die nicht generiert werden sollen",
    AllowMultipleArgumentsPerToken = true,
    CustomParser = result =>
    {
        var tokens = result.Tokens;
        var values = tokens.SelectMany(t => t.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        return values;
    }
};

// Optionaler Parameter: Nur diese Objekte generieren
// Unterstützt komma- und leerzeichengetrennte Werte
var includeOption = new Option<string[]>("--include")
{
    Description = "Nur diese Objekte generieren",
    AllowMultipleArgumentsPerToken = true,
    CustomParser = result =>
    {
        var tokens = result.Tokens;
        var values = tokens.SelectMany(t => t.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToArray();
        return values;
    }
};

var rootCommand = new RootCommand("Code Generator");
rootCommand.Options.Add(metadataFileOption);
rootCommand.Options.Add(namespaceOption);
rootCommand.Options.Add(outputDirOption);
rootCommand.Options.Add(languageOption);
rootCommand.Options.Add(excludeOption);
rootCommand.Options.Add(includeOption);

rootCommand.SetAction(async (parseResult) =>
{

    // Optionen aus dem ParseResult in ein GeneratorOptions-Objekt übertragen
    var options = new GeneratorOptions
    {
        MetadataFile = parseResult.GetValue(metadataFileOption)!,
        Namespace = parseResult.GetValue(namespaceOption)!,
        OutputDir = parseResult.GetValue(outputDirOption)!,
        Language = parseResult.GetValue(languageOption),
        Exclude = parseResult.GetValue(excludeOption) ?? Array.Empty<string>(),
        Include = parseResult.GetValue(includeOption) ?? Array.Empty<string>()
    };

    var generatorService = new GeneratorService();

    try
    {
        await generatorService.RunAsync(options);
    }
    catch (GeneratorException ex)
    {
        // Bekannte Fehler aus der Geschäftslogik
        Console.Error.WriteLine($"Fehler: {ex.Message}");
        Environment.Exit(1);
    }
    catch (Exception ex)
    {
        // Unerwartete Fehler
        Console.Error.WriteLine($"Unerwarteter Fehler: {ex.Message}");
        Environment.Exit(2);
    }

});

// Keine Argumente → Help anzeigen
if (args.Length == 0)
{
    rootCommand.Parse("--help").Invoke();
    return 0;
}

return await rootCommand.Parse(args).InvokeAsync();
