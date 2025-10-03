using System.Reactive.Disposables;
using System.Reactive.Linq;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Services.Notification;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ScriptFileParser = CreationEditor.Services.Mutagen.References.Parser.ScriptFileParser;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ReferenceService : IReferenceService {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IMutagenCommonAspectsProvider _mutagenCommonAspectsProvider;

    // Asset reference controllers
    private readonly ReferenceController<IModGetter, RecordModPair, AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter, IFormLinkIdentifier, IReferencedAsset> _recordAssetReferenceController;
    private readonly ReferenceController<IDataSource, DataSourceFileLink, AssetReferenceCache<DataRelativePath>, IAssetLinkGetter, DataRelativePath, IReferencedAsset> _nifTextureReferenceController;
    private readonly ReferenceController<IDataSource, DataSourceFileLink, AssetReferenceCache<DataRelativePath>, IAssetLinkGetter, DataRelativePath, IReferencedAsset> _scriptAssetReferenceController;

    // Record reference controllers
    private readonly ReferenceController<IModGetter, RecordModPair, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier, IReferencedRecord> _recordReferenceController;
    private readonly ReferenceController<IModGetter, RecordModPair, DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IFormLinkIdentifier, IReferencedRecord> _recordGlobalVariableReferenceController;
    private readonly ReferenceController<IDataSource, DataSourceFileLink, AssetDictionaryReferenceCache<string>, string, DataRelativePath, IReferencedRecord> _nifSoundReferenceController;
    private readonly ReferenceController<IDataSource, DataSourceFileLink, AssetDictionaryReferenceCache<uint>, uint, DataRelativePath, IReferencedRecord> _nifAddonNodeReferenceController;

    private readonly ReferenceSubscriptionManager<IFormLinkIdentifier, IFormLinkIdentifier, IReferencedRecord> _recordReferenceSubscriptionManager;
    private readonly ReferenceSubscriptionManager<string, IFormLinkIdentifier, IReferencedRecord> _recordGlobalVariableReferenceSubscriptionManager;
    private readonly ReferenceSubscriptionManager<string, DataRelativePath, IReferencedRecord> _assetRecordReferenceSubscriptionManager;
    private readonly ReferenceSubscriptionManager<uint, DataRelativePath, IReferencedRecord> _nifAddonNodeReferenceSubscriptionManager;
    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IFormLinkIdentifier, IReferencedAsset> _recordAssetReferenceSubscriptionManager;
    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, DataRelativePath, IReferencedAsset> _assetReferenceSubscriptionManager;

    public IObservable<bool> IsLoading { get; }
    public IObservable<bool> IsLoadingAssetReferences { get; }
    public IObservable<bool> IsLoadingRecordReferences { get; }

    public ReferenceService(
        IEditorEnvironment editorEnvironment,
        IDataSourceService dataSourceService,
        INotificationService notificationService,
        IMutagenCommonAspectsProvider mutagenCommonAspectsProvider,
        // Update Triggers
        ModReferenceUpdateTrigger<DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IReferencedRecord> stringModReferenceUpdateTrigger,
        ModReferenceUpdateTrigger<RecordReferenceCache, IFormLinkIdentifier, IReferencedRecord> modReferenceUpdateTrigger,
        ModReferenceUpdateTrigger<AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter, IReferencedAsset> modAssetReferenceUpdateTrigger,
        DataSourceReferenceUpdateTrigger<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter, IReferencedAsset> dataSourceReferenceUpdateTrigger,
        DataSourceReferenceUpdateTrigger<AssetDictionaryReferenceCache<uint>, uint, IReferencedRecord> addonNodeReferenceUpdateTrigger,
        DataSourceReferenceUpdateTrigger<AssetDictionaryReferenceCache<string>, string, IReferencedRecord> soundRecordReferenceUpdateTrigger,
        // Cache Controllers
        RecordReferenceCacheController recordReferenceCacheController,
        AssetReferenceCacheController<IModGetter, IFormLinkIdentifier> recordAssetReferenceCacheController,
        AssetReferenceCacheController<IDataSource, DataRelativePath> assetAssetReferenceCacheController,
        DictionaryReferenceCacheController<IModGetter, string, IFormLinkIdentifier> stringModDictionaryReferenceCacheController,
        AssetDictionaryReferenceCacheController<string> stringAssetDictionaryReferenceCacheController,
        AssetDictionaryReferenceCacheController<uint> intAssetDictionaryReferenceCacheController,
        // Query Configs
        RecordReferenceQueryConfig recordReferenceQueryConfig,
        RecordGlobalVariableReferenceQueryConfig recordGlobalVariableReferenceQueryConfig,
        RecordAssetReferenceQueryConfig recordAssetReferenceQueryConfig,
        DictionaryAssetReferenceQueryConfig<NifAddonNodeLinkParser, AssetDictionaryReferenceCache<uint>, uint> nifAddonNodeReferenceQueryConfig,
        DictionaryAssetReferenceQueryConfig<NifSoundLinkParser, AssetDictionaryReferenceCache<string>, string> nifSoundReferenceQueryConfig,
        AssetReferenceCacheQueryConfig<NifTextureParser> nifTextureReferenceQueryConfig,
        AssetReferenceCacheQueryConfig<ScriptFileParser> scriptAssetReferenceQueryConfig) {
        _editorEnvironment = editorEnvironment;
        _mutagenCommonAspectsProvider = mutagenCommonAspectsProvider;

        _recordReferenceSubscriptionManager =
            new ReferenceSubscriptionManager<IFormLinkIdentifier, IFormLinkIdentifier, IReferencedRecord>(
                record => editorEnvironment.LinkCache.ResolveMod(record.Record.FormKey.ModKey) is null,
                (record, change) => record.RecordReferences.Apply(change, FormLinkIdentifierEqualityComparer.Instance),
                (record, newData) => record.RecordReferences.AddRange(newData),
                asset => asset.Record);

        _recordGlobalVariableReferenceSubscriptionManager =
            new ReferenceSubscriptionManager<string, IFormLinkIdentifier, IReferencedRecord>(
                record => editorEnvironment.LinkCache.ResolveMod(record.Record.FormKey.ModKey) is null,
                (record, change) => record.RecordReferences.Apply(change, FormLinkIdentifierEqualityComparer.Instance),
                (record, newData) => record.RecordReferences.AddRange(newData),
                record => record.Record.EditorID ?? string.Empty);

        _assetRecordReferenceSubscriptionManager = new ReferenceSubscriptionManager<string, DataRelativePath, IReferencedRecord>(
            record => editorEnvironment.LinkCache.ResolveMod(record.Record.FormKey.ModKey) is null,
            (record, change) => record.AssetReferences.Apply(change),
            (record, newData) => record.AssetReferences.AddRange(newData),
            record => record.Record.EditorID ?? string.Empty);

        _nifAddonNodeReferenceSubscriptionManager = new ReferenceSubscriptionManager<uint, DataRelativePath, IReferencedRecord>(
            record => editorEnvironment.LinkCache.ResolveMod(record.Record.FormKey.ModKey) is null,
            (record, change) => record.AssetReferences.Apply(change),
            (record, newData) => record.AssetReferences.AddRange(newData),
            record => mutagenCommonAspectsProvider.GetAddonNodeIndex(record.Record) ?? 0);

        _recordAssetReferenceSubscriptionManager = new ReferenceSubscriptionManager<IAssetLinkGetter, IFormLinkIdentifier, IReferencedAsset>(
            asset => !dataSourceService.FileExists(asset.AssetLink.DataRelativePath),
            (asset, change) => asset.RecordReferences.Apply(change),
            (record, newData) => record.RecordReferences.AddRange(newData),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

        _assetReferenceSubscriptionManager = new ReferenceSubscriptionManager<IAssetLinkGetter, DataRelativePath, IReferencedAsset>(
            asset => !dataSourceService.FileExists(asset.AssetLink.DataRelativePath),
            (asset, change) => asset.AssetReferences.Apply(change),
            (record, newData) => record.AssetReferences.AddRange(newData),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

        _assetReferenceSubscriptionManager = new ReferenceSubscriptionManager<IAssetLinkGetter, DataRelativePath, IReferencedAsset>(
            asset => !dataSourceService.FileExists(asset.AssetLink.DataRelativePath),
            (asset, change) => asset.AssetReferences.Apply(change),
            (record, newData) => record.AssetReferences.AddRange(newData),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

        _recordReferenceController =
            new ReferenceController<IModGetter, RecordModPair, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier, IReferencedRecord>(
                notificationService,
                modReferenceUpdateTrigger,
                recordReferenceCacheController,
                recordReferenceQueryConfig,
                _recordReferenceSubscriptionManager);

        _recordGlobalVariableReferenceController =
            new ReferenceController<IModGetter, RecordModPair, DictionaryReferenceCache<string, IFormLinkIdentifier>, string, IFormLinkIdentifier, IReferencedRecord>(
                notificationService,
                stringModReferenceUpdateTrigger,
                stringModDictionaryReferenceCacheController,
                recordGlobalVariableReferenceQueryConfig,
                _recordGlobalVariableReferenceSubscriptionManager);

        _recordAssetReferenceController =
            new ReferenceController<IModGetter, RecordModPair, AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter, IFormLinkIdentifier,
                IReferencedAsset>(
                notificationService,
                modAssetReferenceUpdateTrigger,
                recordAssetReferenceCacheController,
                recordAssetReferenceQueryConfig,
                _recordAssetReferenceSubscriptionManager);

        _nifAddonNodeReferenceController =
            new ReferenceController<IDataSource, DataSourceFileLink, AssetDictionaryReferenceCache<uint>, uint, DataRelativePath, IReferencedRecord>(
                notificationService,
                addonNodeReferenceUpdateTrigger,
                intAssetDictionaryReferenceCacheController,
                nifAddonNodeReferenceQueryConfig,
                _nifAddonNodeReferenceSubscriptionManager);

        _nifTextureReferenceController =
            new ReferenceController<IDataSource, DataSourceFileLink, AssetReferenceCache<DataRelativePath>, IAssetLinkGetter, DataRelativePath,
                IReferencedAsset>(
                notificationService,
                dataSourceReferenceUpdateTrigger,
                assetAssetReferenceCacheController,
                nifTextureReferenceQueryConfig,
                _assetReferenceSubscriptionManager);

        _nifSoundReferenceController =
            new ReferenceController<IDataSource, DataSourceFileLink, AssetDictionaryReferenceCache<string>, string, DataRelativePath, IReferencedRecord>(
                notificationService,
                soundRecordReferenceUpdateTrigger,
                stringAssetDictionaryReferenceCacheController,
                nifSoundReferenceQueryConfig,
                _assetRecordReferenceSubscriptionManager);

        _scriptAssetReferenceController =
            new ReferenceController<IDataSource, DataSourceFileLink, AssetReferenceCache<DataRelativePath>, IAssetLinkGetter, DataRelativePath,
                IReferencedAsset>(
                notificationService,
                dataSourceReferenceUpdateTrigger,
                assetAssetReferenceCacheController,
                scriptAssetReferenceQueryConfig,
                _assetReferenceSubscriptionManager);

        IsLoadingAssetReferences = _recordAssetReferenceController.IsLoading
            .CombineLatest(_nifTextureReferenceController.IsLoading,
                _scriptAssetReferenceController.IsLoading,
                (a, b, c) => a || b || c);

        IsLoadingRecordReferences = _recordReferenceController.IsLoading
            .CombineLatest(_recordGlobalVariableReferenceController.IsLoading, 
                _nifSoundReferenceController.IsLoading,
                _nifAddonNodeReferenceController.IsLoading,
                (a, b, c, d) => a || b || c || d);

        IsLoading = IsLoadingAssetReferences.CombineLatest(IsLoadingRecordReferences, (a, b) => a || b);
    }

    public IDisposable GetReferencedAsset(IAssetLinkGetter asset, out IReferencedAsset referencedAsset) {
        var recordAssetReferences = _recordAssetReferenceController.GetReferences(asset);
        var nifTextureReferences = _nifTextureReferenceController.GetReferences(asset);
        var scriptReferences = _scriptAssetReferenceController.GetReferences(asset);
        referencedAsset = new ReferencedAsset(asset, recordAssetReferences, nifTextureReferences.Concat(scriptReferences));

        var recordDisposable = _recordAssetReferenceSubscriptionManager.Register(referencedAsset);
        var assetDisposable = _assetReferenceSubscriptionManager.Register(referencedAsset);

        return new CompositeDisposable(recordDisposable, assetDisposable);
    }

    public IDisposable GetReferencedRecord<TMajorRecordGetter>(
        TMajorRecordGetter record,
        out IReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordGetter {
        var recordReferences = _recordReferenceController.GetReferences(record);
        var editorId = record.EditorID;
        if (editorId is not null) {
            recordReferences = recordReferences.Concat(_recordGlobalVariableReferenceController.GetReferences(editorId));
        }

        var assetReferences = editorId is not null ? _nifSoundReferenceController.GetReferences(editorId) : [];
        var addonNodeIndex = _mutagenCommonAspectsProvider.GetAddonNodeIndex(record);
        if (addonNodeIndex is not null) {
            assetReferences = assetReferences.Concat(_nifAddonNodeReferenceController.GetReferences(addonNodeIndex.Value));
        }

        outReferencedRecord = new ReferencedRecord<TMajorRecordGetter>(record, recordReferences, assetReferences);

        var nifAddonNodeDisposable = _nifAddonNodeReferenceSubscriptionManager.Register(outReferencedRecord);
        var assetRecordDisposable = _assetRecordReferenceSubscriptionManager.Register(outReferencedRecord);
        var recordDisposable = _recordReferenceSubscriptionManager.Register(outReferencedRecord);
        var globalVariableDisposable = _recordGlobalVariableReferenceSubscriptionManager.Register(outReferencedRecord);
        return new CompositeDisposable(nifAddonNodeDisposable, assetRecordDisposable, recordDisposable, globalVariableDisposable);
    }

    public IEnumerable<IFormLinkIdentifier> GetRecordReferences(IFormLinkIdentifier formLink) {
        var references = _recordReferenceController.GetReferences(formLink);

        if (formLink.Type.InheritsFrom(_mutagenCommonAspectsProvider.GlobalVariableType)
         && _editorEnvironment.LinkCache.TryResolveIdentifier(formLink.FormKey, _mutagenCommonAspectsProvider.GlobalVariableType, out var identifier) && identifier is not null) {
            references = references.Concat(_recordGlobalVariableReferenceController.GetReferences(identifier));
        }

        return references;
    }

    public IEnumerable<IFormLinkIdentifier> GetRecordReferences(IAssetLinkGetter assetLink) {
        return _recordAssetReferenceController.GetReferences(assetLink);
    }

    public IEnumerable<DataRelativePath> GetAssetReferences(IFormLinkIdentifier formLink) {
        if (formLink.Type.InheritsFrom(_mutagenCommonAspectsProvider.AddonNodeRecordType)
          && _editorEnvironment.LinkCache.TryResolve(formLink, out var addonNode)) {
            // Record is an addon node
            var addonNodeIndex = _mutagenCommonAspectsProvider.GetAddonNodeIndex(addonNode);
            if (addonNodeIndex is not null) {
                return _nifAddonNodeReferenceController.GetReferences(addonNodeIndex.Value);
            }
        } else if (formLink.Type.InheritsFrom(_mutagenCommonAspectsProvider.SoundDescriptorRecordType)
         && _editorEnvironment.LinkCache.TryResolve(formLink, out var soundDescriptor)
         && soundDescriptor is { EditorID: {} soundDescriptorEditorId }) {
            // Record is a sound
            return _nifSoundReferenceController.GetReferences(soundDescriptorEditorId);
        } else if (formLink.Type.InheritsFrom(_mutagenCommonAspectsProvider.SoundMarkerRecordType)
         && _editorEnvironment.LinkCache.TryResolve(formLink, out var soundMarker)
         && soundMarker is { EditorID: {} soundMarkerEditorId }) {
            // Record is a sound
            return _nifSoundReferenceController.GetReferences(soundMarkerEditorId);
        }

        return [];
    }

    public IEnumerable<DataRelativePath> GetAssetReferences(IAssetLinkGetter assetLink) {
        var nifTextureReferences = _nifTextureReferenceController.GetReferences(assetLink);
        var scriptReferences = _scriptAssetReferenceController.GetReferences(assetLink);

        return nifTextureReferences.Concat(scriptReferences);
    }

    public IEnumerable<string> GetMissingRecordLinks(DataSourceFileLink fileFileLink) {
        var addonNodeIndices = _nifAddonNodeReferenceController.GetLinks(fileFileLink).ToHashSet();

        if (addonNodeIndices.Count > 0) {
            foreach (var addonNode in
                _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides(_mutagenCommonAspectsProvider.AddonNodeRecordType)) {
                var addonNodeIndex = _mutagenCommonAspectsProvider.GetAddonNodeIndex(addonNode);
                if (addonNodeIndex is null) continue;

                addonNodeIndices.Remove(addonNodeIndex.Value);
            }

            foreach (var addonNodeIndex in addonNodeIndices) {
                yield return "Addon Node: " + addonNodeIndex;
            }
        }

        var soundEditorIds = _nifSoundReferenceController.GetLinks(fileFileLink).ToArray();
        if (soundEditorIds.Length > 0) {
            foreach (var soundEditorId in soundEditorIds) {
                if (_editorEnvironment.LinkCache.TryResolve(soundEditorId, _mutagenCommonAspectsProvider.SoundDescriptorRecordType, out _)) continue;
                if (_editorEnvironment.LinkCache.TryResolve(soundEditorId, _mutagenCommonAspectsProvider.SoundMarkerRecordType, out _)) continue;

                yield return "Sound: " + soundEditorId;
            }
        }
    }

    public IEnumerable<IAssetLinkGetter> GetAssetLinks(DataSourceFileLink fileFileLink) {
        return _nifTextureReferenceController.GetLinks(fileFileLink)
            .Concat(_scriptAssetReferenceController.GetLinks(fileFileLink));
    }

    public int GetReferenceCount(IAssetLink assetLink) {
        return GetRecordReferences(assetLink).Count() + GetAssetReferences(assetLink).Count();
    }

    public int GetReferenceCount(IFormLinkIdentifier formLink) {
        return GetRecordReferences(formLink).Count() + GetAssetReferences(formLink).Count();
    }
}
