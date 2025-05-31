using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using CreationEditor.Services.Notification;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Controller;

public sealed class AssetReferenceController : IAssetReferenceController {
    private readonly CompositeDisposable _disposables = new();
    private readonly CompositeDisposable _dataSourceUpdatedDisposables = new();

    private readonly ILogger _logger;
    private readonly IDataSourceService _dataSourceService;
    private readonly INotificationService _notificationService;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly NifFileAssetParser _nifFileAssetParser;
    private readonly IAssetReferenceCacheFactory _assetReferenceCacheFactory;

    private readonly ConcurrentQueue<RecordModPair> _recordCreations = new();
    private readonly ConcurrentQueue<RecordModPair> _recordDeletions = new();

    private readonly ConcurrentQueue<FileSystemLink> _assetCreations = new();
    private readonly ConcurrentQueue<FileSystemLink> _assetDeletions = new();

    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IReferencedAsset, IFormLinkIdentifier> _modAssetReferenceManager
        = new((asset, change) => asset.RecordReferences.Apply(change),
            (record, newData) => record.RecordReferences.Load(newData),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IReferencedAsset, DataRelativePath> _nifDirectoryAssetReferenceManager
        = new((asset, change) => asset.NifDirectoryReferences.Apply(change),
            (record, newData) => record.NifDirectoryReferences.Load(newData),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IReferencedAsset, DataRelativePath> _nifArchiveAssetReferenceManager
        = new((asset, change) => asset.NifArchiveReferences.Apply(change),
            (record, newData) => record.NifArchiveReferences.Load(newData),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

    private readonly List<AssetReferenceCache<IModGetter, IFormLinkIdentifier>> _modAssetCaches = [];
    private readonly List<AssetReferenceCache<FileSystemLink, DataRelativePath>> _nifDirectoryAssetReferenceCaches = [];
    private readonly List<AssetReferenceCache<FileSystemLink, DataRelativePath>> _nifArchiveAssetCaches = [];

    private int _loadingProcesses;
    private readonly BehaviorSubject<bool> _isLoading = new(true);
    public IObservable<bool> IsLoading => _isLoading;

    public AssetReferenceController(
        ILogger logger,
        IDataSourceService dataSourceService,
        IDataSourceWatcherProvider dataSourceWatcherProvider,
        INotificationService notificationService,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        NifFileAssetParser nifFileAssetParser,
        IAssetReferenceCacheFactory assetReferenceCacheFactory) {
        _logger = logger;
        _dataSourceService = dataSourceService;
        var dataSourceWatcherProvider1 = dataSourceWatcherProvider;
        _notificationService = notificationService;
        _editorEnvironment = editorEnvironment;
        _nifFileAssetParser = nifFileAssetParser;
        _assetReferenceCacheFactory = assetReferenceCacheFactory;

        _editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => UpdateLoadingProcess(InitLoadOrderReferences).FireAndForget(
                e => logger.Here().Error(e, "Failed to load Mod Asset References {Exception}", e.Message)))
            .DisposeWith(_disposables);

        _dataSourceService.DataSourcesChanged
            .ObserveOnTaskpool()
            .Subscribe(dataSources => {
                _dataSourceUpdatedDisposables.Clear();

                var fileSystemDataSources = dataSources.OfType<FileSystemDataSource>().ToArray();
                var archiveDataSources = dataSources.OfType<ArchiveDataSource>().ToArray();
                Task.Run(() => UpdateLoadingProcess(() => InitNifDirectoryReferences(fileSystemDataSources)))
                    .FireAndForget(e => logger.Here().Error(e, "Failed to load Nif Directory Asset References {Exception}", e.Message));
                Task.Run(() => UpdateLoadingProcess(() => InitNifArchiveReferences(archiveDataSources)))
                    .FireAndForget(e => logger.Here().Error(e, "Failed to load Nif Archive Asset References {Exception}", e.Message));

                foreach (var dataSource in fileSystemDataSources) {
                    var watcher = dataSourceWatcherProvider1.GetWatcher(dataSource);
                    watcher.CreatedFile
                        .Subscribe(RegisterCreation)
                        .DisposeWith(_dataSourceUpdatedDisposables);

                    watcher.DeletedFile
                        .Subscribe(RegisterDeletion)
                        .DisposeWith(_dataSourceUpdatedDisposables);

                    watcher.RenamedFile
                        .Subscribe(x => {
                            RegisterDeletion(x.Old);
                            RegisterCreation(x.New);
                        })
                        .DisposeWith(_dataSourceUpdatedDisposables);

                    watcher.Changed
                        .Subscribe(link => {
                            var registerUpdate = RegisterUpdate(link);
                            registerUpdate(link);
                        })
                        .DisposeWith(_dataSourceUpdatedDisposables);

                }
            })
            .DisposeWith(_disposables);

        recordController.RecordChangedDiff.Subscribe(RegisterUpdate).DisposeWith(_disposables);
        recordController.RecordCreated.Subscribe(RegisterCreation).DisposeWith(_disposables);
        recordController.WinningRecordDeleted.Subscribe(RegisterDeletion).DisposeWith(_disposables);
    }

    public void Dispose() => _disposables.Dispose();

    private async Task InitLoadOrderReferences() {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Asset References");

        var modKeys = _editorEnvironment.LinkCache.PriorityOrder.Select(mod => mod.ModKey).ToList();

        // Remove references from unloaded mods
        _modAssetCaches.RemoveWhere(c => !modKeys.Contains(c.Source.ModKey));
        _modAssetReferenceManager.UnregisterWhere(asset => asset.ModKey != ModKey.Null && !modKeys.Contains(asset.ModKey));

        // Add references for new mods
        var tasks = _editorEnvironment.LinkCache.PriorityOrder
            .Where(mod => _modAssetCaches.TrueForAll(c => c.Source.ModKey != mod.ModKey))
            .Select(mod => _assetReferenceCacheFactory.GetModCache(mod));

        foreach (var assetCache in await Task.WhenAll(tasks)) {
            _modAssetCaches.Add(assetCache);
        }

        // Update existing subscriptions
        _modAssetReferenceManager.UpdateAll(_modAssetCaches.GetReferences);

        // Handle previous record creations and deletions  while the reference cache wasn't initialized
        while (_recordCreations.TryDequeue(out var record)) RegisterCreation(record);
        while (_recordDeletions.TryDequeue(out var record)) RegisterDeletion(record);

        linearNotifier.Stop();
    }

    private async Task InitNifDirectoryReferences(IReadOnlyList<FileSystemDataSource> fileSystemDataSources) {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Nif References");

        // Remove active subscriptions from unloaded directories
        _nifDirectoryAssetReferenceCaches.RemoveWhere(f => !fileSystemDataSources.Contains(f.Source.DataSource));

        // Remove references from unloaded directories
        _nifDirectoryAssetReferenceManager.UnregisterWhere(asset => !_dataSourceService.FileExists(asset.AssetLink.DataRelativePath));

        // Add references for new directories
        var tasks = fileSystemDataSources
            .Select(dataSource => Task.Run(async () => {
                var fileSystemLink = new FileSystemLink(dataSource, string.Empty);
                return await _assetReferenceCacheFactory.GetNifCache(fileSystemLink);
            }))
            .ToArray();

        foreach (var assetCache in await Task.WhenAll(tasks)) {
            _nifDirectoryAssetReferenceCaches.Add(assetCache);
        }

        // Change existing subscriptions
        _nifDirectoryAssetReferenceManager.UpdateAll(_nifDirectoryAssetReferenceCaches.GetReferences);

        // Handle previous record creations and deletions  while the reference cache wasn't initialized
        while (_assetCreations.TryDequeue(out var asset)) RegisterCreation(asset);
        while (_assetDeletions.TryDequeue(out var asset)) RegisterDeletion(asset);

        linearNotifier.Stop();
    }

    private async Task InitNifArchiveReferences(IReadOnlyList<ArchiveDataSource> archiveDataSources) {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Archive Nif References");

        // Remove active subscriptions from unloaded archives
        _nifDirectoryAssetReferenceManager.UnregisterWhere(asset => !_dataSourceService.FileExists(asset.AssetLink.DataRelativePath));

        // Remove references from unloaded archives
        var currentArchives = archiveDataSources.Select(x => x.ArchiveLink).ToArray();
        _nifDirectoryAssetReferenceCaches.RemoveWhere(f => !currentArchives.Contains(f.Source));

        // Add references for new archives
        var tasks = archiveDataSources
            .Where(dataSource => _nifArchiveAssetCaches.Any(x => x.Source.Equals(dataSource.ArchiveLink)))
            .Select(async archive => {
                using var notifier = new ChainedNotifier(_notificationService, $"Loading Archive Nif References in {archive.Name}");
                var newCache = await _assetReferenceCacheFactory.GetNifArchiveCache(archive.ArchiveLink);

                notifier.Stop();

                return newCache;
            });

        foreach (var assetCache in await Task.WhenAll(tasks)) {
            _nifArchiveAssetCaches.Add(assetCache);
        }

        // Change existing subscriptions
        _nifArchiveAssetReferenceManager.UpdateAll(_nifArchiveAssetCaches.GetReferences);

        linearNotifier.Stop();

        _logger.Here().Information("Loaded Nif References for {Count} Archives", _nifArchiveAssetCaches.Count);
    }

    public IDisposable GetReferencedAsset(IAssetLinkGetter asset, out IReferencedAsset referencedAsset) {
        var modReferences = _modAssetCaches.GetReferences(asset);
        var nifDirectoryReferences = _nifDirectoryAssetReferenceCaches.GetReferences(asset);
        var nifArchiveReferences = _nifArchiveAssetCaches.GetReferences(asset);
        referencedAsset = new ReferencedAsset(asset, modReferences, nifDirectoryReferences, nifArchiveReferences);

        var pluginDisposable = _modAssetReferenceManager.Register(referencedAsset);
        var nifDisposable = _nifDirectoryAssetReferenceManager.Register(referencedAsset);
        var nifArchiveDisposable = _nifArchiveAssetReferenceManager.Register(referencedAsset);

        return new CompositeDisposable(pluginDisposable, nifDisposable, nifArchiveDisposable);
    }

    public IEnumerable<IFormLinkIdentifier> GetRecordReferences(IAssetLinkGetter assetLink) {
        return _modAssetCaches.GetReferences(assetLink);
    }

    public IEnumerable<DataRelativePath> GetAssetReferences(IAssetLinkGetter assetLink) {
        var nifDirectoryReferences = _nifDirectoryAssetReferenceCaches.GetReferences(assetLink);
        var nifArchiveReferences = _nifArchiveAssetCaches.GetReferences(assetLink);

        return nifDirectoryReferences.Concat(nifArchiveReferences);
    }

    public int GetReferenceCount(IAssetLinkGetter assetLink) {
        var nifDirectoryReferences = _nifDirectoryAssetReferenceCaches.GetReferences(assetLink);
        var nifArchiveReferences = _nifArchiveAssetCaches.GetReferences(assetLink);
        var modReferences = _modAssetCaches.GetReferences(assetLink);

        return nifDirectoryReferences.Count() + nifArchiveReferences.Count() + modReferences.Count();
    }

    public IEnumerable<IAssetLinkGetter> GetAssetLinksFrom(FileSystemLink fileLink) {
        var cache = GetCacheFor(fileLink);
        if (cache is null) return [];

        return cache.FindLinksToReference(fileLink.DataRelativePath);
    }

    public Action<FileSystemLink> RegisterUpdate(FileSystemLink fileLink) {
        ValidateAsset(fileLink);

        var cache = GetCacheFor(fileLink);
        if (cache is null) return _ => {};

        var before = cache.FindLinksToReference(fileLink.DataRelativePath).ToArray();

        return newAsset => {
            var after = _nifFileAssetParser.ParseFile(newAsset).Select(result => result.AssetLink).ToArray();

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);

            // Remove the asset from its former references
            RemoveAssetReferences(cache, newAsset, removedReferences);

            // Add the asset to its new references
            AddAssetReferences(cache, newAsset, addedReferences);
        };
    }

    public void RegisterCreation(FileSystemLink fileLink) {
        var cache = GetCacheFor(fileLink);
        if (cache is null) {
            _assetCreations.Enqueue(fileLink);
            return;
        }

        ValidateAsset(fileLink);

        AddAssetReferences(cache, fileLink, _nifFileAssetParser.ParseFile(fileLink).Select(r => r.AssetLink));
    }

    public void RegisterDeletion(FileSystemLink fileLink) {
        var cache = GetCacheFor(fileLink);
        if (cache is null) {
            _assetDeletions.Enqueue(fileLink);
            return;
        }

        ValidateAsset(fileLink);

        RemoveAssetReferences(cache, fileLink, _nifFileAssetParser.ParseFile(fileLink).Select(r => r.AssetLink));
    }

    private static void ValidateAsset(FileSystemLink fileLink) {
        if (!fileLink.Exists()) throw new NotImplementedException("Updating virtual assets is not supported currently");
    }

    private void AddAssetReferences(
        AssetReferenceCache<FileSystemLink, DataRelativePath> cache,
        FileSystemLink fileLink,
        IEnumerable<IAssetLinkGetter> references) {
        foreach (var added in references) {
            cache.AddReference(added, fileLink.DataRelativePath);

            var change = new Change<DataRelativePath>(ListChangeReason.Add, fileLink.DataRelativePath);
            _nifDirectoryAssetReferenceManager.Update(added, change);
        }
    }

    private void RemoveAssetReferences(
        AssetReferenceCache<FileSystemLink, DataRelativePath> cache,
        FileSystemLink fileLink,
        IEnumerable<IAssetLinkGetter> references) {
        foreach (var removed in references) {
            cache.RemoveReference(removed, fileLink.DataRelativePath);

            var change = new Change<DataRelativePath>(ListChangeReason.Remove, fileLink.DataRelativePath);
            _nifDirectoryAssetReferenceManager.Update(removed, change);
        }
    }

    public Action<RecordModPair> RegisterUpdate(RecordModPair old) {
        var modCache = GetCacheFor(old.Mod.ModKey);
        if (modCache is null) return _ => {};

        // Collect the references before and after the update
        HashSet<IAssetLinkGetter> before;
        using (var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache()) {
            before = old.Record.EnumerateAllAssetLinks(assetLinkCache).ToHashSet();
        }

        return x => {
            HashSet<IAssetLinkGetter> after;
            using (var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache()) {
                after = x.Record.EnumerateAllAssetLinks(assetLinkCache).ToHashSet();
            }

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);
            var newRecordLink = x.Record.ToLinkFromRuntimeType();

            // Remove the record from its former references
            RemoveRecordReferences(modCache, newRecordLink, removedReferences);

            // Add the record to its new references
            AddRecordReferences(modCache, newRecordLink, addedReferences);
        };
    }

