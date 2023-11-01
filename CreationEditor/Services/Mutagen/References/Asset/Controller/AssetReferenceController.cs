using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Parser;
using CreationEditor.Services.Notification;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Controller;

public sealed class AssetReferenceController : IAssetReferenceController, ILifecycleTask {
    private readonly CompositeDisposable _disposables = new();

    private readonly IFileSystem _fileSystem;
    private readonly IArchiveService _archiveService;
    private readonly INotificationService _notificationService;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly NifFileAssetParser _nifFileAssetParser;
    private readonly IAssetReferenceCacheFactory _assetReferenceCacheFactory;

    private readonly ConcurrentQueue<IMajorRecordGetter> _recordCreations = new();
    private readonly ConcurrentQueue<IMajorRecordGetter> _recordDeletions = new();

    private readonly ConcurrentQueue<AssetFile> _assetCreations = new();
    private readonly ConcurrentQueue<AssetFile> _assetDeletions = new();

    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IReferencedAsset, IFormLinkGetter> _modAssetReferenceManager
        = new((asset, change) => asset.RecordReferences.Apply(change),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IReferencedAsset, string> _nifDirectoryAssetReferenceManager
        = new((asset, change) => asset.NifDirectoryReferences.Apply(change),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

    private readonly ReferenceSubscriptionManager<IAssetLinkGetter, IReferencedAsset, string> _nifArchiveAssetReferenceManager
        = new((asset, change) => asset.NifArchiveReferences.Apply(change),
            asset => asset.AssetLink,
            AssetLinkEqualityComparer.Instance);

    private readonly List<AssetReferenceCache<IModGetter, IFormLinkGetter>> _modAssetCaches = new();
    private AssetReferenceCache<string, string> _nifDirectoryAssetReferenceCache = null!;
    private readonly List<AssetReferenceCache<string, string>> _nifArchiveAssetCaches = new();

    private int _loadingProcesses;
    private readonly BehaviorSubject<bool> _isLoading = new(true);
    public IObservable<bool> IsLoading => _isLoading;

    public AssetReferenceController(
        IFileSystem fileSystem,
        IArchiveService archiveService,
        INotificationService notificationService,
        IEditorEnvironment editorEnvironment,
        IDataDirectoryProvider dataDirectoryProvider,
        IRecordController recordController,
        NifFileAssetParser nifFileAssetParser,
        IAssetReferenceCacheFactory assetReferenceCacheFactory) {
        _fileSystem = fileSystem;
        _archiveService = archiveService;
        _notificationService = notificationService;
        _editorEnvironment = editorEnvironment;
        _dataDirectoryProvider = dataDirectoryProvider;
        _nifFileAssetParser = nifFileAssetParser;
        _assetReferenceCacheFactory = assetReferenceCacheFactory;

        _editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => UpdateLoadingProcess(InitLoadOrderReferences))
            .DisposeWith(_disposables);

        recordController.RecordChangedDiff.Subscribe(RegisterUpdate).DisposeWith(_disposables);
        recordController.RecordCreated.Subscribe(RegisterCreation).DisposeWith(_disposables);
        recordController.RecordDeleted.Subscribe(RegisterDeletion).DisposeWith(_disposables);
    }

    public void PreStartup() {}
    public void PostStartupAsync(CancellationToken token) {
        Task.Run(() => UpdateLoadingProcess(InitNifDirectoryReferences), token);
        Task.Run(() => UpdateLoadingProcess(InitNifArchiveReferences), token);
    }
    public void OnExit() {}
    public void Dispose() => _disposables.Dispose();

    private async Task InitLoadOrderReferences() {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Asset References");

        var modKeys = _editorEnvironment.LinkCache.PriorityOrder.Select(mod => mod.ModKey).ToList();

        // Remove references from unloaded mods
        _modAssetCaches.RemoveWhere(c => !modKeys.Contains(c.Source.ModKey));
        _modAssetReferenceManager.UnregisterWhere(asset => asset.ModKey != ModKey.Null && !modKeys.Contains(asset.ModKey));

        // Add references for new mods
        var tasks = _editorEnvironment.LinkCache.PriorityOrder
            .Where(mod => _modAssetCaches.All(c => c.Source.ModKey != mod.ModKey))
            .Select(mod => Task.Run(async () => await _assetReferenceCacheFactory.GetModCache(mod)));

        foreach (var assetCache in await Task.WhenAll(tasks)) {
            _modAssetCaches.Add(assetCache);
        }

        // Update existing subscriptions
        _modAssetReferenceManager.Change(asset => {
            var references = _modAssetCaches.GetReferences(asset);

            return new Change<IFormLinkGetter>(ListChangeReason.AddRange, references);
        });

        linearNotifier.Stop();
    }

    private async Task InitNifDirectoryReferences() {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Nif References");

        _nifDirectoryAssetReferenceCache = await _assetReferenceCacheFactory.GetNifCache(_dataDirectoryProvider.Path);

        linearNotifier.Stop();

        // Change existing subscriptions
        _nifDirectoryAssetReferenceManager.Change(asset => {
            var references = _nifDirectoryAssetReferenceCache.GetReferences(asset);

            return new Change<string>(ListChangeReason.AddRange, references);
        });
    }

    private async Task InitNifArchiveReferences() {
        var extension = _archiveService.GetExtension();
        var dataDirectory = _dataDirectoryProvider.Path;

        var archives = _fileSystem.Directory.EnumerateFiles(dataDirectory, $"*{extension}").ToArray();
        if (archives.Length > 0) {
            using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Archive Nif References");

            var elapsedTime = Stopwatch.GetTimestamp();

            var assetCaches = await Task.WhenAll(archives
                .Select(mod => Task.Run(async () => await _assetReferenceCacheFactory.GetNifArchiveCache(mod))));
            Console.WriteLine($"NIF ASSET: {Stopwatch.GetElapsedTime(elapsedTime, Stopwatch.GetTimestamp())}");

            _nifArchiveAssetCaches.AddRange(assetCaches);

            // Update existing subscriptions with new references
            _nifArchiveAssetReferenceManager.Change(asset => {
                var references = assetCaches.GetReferences(asset);

                return new Change<string>(ListChangeReason.AddRange, references);
            });

            linearNotifier.Stop();
        }

        _archiveService.ArchiveCreated
            .Subscribe(Add)
            .DisposeWith(_disposables);

        _archiveService.ArchiveDeleted
            .Subscribe(Remove)
            .DisposeWith(_disposables);

        _archiveService.ArchiveRenamed
            .Subscribe(async e => {
                Remove(e.OldName);
                await Add(e.NewName);
            })
            .DisposeWith(_disposables);
        return;

        async Task Add(string archive) {
            var relativePath = _fileSystem.Path.GetRelativePath(dataDirectory, archive);
            using var notifier = new ChainedNotifier(_notificationService, $"Loading Archive Nif References in {relativePath}");

            var newCache = await _assetReferenceCacheFactory.GetNifArchiveCache(archive);
            _nifArchiveAssetCaches.Add(newCache);

            // Change existing subscriptions
            _nifArchiveAssetReferenceManager.Change(asset => {
                var references = newCache.GetReferences(asset);

                return new Change<string>(ListChangeReason.AddRange, references);
            });

            notifier.Stop();
        }

        void Remove(string archive) {
            var cache = _nifArchiveAssetCaches.FirstOrDefault(c => string.Equals(c.Source, archive, AssetCompare.PathComparison));
            if (cache is null) return;

            // Change existing subscriptions
            _nifArchiveAssetReferenceManager.Change(asset => {
                var references = cache.GetReferences(asset);

                return new Change<string>(ListChangeReason.RemoveRange, references);
            });
        }
    }

    public IDisposable GetReferencedAsset(IAssetLinkGetter asset, out IReferencedAsset referencedAsset) {
        var modReferences = _modAssetCaches?.GetReferences(asset);
        var nifDirectoryReferences = _nifDirectoryAssetReferenceCache?.GetReferences(asset);
        var nifArchiveReferences = _nifArchiveAssetCaches?.GetReferences(asset);
        referencedAsset = new ReferencedAsset(asset, modReferences, nifDirectoryReferences, nifArchiveReferences);

        var pluginDisposable = _modAssetReferenceManager.Register(referencedAsset);
        var nifDisposable = _nifDirectoryAssetReferenceManager.Register(referencedAsset);
        var nifArchiveDisposable = _nifArchiveAssetReferenceManager.Register(referencedAsset);

        return new CompositeDisposable(pluginDisposable, nifDisposable, nifArchiveDisposable);
    }

    public Action<AssetFile> RegisterUpdate(AssetFile assetFile) {
        if (_nifDirectoryAssetReferenceCache is null) return _ => {};

        ValidateAsset(assetFile);

        var before = _nifDirectoryAssetReferenceCache.FindLinksToReference(assetFile.ReferencedAsset.AssetLink.DataRelativePath).ToArray();

        return newAsset => {
            var after = _nifFileAssetParser.ParseFile(newAsset.Path).Select(result => result.AssetLink).ToArray();

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);

            // Remove the asset from its former references
            RemoveAssetReferences(newAsset, removedReferences);

            // Add the asset to its new references
            AddAssetReferences(newAsset, addedReferences);
        };
    }

