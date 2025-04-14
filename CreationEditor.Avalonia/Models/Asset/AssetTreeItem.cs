// using System.Collections.ObjectModel;
// using System.Diagnostics.CodeAnalysis;
// using System.Reactive.Linq;
// using CreationEditor.Avalonia.Comparer;
// using CreationEditor.Services.Asset;
// using CreationEditor.Services.DataSource;
// using CreationEditor.Services.Mutagen.References.Asset;
// using DynamicData;
// using Noggog;
// using ReactiveUI;
// namespace CreationEditor.Avalonia.Models.Asset;
//
// public sealed class AssetTreeItem(
//     FileSystemLink link,
//     IAsset asset,
//     IObservable<Func<IAsset, bool>> filterObservable)
//     : IAsset {
//
//     private readonly DisposableBucket _disposables = new();
//
//     public IAsset Asset { get; } = asset;
//     public FileSystemLink Link { get; } = link;
//     IEnumerable<IAsset> IAsset.Children => Asset.Children;
//     public bool IsDirectory => Asset.IsDirectory;
//     public bool HasChildren => Asset.HasChildren;
//     public bool IsVirtual => Asset.IsVirtual;
//     public IEnumerable<IReferencedAsset> GetReferencedAssets() => Asset.GetReferencedAssets();
//
//     [field: AllowNull, MaybeNull]
//     public ReadOnlyObservableCollection<AssetTreeItem> Children => field ??= LoadChildren();
//
//     private ReadOnlyObservableCollection<AssetTreeItem> LoadChildren() {
//         if (Asset is AssetDirectory assetDirectory) {
//             // Load up with initial items so the returning collection is already filled with something
//             // Otherwise the TreeDataGrid doesn't like it, see https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid/issues/132
//             // There might be more issues that this one though
//             // DISABLED FOR NOW - this causes duplicate items to be added to the collection
//             // var initialItem = assetDirectory.Children
//             //     .Select(Selector)
//             //     .Order(AssetComparers.PathComparer)
//             //     .Cast<AssetTreeItem>()
//             //     .ToList();
//
//             assetDirectory.LoadAssets();
//             assetDirectory.Assets
//                 .Connect()
//                 .SubscribeOn(RxApp.TaskpoolScheduler)
//                 .Filter(filterObservable)
//                 .Transform((Func<IAsset, AssetTreeItem>) Selector)
//                 .SortAndBind(out var collection, AssetComparers.PathComparer)
//                 .Subscribe();
//
//             return collection;
//
//             AssetTreeItem Selector(IAsset a) {
//                 var filePath = Link.FileSystem.Path.Combine(Link.DataRelativePath.Path, Link.Name);
//                 return new AssetTreeItem(Link with { DataRelativePath = filePath }, a, filterObservable);
//             }
//         }
//
//         return new ReadOnlyObservableCollection<AssetTreeItem>([]);
//     }
//
//     // public IObservable<bool> AnyOrphaned { get; }
//     public IObservable<bool> AnyOrphaned => _anyOrphaned ?? LoadAnyOrphaned();
//     private IObservable<bool> LoadAnyOrphaned() {
//         _anyOrphaned = Children
//             .SelectWhenCollectionChanges(() => Asset is AssetFile file
//                 ? file.ReferencedAsset.ReferenceCount.Select(x => x == 0)
//                 : Observable.Return(false));
//
//         return _anyOrphaned;
//     }
//     private IObservable<bool>? _anyOrphaned;
//
//     // AnyOrphaned = Children
//     //     .WhenCollectionChanges()
//     //     .Select(_ => Asset is Asset file
//     //         ? file.ReferencedAsset.ReferenceCount.Select(x => x == 0)
//     //         : Observable.Return(false))
//     //     .Switch();
//
//     public void Dispose() => _disposables.Dispose();
// }
