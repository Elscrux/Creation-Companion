using System.Text.RegularExpressions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed partial class ScriptFileParser(IAssetTypeService assetTypeService)
    : IFileParser<IAssetLinkGetter> {

    [GeneratedRegex(@"ScriptName\s+([a-zA-Z_]\w*)(?:\s+extends\s+([a-zA-Z_]\w*))?", RegexOptions.IgnoreCase)]
    private static partial Regex ScriptNameRegex { get; }

    public string Name => "Scripts";
    public IAssetType AssetType => assetTypeService.Provider.ScriptSource;

    public IEnumerable<IAssetLinkGetter> ParseFile(string actualFilePath, DataSourceFileLink fileLink) {
        var results = new HashSet<IAssetLinkGetter>();
        if (!fileLink.DataSource.FileSystem.File.Exists(actualFilePath)) return results;

        using var stream = fileLink.FileSystem.File.Open(actualFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream);
        var line = reader.ReadLine();
        while (line is not null) {
            var match = ScriptNameRegex.Match(line);
            if (match.Success) {
                var parentScriptName = match.Groups[2].Value;
                if (!string.IsNullOrWhiteSpace(parentScriptName)) {
                    var sourceFile = fileLink.FileSystem.Path.Combine("Scripts", "Source", parentScriptName + ".psc");
                    var sourceLink = assetTypeService.GetAssetLink(sourceFile);
                    if (sourceLink is not null) {
                        results.Add(sourceLink);
                    }
                    var compiledFile = fileLink.FileSystem.Path.Combine("Scripts", parentScriptName + ".pex");
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
