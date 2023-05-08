using System.IO.Abstractions;
using System.Text.RegularExpressions;
namespace CreationEditor.Services.Cache;

public sealed partial class CacheLocationProvider : ICacheLocationProvider {
    private const string CacheDirectory = "MutagenCache";
    private const string CacheExtension = "cache";
    private const string TempCacheExtension = "temp";

    [GeneratedRegex("[\\/:*?\"<>|]")]
    private static partial Regex IllegalFileNameRegex();

    private readonly IFileSystem _fileSystem;
    private readonly string _subDirectory;

    public CacheLocationProvider(
        IFileSystem fileSystem,
        string subDirectory) {
        _fileSystem = fileSystem;
        _subDirectory = IllegalFileNameRegex().Replace(subDirectory, string.Empty);

    }

    private string GetIdentifierPath(string[] identifiers) {
        if (identifiers.Length == 0) throw new ArgumentException("Must provide at least one identifier", nameof(identifiers));

        var path = CacheDirPath();
        for (var i = 0; i < identifiers.Length - 1; i++) {
            path = _fileSystem.Path.Combine(path, IllegalFileNameRegex().Replace(identifiers[i], string.Empty));
        }

        return path;
    }

    private string CacheDirPath() => _fileSystem.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), CacheDirectory, _subDirectory);
    public string CacheFile(params string[] identifiers) => _fileSystem.Path.Combine(GetIdentifierPath(identifiers), $"{identifiers[^1]}.{CacheExtension}");
    public string TempCacheFile(params string[] identifiers) => _fileSystem.Path.Combine(GetIdentifierPath(identifiers), $"{identifiers[^1]}.{TempCacheExtension}");
}
