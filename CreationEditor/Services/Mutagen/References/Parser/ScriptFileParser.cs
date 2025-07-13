using System.IO.Abstractions;
using System.Text.RegularExpressions;
using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed partial class ScriptFileParser(IAssetTypeService assetTypeService)
    : IFileParser<IAssetLinkGetter> {

    [GeneratedRegex(@"ScriptName\s+([a-zA-Z_]\w*)(?:\s+extends\s+([a-zA-Z_]\w*))?", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptNameRegex { get; }

    public string Name => "Scripts";
    public IEnumerable<string> FileExtensions => assetTypeService.Provider.ScriptSource.FileExtensions;

    public IEnumerable<IAssetLinkGetter> ParseFile(string filePath, IFileSystem fileSystem) {
        if (assetTypeService.GetAssetType(filePath) != assetTypeService.Provider.ScriptSource) return [];

        var results = new HashSet<IAssetLinkGetter>();
        if (!fileSystem.File.Exists(filePath)) return results;

        using var stream = fileSystem.File.OpenRead(filePath);
        using var reader = new StreamReader(stream);
        var line = reader.ReadLine();
        while (line is not null) {
            var match = ScriptNameRegex.Match(line);
            if (match.Success) {
                var parentScriptName = match.Groups[2].Value;
                if (!string.IsNullOrWhiteSpace(parentScriptName)) {
                    var sourceFile = fileSystem.Path.Combine("Scripts", "Source", parentScriptName + ".psc");
                    var sourceLink = assetTypeService.GetAssetLink(sourceFile);
                    if (sourceLink is not null) {
                        results.Add(sourceLink);
                    }
                    var compiledFile = fileSystem.Path.Combine("Scripts", parentScriptName + ".pex");
                    var compiledLink = assetTypeService.GetAssetLink(compiledFile);
                    if (compiledLink is not null) {
                        results.Add(compiledLink);
                    }

                    return results;
                }
            }

            line = reader.ReadLine();
        }

        // TODO also include check for global script calls, and include those scripts as reference

        return results;
    }
}
