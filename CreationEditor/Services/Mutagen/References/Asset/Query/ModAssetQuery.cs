using System.IO.Abstractions;
using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ModAssetQuery : IAssetReferenceCacheableQuery<IModGetter, IFormLinkGetter>, IDisposable {
    private readonly IDisposableDropoff _disposableDropoff = new DisposableBucket();

    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;
    private IAssetLinkCache _assetLinkCache;

    public Version CacheVersion { get; } = new(1, 0);
    public IAssetReferenceSerialization<IModGetter, IFormLinkGetter> Serialization { get; }
    public IInternalCacheValidation<IModGetter, IFormLinkGetter>? CacheValidation => null;
    public string QueryName => "Mod";
    public Dictionary<IModGetter, AssetReferenceCache<IModGetter, IFormLinkGetter>> AssetCaches { get; } = new();

    public bool SkipResolvedAssets { get; set; } = true; // todo change back to false by default when inferred assets bug is fixed

    public ModAssetQuery(
        IFileSystem fileSystem,
        ILinkCacheProvider linkCacheProvider,
        IDataDirectoryProvider dataDirectoryProvider,
        IMutagenTypeProvider mutagenTypeProvider,
        IAssetReferenceSerialization<IModGetter, IFormLinkGetter> serialization) {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        _mutagenTypeProvider = mutagenTypeProvider;
        _assetLinkCache = linkCacheProvider.LinkCache.CreateImmutableAssetLinkCache();
        Serialization = serialization;

        linkCacheProvider.LinkCacheChanged
            .Subscribe(linkCache => {
                _assetLinkCache?.Dispose();
                _assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
            })
            .DisposeWith(_disposableDropoff);
    }

    public string GetName(IModGetter source) => source.ModKey.FileName;

    public IEnumerable<AssetQueryResult<IFormLinkGetter>> ParseAssets(IModGetter source) {
        if (SkipResolvedAssets) {
            foreach (var record in source.EnumerateMajorRecords()) {
                foreach (var assetLink in record.EnumerateAssetLinks(AssetLinkQuery.Listed | AssetLinkQuery.Inferred).Where(l => !l.IsNull)) {
                    yield return new AssetQueryResult<IFormLinkGetter>(assetLink, record.ToLinkFromRuntimeType());
                }
            }
        } else {
            foreach (var record in source.EnumerateMajorRecords()) {
                foreach (var assetLink in record.EnumerateAllAssetLinks(_assetLinkCache).Where(l => !l.IsNull)) {
                    yield return new AssetQueryResult<IFormLinkGetter>(assetLink, record.ToLinkFromRuntimeType());
                }
            }
        }
    }

    public void WriteCacheCheck(BinaryWriter writer, IModGetter mod) {
        var modFilePath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, mod.ModKey.FileName);
        if (!_fileSystem.File.Exists(modFilePath)) return;

        var hash = _fileSystem.GetFileHash(modFilePath);
        writer.Write(hash);
    }

    public void WriteContext(BinaryWriter writer, IModGetter source) {
        // Write game
        writer.Write(_mutagenTypeProvider.GetGameName(source));
    }

    public void WriteUsages(BinaryWriter writer, IEnumerable<IFormLinkGetter> usages) {
        foreach (var usage in usages) {
            writer.Write(usage.FormKey.ToString());
            writer.Write(_mutagenTypeProvider.GetTypeName(usage));
        }
    }

    public bool IsCacheUpToDate(BinaryReader binaryReader, IModGetter source) {
        var modFilePath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, source.ModKey.FileName);
        if (!_fileSystem.Path.Exists(modFilePath)) return false;

        // Read hash in cache
        var hash = binaryReader.ReadBytes(_fileSystem.GetHashBytesLength());

        // Validate hash
        return _fileSystem.IsFileHashValid(modFilePath, hash);
    }

    public string ReadContextString(BinaryReader reader) {
        // Read game string
        var game = reader.ReadString();
        return game;
    }

    public IEnumerable<IFormLinkGetter> ReadUsages(BinaryReader reader, string context, int count) {
        for (var i = 0; i < count; i++) {
            var referenceFormKey = FormKey.Factory(reader.ReadString());
            var typeString = reader.ReadString();
            if (_mutagenTypeProvider.GetType(context, typeString, out var type)) {
                yield return new FormLinkInformation(referenceFormKey, type);
            } else {
                throw new ArgumentException();
            }
        }
    }

    public void Dispose() {
        _disposableDropoff.Dispose();
        _assetLinkCache?.Dispose();
    }
}
