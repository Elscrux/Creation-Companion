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

public sealed class AssetTreeItem : IAsset {
    private readonly IFileSystem _fileSystem;
    private readonly IObservable<Func<IAsset, bool>> _filterObservable;
    private Func<IAsset, bool> _filter = null!;
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    public IAsset Asset { get; }
    public string Path { get; }
    IEnumerable<IAsset> IAsset.Children => Asset.Children;
    public bool IsDirectory => Asset.IsDirectory;
    public bool HasChildren => Asset.HasChildren;
    public bool IsVirtual => Asset.IsVirtual;
    public IEnumerable<IReferencedAsset> GetReferencedAssets() => Asset.GetReferencedAssets();

    public ReadOnlyObservableCollection<AssetTreeItem> Children => _children ??= LoadChildren();
    private ReadOnlyObservableCollection<AssetTreeItem>? _children;

    private ReadOnlyObservableCollection<AssetTreeItem> LoadChildren() {
        if (Asset is AssetDirectory assetDirectory) {
            AssetTreeItem Selector(IAsset a) => new(_fileSystem.Path.Combine(Path, _fileSystem.Path.GetFileName(a.Path)), a, _fileSystem, _filterObservable);

            // Load up with initial items so the returning collection is already filled with something
            // Otherwise the TreeDataGrid doesn't like it, see https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid/issues/132
            // There might be more issues that this one though
            var filtered = _filter == null ? assetDirectory.Children : assetDirectory.Children.Where(_filter);
            var initialItem = filtered
                .Select(Selector)
                .Order(AssetComparers.PathComparer)
                .Cast<AssetTreeItem>();

            return assetDirectory.Assets
                .Connect()
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .Filter(_filterObservable)
                .Transform((Func<IAsset, AssetTreeItem>) Selector)
                .Sort(AssetComparers.PathComparer)
                .ToObservableCollectionSync(initialItem, _disposables);
        }

        return new ReadOnlyObservableCollection<AssetTreeItem>(new ObservableCollectionExtended<AssetTreeItem>());
    }

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

    public AssetTreeItem(string path, IAsset asset, IFileSystem fileSystem, IObservable<Func<IAsset, bool>> filterObservable) {
        _fileSystem = fileSystem;
        _filterObservable = filterObservable;
        Path = path;
        Asset = asset;

        filterObservable
            .Subscribe(f => _filter = f)
            .DisposeWith(_disposables);

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