    public void RegisterCreation(AssetFile file) {
        if (_nifDirectoryAssetReferenceCache is null) {
            _assetCreations.Enqueue(file);
            return;
        }

        ValidateAsset(file);

        AddAssetReferences(file, _nifFileAssetParser.ParseFile(file.Path).Select(r => r.AssetLink));
    }

    public void RegisterDeletion(AssetFile file) {
        if (_nifDirectoryAssetReferenceCache is null) {
            _assetDeletions.Enqueue(file);
            return;
        }

        ValidateAsset(file);

        RemoveAssetReferences(file, _nifFileAssetParser.ParseFile(file.Path).Select(r => r.AssetLink));
    }

    private static void ValidateAsset(AssetFile assetFileFile) {
        if (assetFileFile.IsVirtual) throw new NotImplementedException("Updating virtual assets is not supported currently");
    }

    private void AddAssetReferences(AssetFile file, IEnumerable<IAssetLinkGetter> references) {
        foreach (var added in references) {
            var dataRelativePath = file.ReferencedAsset.AssetLink.DataRelativePath;
            _nifDirectoryAssetReferenceCache.AddReference(added, dataRelativePath);

            var change = new Change<string>(ListChangeReason.Add, dataRelativePath);
            _nifDirectoryAssetReferenceManager.Change(added, change);
        }
    }

