using System.IO.Abstractions;
using System.Text.RegularExpressions;
namespace CreationEditor.Services.Cache;

public sealed partial class CacheLocationProvider : ICacheLocationProvider {
    private const string CacheDirectory = "MutagenCache";
    private const string CacheExtension = "cache";

    [GeneratedRegex("[\\/:*?\"<>|]")]
    private static partial Regex IllegalFileNameRegex();

    private readonly IFileSystem _fileSystem;
    private readonly string _cacheDirPath;

    public CacheLocationProvider(
        IFileSystem fileSystem,
        params string[] subDirectories) {
        _fileSystem = fileSystem;

        var baseDir = _fileSystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CacheDirectory);
        _cacheDirPath = subDirectories.Aggregate(baseDir, (cur, next) => _fileSystem.Path.Combine(cur, IllegalFileNameRegex().Replace(next, string.Empty)));
    }

    private string GetIdentifierPath(string[] identifiers) {
        if (identifiers.Length == 0) throw new ArgumentException("Must provide at least one identifier", nameof(identifiers));

        var path = _cacheDirPath;
        for (var i = 0; i < identifiers.Length - 1; i++) {
            path = _fileSystem.Path.Combine(path, IllegalFileNameRegex().Replace(identifiers[i], string.Empty));
        }

        return path;
    }

    public string CacheFile(params string[] identifiers) => _fileSystem.Path.Combine(GetIdentifierPath(identifiers), IllegalFileNameRegex().Replace($"{identifiers[^1]}.{CacheExtension}", string.Empty));
}
