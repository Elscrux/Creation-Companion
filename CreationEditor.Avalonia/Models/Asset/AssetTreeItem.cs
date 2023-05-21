using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Models.Asset;

public sealed record AssetTreeItem : IAsset {
    private readonly IFileSystem _fileSystem;
    private readonly IObservable<Func<IAsset, bool>> _filter;
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public IAsset Asset { get; }
    public string Path { get; }
    IEnumerable<IAsset> IAsset.Children => Asset.Children;
    public bool IsDirectory => Asset.IsDirectory;
    public bool HasChildren => Asset.HasChildren;
    public bool IsVirtual => Asset.IsVirtual;
    public IEnumerable<IReferencedAsset> GetReferencedAssets() => Asset.GetReferencedAssets();

    public ReadOnlyObservableCollection<AssetTreeItem> Children { get; }
    // public ReadOnlyObservableCollection<AssetTreeItem> Children => _children ?? LoadChildren();
    // private ReadOnlyObservableCollection<AssetTreeItem>? _children;
    // private ReadOnlyObservableCollection<AssetTreeItem> LoadChildren() {
    //     return _children = Asset is AssetDirectory assetDirectory
    //         ? assetDirectory.Assets
    //             .Connect()
    //             .SubscribeOn(RxApp.TaskpoolScheduler)
    //             .Filter(_filter)
    //             .Sort(AssetComparers.PathComparer)
    //             .ToObservableCollection((a, _) => new AssetTreeItem(_fileSystem.Path.Combine(Path, _fileSystem.Path.GetFileName(a.Path)), a, _fileSystem, _filter), _disposables)
    //         : new ReadOnlyObservableCollection<AssetTreeItem>(new ObservableCollectionExtended<AssetTreeItem>());
    // }

    // public IObservable<bool> AnyOrphaned { get; }
    public IObservable<bool> AnyOrphaned => _anyOrphaned ?? LoadAnyOrphaned();
    private IObservable<bool> LoadAnyOrphaned() {
        _anyOrphaned = Children
            .ObserveCollectionChanges().Unit()
            .StartWith(Unit.Default)
            .Select(_ => Asset is AssetFile file
                ? file.ReferencedAsset.ReferenceCount.Select(x => x == 0)
                : Observable.Return(false))
            .Switch();

        return _anyOrphaned;
    }
    private IObservable<bool>? _anyOrphaned;

    public AssetTreeItem(string path, IAsset asset, IFileSystem fileSystem, IObservable<Func<IAsset, bool>> filter, bool forceLoad = false) {
        _fileSystem = fileSystem;
        _filter = filter;
        Path = path;
        Asset = asset;

        // if (forceLoad) LoadChildren();

        Children = Asset is AssetDirectory assetDirectory
            ? assetDirectory.Assets
                .Connect()
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Filter(filter)
                .Sort(AssetComparers.PathComparer)
                .ToObservableCollection((a, _) => new AssetTreeItem(fileSystem.Path.Combine(path, fileSystem.Path.GetFileName(a.Path)), a, fileSystem, filter), _disposables)
            : new ReadOnlyObservableCollection<AssetTreeItem>(new ObservableCollectionExtended<AssetTreeItem>());

        // AnyOrphaned = Children
        //     .ObserveCollectionChanges().Unit()
        //     .StartWith(Unit.Default)
        //     .Select(_ => Asset is Asset file
        //         ? file.ReferencedAsset.ReferenceCount.Select(x => x == 0)
        //         : Observable.Return(false))
        //     .Switch();
    }

    public void Dispose() {
        _disposables.Dispose();
        // todo decrease asset ref count
    }
}
