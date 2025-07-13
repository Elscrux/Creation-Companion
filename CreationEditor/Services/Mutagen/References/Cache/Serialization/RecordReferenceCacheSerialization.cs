using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Services.Mutagen.Type;
using ICSharpCode.SharpZipLib.GZip;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public sealed class RecordReferenceCacheSerialization(
    Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
    IMutagenTypeProvider mutagenTypeProvider,
    IDataSourceService dataSourceService,
    IFileSystem fileSystem)
    : IReferenceCacheSerialization<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> {

    private static readonly IReadOnlyList<string> CacheLocation = ["References"];

    private readonly ICacheLocationProvider _cacheLocationProvider = cacheLocationProviderFactory(CacheLocation);

    public Version CacheVersion { get; } = new(2, 0);

    private string? GetCacheFile(IModGetter source, IReferenceQuery<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> query) {
        // Get mod path
        var modKey = source.ModKey;
        var link = dataSourceService.GetFileLink(new DataRelativePath(modKey.FileName));
        if (link is null || !link.Exists()) return null;

        // Create a unique cache file path for game, cache version, mod filename and mod hash
        // Example: Skyrim/v2.0/NewMod.esp_4B0B3420E493A066.cache
        var gameName = mutagenTypeProvider.GetGameName(source);
        var modHash = link.FileSystem.GetFileHash(link.FullPath);
        return _cacheLocationProvider.CacheFile(query.Name, gameName, $"v{CacheVersion}", $"{modKey.FileName}_{Convert.ToHexString(modHash)}");
    }

    private string GetGameName(string cacheFilePath) =>
        cacheFilePath.Split(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar)[^3];

    public bool Validate(
        IModGetter source,
        IReferenceQuery<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> query) {
        var cacheFilePath = GetCacheFile(source, query);
        return fileSystem.File.Exists(cacheFilePath);
    }

    public RecordReferenceCache Deserialize(
        IModGetter source,
        IReferenceQuery<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> query) {
        var cacheFilePath = GetCacheFile(source, query);
        if (cacheFilePath is null || !fileSystem.File.Exists(cacheFilePath)) {
            throw new FileNotFoundException($"Cache file not found: {cacheFilePath}");
        }

        var game = GetGameName(cacheFilePath);

        // Read mod cache file
        var fileStream = fileSystem.File.OpenRead(cacheFilePath);
        var zip = new GZipInputStream(fileStream);
        using var reader = new BinaryReader(zip);

        // Read form keys
        var recordCount = reader.ReadInt32();
        var records = new HashSet<FormKey>(recordCount);
        for (var i = 0; i < recordCount; i++) {
            var formKey = FormKey.Factory(reader.ReadString());
            records.Add(formKey);
        }

        // Read references
        var formCount = reader.ReadInt32();
        var modCache = new ConcurrentDictionary<FormKey, HashSet<IFormLinkIdentifier>>();
        for (var i = 0; i < formCount; i++) {
            var formKey = FormKey.Factory(reader.ReadString());
            var referenceCount = reader.ReadInt32();
            var references = new HashSet<IFormLinkIdentifier>(referenceCount);
            for (var j = 0; j < referenceCount; j++) {
                var referenceFormKey = FormKey.Factory(reader.ReadString());
                var typeString = reader.ReadString();
                var type = mutagenTypeProvider.GetType(game, typeString);
                references.Add(new FormLinkInformation(referenceFormKey, type));
            }

            modCache.TryAdd(formKey, references);
        }

        return new RecordReferenceCache(modCache, records);
    }

    public void Serialize(
        IModGetter source,
        IReferenceQuery<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> query,
        RecordReferenceCache cache) {
        var cacheFilePath = GetCacheFile(source, query);
        if (cacheFilePath is null) return;

        // Open writer
        var cacheFile = fileSystem.FileInfo.New(cacheFilePath);
        if (!cacheFile.Exists) cacheFile.Directory?.Create();
        using var fileStream = fileSystem.File.OpenWrite(cacheFile.FullName);
        using var zip = new GZipOutputStream(fileStream);
        using var writer = new BinaryWriter(zip);

        // Write form keys
        writer.Write(cache.FormKeys.Count);
        foreach (var formKey in cache.FormKeys) {
            writer.Write(formKey.ToString());
        }

        // Write referenced form count
        writer.Write(cache.Cache.Count);

        // Write references
        foreach (var (formKey, references) in cache.Cache) {
            writer.Write(formKey.ToString());
            writer.Write(references.Count);
            foreach (var reference in references) {
                writer.Write(reference.FormKey.ToString());
                writer.Write(mutagenTypeProvider.GetTypeName(reference));
            }
        }
    }
}
