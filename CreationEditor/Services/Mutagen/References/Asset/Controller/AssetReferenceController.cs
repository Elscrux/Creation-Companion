using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using CreationEditor.Services.Notification;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Mutagen.References.Asset.Controller;

public sealed class AssetReferenceController : IAssetReferenceController {
    private readonly CompositeDisposable _disposables = new();

    private readonly IFileSystem _fileSystem;
    private readonly IArchiveService _archiveService;
    private readonly INotificationService _notificationService;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly ModAssetQuery _modAssetQuery;
    private readonly NifDirectoryAssetQuery _nifDirectoryAssetQuery;
    private readonly NifArchiveAssetQuery _nifArchiveAssetQuery;

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

    private readonly List<AssetCache<IModGetter, IFormLinkGetter>> _modAssetCaches = new();
    private AssetCache<string, string> _nifDirectoryAssetCache = null!;
    private readonly List<AssetCache<string, string>> _nifArchiveAssetCaches = new();

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
        ModAssetQuery modAssetQuery,
        NifDirectoryAssetQuery nifDirectoryAssetQuery,
        NifArchiveAssetQuery nifArchiveAssetQuery) {
        _fileSystem = fileSystem;
        _archiveService = archiveService;
        _notificationService = notificationService;
        _editorEnvironment = editorEnvironment;
        _dataDirectoryProvider = dataDirectoryProvider;
        _modAssetQuery = modAssetQuery;
        _nifDirectoryAssetQuery = nifDirectoryAssetQuery;
        _nifArchiveAssetQuery = nifArchiveAssetQuery;

        Task.Run(() => UpdateLoadingProcess(InitNifDirectoryReferences));
        Task.Run(() => UpdateLoadingProcess(InitNifArchiveReferences));

        _editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => UpdateLoadingProcess(InitLoadOrderReferences))
            .DisposeWith(_disposables);

        void UpdateLoadingProcess(Action action) {
            Interlocked.Increment(ref _loadingProcesses);
            action();
            Interlocked.Decrement(ref _loadingProcesses);
            _isLoading.OnNext(_loadingProcesses > 0);
        }

        recordController.RecordChangedDiff.Subscribe(RegisterUpdate);
        recordController.RecordCreated.Subscribe(RegisterCreation);
        recordController.RecordDeleted.Subscribe(RegisterDeletion);
    }

    public void Dispose() => _disposables.Dispose();

    private void InitLoadOrderReferences() {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Asset References");

        var modKeys = _editorEnvironment.LinkCache.PriorityOrder.Select(mod => mod.ModKey).ToList();

        // Remove references from unloaded mods
        _modAssetCaches.RemoveWhere(c => !modKeys.Contains(c.Origin.ModKey));
        _modAssetReferenceManager.UnregisterWhere(asset => asset.ModKey != ModKey.Null && !modKeys.Contains(asset.ModKey));

        // Add references for new mods
        foreach (var mod in _editorEnvironment.LinkCache.PriorityOrder) {
            if (_modAssetCaches.Any(c => c.Origin.ModKey == mod.ModKey)) continue;

            var newModCache = new AssetCache<IModGetter, IFormLinkGetter>(_modAssetQuery, mod);

            _modAssetCaches.Add(newModCache);
        }

        // Update existing subscriptions
        _modAssetReferenceManager.Change(asset => {
            var references = _modAssetCaches.GetReferences(asset);

            return new Change<IFormLinkGetter>(ListChangeReason.AddRange, references);
        });

        linearNotifier.Stop();
    }

    private void InitNifDirectoryReferences() {
        using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Nif References");

        _nifDirectoryAssetCache = new AssetCache<string, string>(_nifDirectoryAssetQuery, _dataDirectoryProvider.Path);

        linearNotifier.Stop();

        // Change existing subscriptions
        _nifDirectoryAssetReferenceManager.Change(asset => {
            var references = _nifDirectoryAssetCache.GetReferences(asset);

            return new Change<string>(ListChangeReason.AddRange, references);
        });
    }

    private void InitNifArchiveReferences() {
        var extension = _archiveService.GetExtension();
        var dataDirectory = _dataDirectoryProvider.Path;

        var archiveWatcher = _fileSystem.FileSystemWatcher
            .New(dataDirectory, extension)
            .DisposeWith(_disposables);

        var archives = _fileSystem.Directory.EnumerateFiles(dataDirectory, $"*{extension}").ToArray();
        if (archives.Length > 0) {
            using var linearNotifier = new ChainedNotifier(_notificationService, "Loading Archive Nif References");

            var elapsedTime = Stopwatch.GetTimestamp();

            var assetCaches = archives.Select(a => new AssetCache<string, string>(_nifArchiveAssetQuery, a)).ToArray();
            Console.WriteLine($"NIF ASSET: {Stopwatch.GetElapsedTime(elapsedTime, Stopwatch.GetTimestamp())}");

            _nifArchiveAssetCaches.AddRange(assetCaches);

            // Update existing subscriptions with new references
            _nifArchiveAssetReferenceManager.Change(asset => {
                var references = assetCaches.GetReferences(asset);

                return new Change<string>(ListChangeReason.AddRange, references);
            });

            linearNotifier.Stop();
        }

        archiveWatcher.Events().Created
            .Subscribe(e => Add(e.FullPath))
            .DisposeWith(_disposables);

        archiveWatcher.Events().Deleted
            .Subscribe(e => Remove(e.FullPath))
            .DisposeWith(_disposables);

        archiveWatcher.Events().Renamed
            .Subscribe(e => {
                Remove(e.OldFullPath);
                Add(e.FullPath);
            })
            .DisposeWith(_disposables);

        void Add(string archive) {
            var relativePath = _fileSystem.Path.GetRelativePath(dataDirectory, archive);
            using var notifier = new ChainedNotifier(_notificationService, $"Loading Archive Nif References in {relativePath}");

            var newCache = new AssetCache<string, string>(_nifArchiveAssetQuery, archive);
            _nifArchiveAssetCaches.Add(newCache);

            // Change existing subscriptions
            _nifArchiveAssetReferenceManager.Change(asset => {
                var references = newCache.GetReferences(asset);

                return new Change<string>(ListChangeReason.AddRange, references);
            });

            notifier.Stop();
        }

        void Remove(string archive) {
            var cache = _nifArchiveAssetCaches.FirstOrDefault(c => string.Equals(c.Origin, archive, AssetCompare.PathComparison));
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
        var nifDirectoryReferences = _nifDirectoryAssetCache?.GetReferences(asset);
        var nifArchiveReferences = _nifArchiveAssetCaches?.GetReferences(asset);
        referencedAsset = new ReferencedAsset(asset, modReferences, nifDirectoryReferences, nifArchiveReferences);

        var pluginDisposable = _modAssetReferenceManager.Register(referencedAsset);
        var nifDisposable = _nifDirectoryAssetReferenceManager.Register(referencedAsset);
        var nifArchiveDisposable = _nifArchiveAssetReferenceManager.Register(referencedAsset);

        return new CompositeDisposable(pluginDisposable, nifDisposable, nifArchiveDisposable);
    }

    public Action<AssetFile> RegisterUpdate(AssetFile assetFile) {
        if (_nifDirectoryAssetCache is null) return _ => {};

        ValidateAsset(assetFile);

        var before = _nifDirectoryAssetCache.FindLinksToReference(assetFile.ReferencedAsset.AssetLink.DataRelativePath).ToArray();

        return newAsset => {
            var after = _nifDirectoryAssetQuery.ParseFile(newAsset.Path).Select(result => result.AssetLink).ToArray();

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
        if (_nifDirectoryAssetCache is null) {
            _assetCreations.Enqueue(file);
            return;
        }

        ValidateAsset(file);

        AddAssetReferences(file, _nifDirectoryAssetQuery.ParseFile(file.Path).Select(r => r.AssetLink));
    }

    public void RegisterDeletion(AssetFile file) {
        if (_nifDirectoryAssetCache is null) {
            _assetDeletions.Enqueue(file);
            return;
        }

        ValidateAsset(file);

        RemoveAssetReferences(file, _nifDirectoryAssetQuery.ParseFile(file.Path).Select(r => r.AssetLink));
    }

    private static void ValidateAsset(AssetFile assetFileFile) {
        if (assetFileFile.IsVirtual) throw new NotImplementedException("Updating virtual assets is not supported currently");
    }

    private void AddAssetReferences(AssetFile file, IEnumerable<IAssetLinkGetter> references) {
        foreach (var added in references) {
            var dataRelativePath = file.ReferencedAsset.AssetLink.DataRelativePath;
            _nifDirectoryAssetCache.AddReference(_nifDirectoryAssetCache.Origin, added, dataRelativePath);

            var change = new Change<string>(ListChangeReason.Add, dataRelativePath);
            _nifDirectoryAssetReferenceManager.Change(added, change);
        }
    }

    private void RemoveAssetReferences(AssetFile file, IEnumerable<IAssetLinkGetter> references) {
        foreach (var removed in references) {
            var dataRelativePath = file.ReferencedAsset.AssetLink.DataRelativePath;
            _nifDirectoryAssetCache.RemoveReference(_nifDirectoryAssetCache.Origin, removed, dataRelativePath);

            var change = new Change<string>(ListChangeReason.Remove, dataRelativePath);
            _nifDirectoryAssetReferenceManager.Change(removed, change);
        }
    }

    public Action<IMajorRecordGetter> RegisterUpdate(IMajorRecordGetter record) {
        var modCache = _modAssetCaches.FirstOrDefault(cache => cache.Origin.ModKey == _editorEnvironment.ActiveMod.ModKey);
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
        if (_nifDirectoryAssetCache is null) {
            _recordCreations.Enqueue(record);
            return;
        }

        var modCache = _modAssetCaches.FirstOrDefault(cache => cache.Origin.ModKey == _editorEnvironment.ActiveMod.ModKey);
        if (modCache is null) return;

        using var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        AddRecordReferences(modCache, record.ToLinkFromRuntimeType(), record.EnumerateAllAssetLinks(assetLinkCache));
    }

    public void RegisterDeletion(IMajorRecordGetter record) {
        if (_nifDirectoryAssetCache is null) {
            _recordDeletions.Enqueue(record);
            return;
        }

        var modCache = _modAssetCaches.FirstOrDefault(cache => cache.Origin.ModKey == _editorEnvironment.ActiveMod.ModKey);
        if (modCache is null) return;

        using var assetLinkCache = _editorEnvironment.LinkCache.CreateImmutableAssetLinkCache();
        RemoveRecordReferences(modCache, record.ToLinkFromRuntimeType(), record.EnumerateAllAssetLinks(assetLinkCache));
    }

    private void AddRecordReferences(AssetCache<IModGetter, IFormLinkGetter> modCache, IFormLinkGetter newRecordLink, IEnumerable<IAssetLinkGetter> references) {
        foreach (var reference in references) {
            if (!modCache.AddReference(modCache.Origin, reference, newRecordLink)) continue;

            var change = new Change<IFormLinkGetter>(ListChangeReason.Add, newRecordLink);
            _modAssetReferenceManager.Change(reference, change);
        }
    }

    private void RemoveRecordReferences(AssetCache<IModGetter, IFormLinkGetter> modCache, IFormLinkGetter newRecordLink, IEnumerable<IAssetLinkGetter> references) {
        foreach (var reference in references) {
            if (!modCache.RemoveReference(modCache.Origin, reference, newRecordLink)) continue;

            var change = new Change<IFormLinkGetter>(ListChangeReason.Remove, newRecordLink);
            _modAssetReferenceManager.Change(reference, change);
        }
    }
}
