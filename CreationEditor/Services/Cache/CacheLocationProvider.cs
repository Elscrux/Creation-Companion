using System.IO.Abstractions;
using System.Text.RegularExpressions;
namespace CreationEditor.Services.Cache;

public sealed partial class CacheLocationProvider : ICacheLocationProvider {
    private const string CacheDirectory = "MutagenCache";
    private const string CacheExtension = "cache";

    [GeneratedRegex("[\\/:*?\"<>|]")]
    private static partial Regex IllegalFileNameRegex { get; }

    private readonly IFileSystem _fileSystem;
    private readonly string _cacheDirPath;

    public CacheLocationProvider(
        IFileSystem fileSystem,
        params IReadOnlyList<string> subDirectories) {
        _fileSystem = fileSystem;

        var baseDir = _fileSystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CacheDirectory);
        _cacheDirPath = subDirectories.Aggregate(
            baseDir,
            (cur, next) => {
                var sanitizedName = IllegalFileNameRegex.Replace(next, string.Empty);
                return _fileSystem.Path.Combine(cur, sanitizedName);
            });
    }

    private string GetIdentifierPath(IReadOnlyList<string> identifiers) {
        if (identifiers.Count == 0) throw new ArgumentException("Must provide at least one identifier", nameof(identifiers));

        var path = _cacheDirPath;
        for (var i = 0; i < identifiers.Count - 1; i++) {
            path = _fileSystem.Path.Combine(path, IllegalFileNameRegex.Replace(identifiers[i], string.Empty));
        }

        return path;
    }

    public string CacheFile(params IReadOnlyList<string> identifiers) {
        var sanitizedName = IllegalFileNameRegex.Replace($"{identifiers[^1]}.{CacheExtension}", string.Empty);
        return _fileSystem.Path.Combine(GetIdentifierPath(identifiers), sanitizedName);
    }
}
