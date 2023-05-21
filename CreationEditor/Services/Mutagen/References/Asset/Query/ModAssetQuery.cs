using Autofac;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Type;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ModAssetQuery : AssetQuery<IModGetter, IFormLinkGetter> {
    private readonly IEnvironmentContext _environmentContext;
    private readonly IMutagenTypeProvider _mutagenTypeProvider;
    private IAssetLinkCache _assetLinkCache;

    protected override string QueryName => "Mod";
    protected override bool CacheAssets => true;
    public bool SkipResolvedAssets { get; set; } = true; // todo change back to false by default when inferred assets bug is fixed

    public ModAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {
        var newScope = lifetimeScope.BeginLifetimeScope().DisposeWith(this);
        var editorEnvironment = newScope.Resolve<IEditorEnvironment>();
        _environmentContext = newScope.Resolve<IEnvironmentContext>();
        _mutagenTypeProvider = newScope.Resolve<IMutagenTypeProvider>();

        _assetLinkCache = editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();

        editorEnvironment.LinkCacheChanged
            .Subscribe(linkCache => {
                _assetLinkCache?.Dispose();
                _assetLinkCache = linkCache.CreateImmutableAssetLinkCache();
            })
            .DisposeWith(this);
    }

    protected override string GetName(IModGetter origin) => origin.ModKey.FileName;

    public override IEnumerable<AssetQueryResult<IFormLinkGetter>> ParseAssets(IModGetter mod) {
        if (SkipResolvedAssets) {
            foreach (var record in mod.EnumerateMajorRecords()) {
                foreach (var assetLink in record.EnumerateAssetLinks(AssetLinkQuery.Listed | AssetLinkQuery.Inferred).Where(l => !l.IsNull)) {
                    yield return new AssetQueryResult<IFormLinkGetter>(assetLink, record.ToLinkFromRuntimeType()) { Origin = mod.ModKey };
                }
            }
        } else {
            foreach (var record in mod.EnumerateMajorRecords()) {
                foreach (var assetLink in record.EnumerateAllAssetLinks(_assetLinkCache).Where(l => !l.IsNull)) {
                    yield return new AssetQueryResult<IFormLinkGetter>(assetLink, record.ToLinkFromRuntimeType()) { Origin = mod.ModKey };
                }
            }
        }
    }

    protected override void WriteCacheCheck(BinaryWriter writer, IModGetter origin) {
        var modFilePath = FileSystem.Path.Combine(_environmentContext.DataDirectoryProvider.Path, origin.ModKey.FileName);
        if (!FileSystem.File.Exists(modFilePath)) return;

        var checksum = FileSystem.GetFileChecksum(modFilePath);
        writer.Write(checksum);
    }

    protected override void WriteContext(BinaryWriter writer, IModGetter origin) {
        // Write game
        writer.Write(_mutagenTypeProvider.GetGameName(origin));
    }

    protected override void WriteUsages(BinaryWriter writer, IEnumerable<IFormLinkGetter> usages) {
        foreach (var usage in usages) {
            writer.Write(usage.FormKey.ToString());
            writer.Write(_mutagenTypeProvider.GetTypeName(usage));
        }
    }

    protected override bool IsCacheUpToDate(BinaryReader binaryReader, IModGetter origin) {
        var modFilePath = FileSystem.Path.Combine(_environmentContext.DataDirectoryProvider.Path, origin.ModKey.FileName);
        if (!FileSystem.Path.Exists(modFilePath)) return false;

        // Read checksum in cache
        var checksum = binaryReader.ReadBytes(FileSystem.GetChecksumBytesLength());

        // Validate checksum
        return FileSystem.IsFileChecksumValid(modFilePath, checksum);
    }

    protected override string ReadContextString(BinaryReader reader) {
        // Read game string
        var game = reader.ReadString();
        return game;
    }

    protected override IEnumerable<IFormLinkGetter> ReadUsages(BinaryReader reader, string context, int count) {
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

    public override void Dispose() {
        base.Dispose();

        _assetLinkCache?.Dispose();
    }
}