    public void RegisterCreation(RecordModPair pair) {
        var modCache = GetCacheFor(pair.Mod.ModKey);
        if (modCache is null) {
            _recordCreations.Enqueue(pair);
            return;
        }

        using var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        AddRecordReferences(modCache, pair.Record.ToLinkFromRuntimeType(), pair.Record.EnumerateAllAssetLinks(assetLinkCache));
    }

    public void RegisterDeletion(RecordModPair pair) {
        var modCache = GetCacheFor(pair.Mod.ModKey);
        if (modCache is null) {
            _recordDeletions.Enqueue(pair);
            return;
        }

        using var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        RemoveRecordReferences(modCache, pair.Record.ToLinkFromRuntimeType(), pair.Record.EnumerateAllAssetLinks(assetLinkCache));
    }

    private void AddRecordReferences(
        AssetReferenceCache<IModGetter, IFormLinkIdentifier> modReferenceCache,
        IFormLinkIdentifier newRecordLink,
        IEnumerable<IAssetLinkGetter> references) {
        foreach (var reference in references) {
            if (!modReferenceCache.AddReference(reference, newRecordLink)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Add, newRecordLink);
            _modAssetReferenceManager.Update(reference, change);
        }
    }

    private void RemoveRecordReferences(
        AssetReferenceCache<IModGetter, IFormLinkIdentifier> modReferenceCache,
        IFormLinkIdentifier newRecordLink,
        IEnumerable<IAssetLinkGetter> references) {
        foreach (var reference in references) {
            if (!modReferenceCache.RemoveReference(reference, newRecordLink)) continue;

            var change = new Change<IFormLinkIdentifier>(ListChangeReason.Remove, newRecordLink);
            _modAssetReferenceManager.Update(reference, change);
        }
    }

    private AssetReferenceCache<IModGetter, IFormLinkIdentifier>? GetCacheFor(ModKey modKey) {
        return _modAssetCaches.Find(cache => cache.Source.ModKey == modKey);
    }

    private AssetReferenceCache<FileSystemLink, DataRelativePath>? GetCacheFor(FileSystemLink link) {
        return _nifDirectoryAssetReferenceCaches.Find(x => x.Source.Contains(link));
    }

    private async Task UpdateLoadingProcess(Func<Task> action) {
        Interlocked.Increment(ref _loadingProcesses);
        await action();
        Interlocked.Decrement(ref _loadingProcesses);
        _isLoading.OnNext(_loadingProcesses > 0);
    }
}