    private void RemoveAssetReferences(AssetFile file, IEnumerable<IAssetLinkGetter> references) {
        foreach (var removed in references) {
            var dataRelativePath = file.ReferencedAsset.AssetLink.DataRelativePath;
            _nifDirectoryAssetReferenceCache.RemoveReference(removed, dataRelativePath);

            var change = new Change<string>(ListChangeReason.Remove, dataRelativePath);
            _nifDirectoryAssetReferenceManager.Change(removed, change);
        }
    }

    public Action<IMajorRecordGetter> RegisterUpdate(IMajorRecordGetter record) {
        var modCache = _modAssetCaches.FirstOrDefault(cache => cache.Source.ModKey == _editorEnvironment.ActiveMod.ModKey);
        if (modCache is null) return _ => {};

        // Collect the references before and after the update
        HashSet<IAssetLinkGetter> before;
        using (var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache()) {
            before = record.EnumerateAllAssetLinks(assetLinkCache).ToHashSet();
        }

        return newRecord => {
            HashSet<IAssetLinkGetter> after;
            using (var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache()) {
                after = newRecord.EnumerateAllAssetLinks(assetLinkCache).ToHashSet();
            }

            // Calculate the diff
            var removedReferences = before.Except(after);
            var addedReferences = after.Except(before);
            var newRecordLink = newRecord.ToLinkFromRuntimeType();

            // Remove the record from its former references
            RemoveRecordReferences(modCache, newRecordLink, removedReferences);

            // Add the record to its new references
            AddRecordReferences(modCache, newRecordLink, addedReferences);
        };
    }

    public void RegisterCreation(IMajorRecordGetter record) {
        if (_nifDirectoryAssetReferenceCache is null) {
            _recordCreations.Enqueue(record);
            return;
        }

        var modCache = _modAssetCaches.FirstOrDefault(cache => cache.Source.ModKey == _editorEnvironment.ActiveMod.ModKey);
        if (modCache is null) return;

        using var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        AddRecordReferences(modCache, record.ToLinkFromRuntimeType(), record.EnumerateAllAssetLinks(assetLinkCache));
    }

    public void RegisterDeletion(IMajorRecordGetter record) {
        if (_nifDirectoryAssetReferenceCache is null) {
            _recordDeletions.Enqueue(record);
            return;
        }

        var modCache = _modAssetCaches.FirstOrDefault(cache => cache.Source.ModKey == _editorEnvironment.ActiveMod.ModKey);
        if (modCache is null) return;

        using var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        RemoveRecordReferences(modCache, record.ToLinkFromRuntimeType(), record.EnumerateAllAssetLinks(assetLinkCache));
    }

    private void AddRecordReferences(AssetReferenceCache<IModGetter, IFormLinkGetter> modReferenceCache, IFormLinkGetter newRecordLink, IEnumerable<IAssetLinkGetter> references) {
        foreach (var reference in references) {
            if (!modReferenceCache.AddReference(reference, newRecordLink)) continue;

            var change = new Change<IFormLinkGetter>(ListChangeReason.Add, newRecordLink);
            _modAssetReferenceManager.Change(reference, change);
        }
    }

    private void RemoveRecordReferences(AssetReferenceCache<IModGetter, IFormLinkGetter> modReferenceCache, IFormLinkGetter newRecordLink, IEnumerable<IAssetLinkGetter> references) {
        foreach (var reference in references) {
            if (!modReferenceCache.RemoveReference(reference, newRecordLink)) continue;

            var change = new Change<IFormLinkGetter>(ListChangeReason.Remove, newRecordLink);
            _modAssetReferenceManager.Change(reference, change);
        }
    }

    private async Task UpdateLoadingProcess(Func<Task> action) {
        Interlocked.Increment(ref _loadingProcesses);
        await action();
        Interlocked.Decrement(ref _loadingProcesses);
        _isLoading.OnNext(_loadingProcesses > 0);
    }
}
