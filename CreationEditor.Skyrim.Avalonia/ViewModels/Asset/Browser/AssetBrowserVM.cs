using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.References;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Asset.Browser;

public sealed partial class AssetBrowserVM : ViewModel, IAssetBrowserVM {
    private readonly IContextMenuProvider _contextMenuProvider;
    private readonly IIgnoredDirectoriesProvider _ignoredDirectoriesProvider;
    private readonly ISearchFilter _searchFilter;
    private DataSourceDirectoryLink _rootDirectory = null!;

    private readonly Dictionary<string, IReadOnlyList<IDataSourceLink>> _filteredFileSystemChildrenCache = [];
    private readonly IReferenceService _referenceService;

    public IObservableCollection<IDataSourceLink> RootItems { get; } = new ObservableCollectionExtended<IDataSourceLink>();

    public IDataSourceService DataSourceService { get; }
    public ReadOnlyObservableCollection<IDataSource> DataSourceSelections { get; }
    public IAssetContextActionsProvider AssetContextActionsProvider { get; }
    public IGenericContextActionsProvider GenericContextActionsProvider { get; }
    public IAssetTypeService AssetTypeService { get; }
    public IAssetIconService AssetIconService { get; }

    [Reactive] public partial IDataSource DataSource { get; set; }
    [Reactive] public partial string SearchText { get; set; } = string.Empty;
    [Reactive] public partial bool ShowEmptyDirectories { get; set; } = false;
    [Reactive] public partial bool ShowIgnoredDirectories { get; set; } = false;

    [Reactive] public partial bool ShowReferencedFiles { get; set; } = true;
    [Reactive] public partial bool ShowOrphanedFiles { get; set; } = true;
    [Reactive] public partial bool ShowOtherFiles { get; set; } = true;

    [Reactive] public partial bool ShowTextures { get; set; } = true;
    [Reactive] public partial bool ShowModels { get; set; } = true;
    [Reactive] public partial bool ShowScriptSources { get; set; } = true;
    [Reactive] public partial bool ShowScripts { get; set; } = true;
    [Reactive] public partial bool ShowSounds { get; set; } = true;
    [Reactive] public partial bool ShowMusic { get; set; } = true;
    [Reactive] public partial bool ShowBehaviors { get; set; } = true;
    [Reactive] public partial bool ShowBodyTextures { get; set; } = true;
    [Reactive] public partial bool ShowDeformedModels { get; set; } = true;
    [Reactive] public partial bool ShowInterfaces { get; set; } = true;
    [Reactive] public partial bool ShowSeq { get; set; } = true;
    [Reactive] public partial bool ShowTranslations { get; set; } = true;

    [Reactive] public partial bool IsBusyLoadingAssets { get; set; } = true;
    [Reactive] public partial bool IsBusyLoadingReferences { get; set; } = true;

    public ReactiveCommand<DataRelativePath, Unit> MoveTo { get; }

    public ReactiveCommand<Unit, Unit> Undo { get; }
    public ReactiveCommand<Unit, Unit> Redo { get; }

    public HierarchicalTreeDataGridSource<IDataSourceLink> AssetTreeSource { get; }

    public AssetBrowserVM(
        IContextMenuProvider contextMenuProvider,
        IDataSourceWatcherProvider dataSourceWatcherProvider,
        IDataSourceService dataSourceService,
        IReferenceService referenceService,
        IAssetController assetController,
        IAssetTypeService assetTypeService,
        IAssetIconService assetIconService,
        IModelService modelService,
        IIgnoredDirectoriesProvider ignoredDirectoriesProvider,
        ISearchFilter searchFilter,
        IAssetContextActionsProvider assetContextActionsProvider,
        IGenericContextActionsProvider genericContextActionsProvider) {
        DataSourceService = dataSourceService;
        _contextMenuProvider = contextMenuProvider;
        _referenceService = referenceService;
        AssetTypeService = assetTypeService;
        AssetIconService = assetIconService;
        _ignoredDirectoriesProvider = ignoredDirectoriesProvider;
        _searchFilter = searchFilter;
        AssetContextActionsProvider = assetContextActionsProvider;
        GenericContextActionsProvider = genericContextActionsProvider;

        DataSource = DataSourceService.ListedOrder.OfType<FileSystemDataSource>().First();
        DataSourceSelections = DataSourceService.DataSourcesChanged
            .Select(x => x.OfType<FileSystemDataSource>().Cast<IDataSource>())
            .ToObservableCollection(this);

        AssetTreeSource = new HierarchicalTreeDataGridSource<IDataSourceLink>(RootItems) {
            Columns = {
                new HierarchicalExpanderColumn<IDataSourceLink>(
                    new TemplateColumn<IDataSourceLink>(
                        "Name",
                        new FuncDataTemplate<IDataSourceLink>((asset, _) => {
                            if (asset is null) return null;

                            // Name
                            var textBlock = new TextBlock {
                                Text = asset.Name,
                                [ToolTip.TipProperty] = asset.DataRelativePath.Path,
                                VerticalAlignment = VerticalAlignment.Center,
                            };

                            if (asset.DataSource.IsReadOnly) {
                                textBlock.Foreground = Brushes.Gray;
                            }

                            // Icon
                            FAIconElement icon;
                            if (asset is DataSourceFileLink) {
                                var assetType = AssetTypeService.GetAssetType(asset.DataRelativePath.Path);
                                icon = assetType is null
                                    ? AssetIconService.GetIcon(asset.DataRelativePath.Extension)
                                    : AssetIconService.GetIcon(assetType);

                                if (assetType is not null
                                 && AssetTypeService.GetAssetLink(asset.DataRelativePath, assetType) is {} assetLink
                                 && _referenceService.GetReferencedAsset(assetLink, out var referencedAsset) is {} disposable) {
                                    icon[!FAIconElement.ForegroundProperty] = referencedAsset.ReferenceCount
                                        .Select(c => StandardBrushes.GetStatusBrush(c > 0))
                                        .ToBinding();
                                    icon.Unloaded += OnSymbolIconOnUnloaded;

                                    void OnSymbolIconOnUnloaded(object? sender, RoutedEventArgs e) {
                                        disposable.Dispose();
                                        icon.Unloaded -= OnSymbolIconOnUnloaded;
                                    }
                                } else {
                                    icon.Foreground = Brushes.Gray;
                                }
                            } else {
                                icon = AssetIconService.GetIcon(Symbol.Folder);
                                icon.Foreground = Brushes.Goldenrod;
                            }

                            return new StackPanel {
                                Orientation = Orientation.Horizontal,
                                Spacing = 5,
                                Children = {
                                    icon,
                                    textBlock,
                                },
                            };
                        }),
                        null,
                        new GridLength(),
                        new TemplateColumnOptions<IDataSourceLink> {
                            CanUserResizeColumn = true,
                            CanUserSortColumn = true,
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = asset => asset.Name,
                            CompareAscending = (x, y) => {
                                var checkNull = ObjectComparers.CheckNull(x, y);
                                return -(checkNull ?? x!.CompareToDirectoriesFirst(y));
                            },
                            CompareDescending = (x, y) => {
                                var checkNull = ObjectComparers.CheckNull(x, y);
                                return checkNull ?? x!.CompareToDirectoriesFirst(y);
                            },
                        }),
                    x => x is DataSourceDirectoryLink directoryLink ? GetFilteredFileSystemChildren(directoryLink) : [],
                    link => link is DataSourceDirectoryLink),
                new TemplateColumn<IDataSourceLink>(
                    "References",
                    new FuncDataTemplate<IDataSourceLink>((asset, _) => {
                        if (asset is null) return null;

                        var assetLink = AssetTypeService.GetAssetLink(asset.DataRelativePath);
                        if (assetLink is null) return null;

                        var referenceCount = _referenceService.GetReferenceCount(assetLink);
                        return new TextBlock { Text = referenceCount.ToString(), VerticalAlignment = VerticalAlignment.Center };
                    }),
                    null,
                    new GridLength(),
                    new TemplateColumnOptions<IDataSourceLink> {
                        CanUserResizeColumn = true,
                        CanUserSortColumn = true,
                        CompareAscending = (x, y) => {
                            var checkNull = ObjectComparers.CheckNull(x, y);
                            return checkNull ?? x!.CompareTo(y);
                        },
                        CompareDescending = (x, y) => {
                            var checkNull = ObjectComparers.CheckNull(x, y);
                            return -(checkNull ?? x!.CompareTo(y));
                        },
                    }),
                new TemplateColumn<IDataSourceLink>(
                    "Flags",
                    new FuncDataTemplate<IDataSourceLink>((asset, _) => {
                        if (asset is not DataSourceFileLink fileLink) return null;

                        var assetLink = AssetTypeService.GetAssetLink(fileLink.DataRelativePath);
                        if (assetLink is null) return null;

                        var missingLinks = GetMissingLinks(fileLink, assetLink).ToArray();
                        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

                        if (missingLinks.Length > 0) {
                            stackPanel.Children.Add(
                                new SymbolIcon {
                                    Symbol = Symbol.ImportantFilled,
                                    Foreground = StandardBrushes.InvalidBrush,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    [ToolTip.TipProperty] = "Missing Links\n" + string.Join(",\n", missingLinks),
                                }
                            );
                        }

                        if (assetLink.Type == SkyrimModelAssetType.Instance) {
                            if (modelService.HasCollision(fileLink)) {
                                // For models, we show the missing links in a tooltip
                                stackPanel.Children.Add(
                                    new FontIcon {
                                        Glyph = "⟟",
                                        VerticalAlignment = VerticalAlignment.Center,
                                        [ToolTip.TipProperty] = "Has Collision",
                                    }
                                );
                            }
                        }

                        return stackPanel;
                    })),
            },
        };

        _referenceService.IsLoading
            .ObserveOnGui()
            .Subscribe(loadingReferences => IsBusyLoadingReferences = loadingReferences)
            .DisposeWith(this);

        Observable.CombineLatest(
                this.WhenAnyValue(x => x.ShowTextures),
                this.WhenAnyValue(x => x.ShowModels),
                this.WhenAnyValue(x => x.ShowScriptSources),
                this.WhenAnyValue(x => x.ShowScripts),
                this.WhenAnyValue(x => x.ShowSounds),
                this.WhenAnyValue(x => x.ShowMusic),
                this.WhenAnyValue(x => x.ShowBehaviors),
                this.WhenAnyValue(x => x.ShowBodyTextures),
                this.WhenAnyValue(x => x.ShowDeformedModels),
                this.WhenAnyValue(x => x.ShowInterfaces),
                this.WhenAnyValue(x => x.ShowSeq),
                this.WhenAnyValue(x => x.ShowTranslations),
                this.WhenAnyValue(x => x.ShowEmptyDirectories),
                this.WhenAnyValue(x => x.ShowIgnoredDirectories),
                this.WhenAnyValue(x => x.ShowReferencedFiles),
                this.WhenAnyValue(x => x.ShowOrphanedFiles),
                this.WhenAnyValue(x => x.ShowOtherFiles))
            .CombineLatest(this.WhenAnyValue(x => x.SearchText))
            .ObserveOnGui()
            .ThrottleMedium()
            .Subscribe(Tree_UpdateAll)
            .DisposeWith(this);

        MoveTo = ReactiveCommand.Create<DataRelativePath>(MoveToPath);

        this.WhenAnyValue(x => x.DataSource)
            .Subscribe(dataSource => {
                var watcher = dataSourceWatcherProvider.GetWatcher(dataSource);
                watcher.Created
                    .ObserveOnGui()
                    .Where(x => Equals(x.DataSource, DataSource))
                    .Subscribe(Tree_AddLink)
                    .DisposeWith(this);
                watcher.Deleted
                    .ObserveOnGui()
                    .Where(x => Equals(x.DataSource, DataSource))
                    .Subscribe(Tree_RemoveLink)
                    .DisposeWith(this);
                watcher.Renamed
                    .ObserveOnGui()
                    .Where(x => Equals(x.Old.DataSource, DataSource) || Equals(x.New.DataSource, DataSource))
                    .Subscribe(Tree_RenameLink)
                    .DisposeWith(this);
                watcher.ChangedFile
                    .ObserveOnGui()
                    .Where(x => Equals(x.DataSource, DataSource))
                    .Subscribe(Tree_UpdateFileLink)
                    .DisposeWith(this);

                Dispatcher.UIThread.Post(() => {
                    IsBusyLoadingAssets = true;
                    _rootDirectory = (DataSource as FileSystemDataSource)?.GetRootLink()
                     ?? throw new InvalidOperationException("Only FileSystemDataSource is supported currently.");
                    RootItems.LoadOptimized(GetAllRootItems());
                    AssetTreeSource.RowSelection!.SingleSelect = false;
                    AssetTreeSource.SortBy(AssetTreeSource.Columns[0], ListSortDirection.Descending);
                    IsBusyLoadingAssets = false;
                    Tree_UpdateAll();
                });
            })
            .DisposeWith(this);

        Undo = ReactiveCommand.Create(assetController.Undo);
        Redo = ReactiveCommand.Create(assetController.Redo);
    }

    private IEnumerable<string> GetMissingLinks(DataSourceFileLink fileLink, IAssetLinkGetter assetLink) {
        if (assetLink.AssetTypeInstance != SkyrimModelAssetType.Instance) return [];

        // Missing Assets - todo move this to (nif/file) analyzer system which we can hook into here
        return _referenceService.GetAssetLinks(fileLink)
            .Where(path => !DataSourceService.FileExists(path.DataRelativePath))
            .Select(x => x.DataRelativePath.Path)
            .Concat(_referenceService.GetMissingRecordLinks(fileLink));
    }

    private HierarchicalRow<IDataSourceLink>? FindParentRow(IDataSourceLink fileLink) {
        var parentPath = fileLink.ParentDirectory?.DataRelativePath;
        return AssetTreeSource.Rows
            .OfType<HierarchicalRow<IDataSourceLink>>()
            .FirstOrDefault(row => row.Model.DataRelativePath == parentPath);
    }

    private IEnumerable<IDataSourceLink> GetAllRootItems() {
        return _rootDirectory
            .EnumerateAllLinks(false)
            .Where(dir => ShowIgnoredDirectories || !_ignoredDirectoriesProvider.IsIgnored(dir.DataRelativePath));
    }

    public async Task Drop(DataSourceDirectoryLink dstDirectory, DragInfo dragInfo) {
        var draggedAssets = dragInfo.Indexes
            .Select(indexPath => {
                var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                var row = dragInfo.Source.Rows[rowIndex];
                return row.Model;
            })
            .OfType<IDataSourceLink>()
            .ToArray();

        await AssetContextActionsProvider.MoveAssets(dstDirectory, draggedAssets);
    }

    public IEnumerable<Control> GetContextMenuItems(IDataSourceLink asset) {
        List<Control> items = [];

        var assetLink = AssetTypeService.GetAssetLink(asset.DataRelativePath);
        IReferencedAsset? referencedAsset;
        if (assetLink is not null) {
            using var _ = _referenceService.GetReferencedAsset(assetLink, out referencedAsset);
        } else {
            referencedAsset = null;
        }

        var menuItems = _contextMenuProvider.GetMenuItems(new SelectedListContext([], [new AssetContext(asset, referencedAsset)]));
        foreach (var control in menuItems.OfType<Control>()) {
            items.Add(control);
        }

        return items;
    }

    private void MoveToPath(DataRelativePath path) {
        var pathIndices = new IndexPath();
        IDataSourceLink? currentNode;
        var items = AssetTreeSource.Items.ToList();
        do {
            currentNode = items.Find(a => path.Path.StartsWith(a.DataRelativePath.Path, DataRelativePath.PathComparison));
            if (currentNode is null) break;

            pathIndices = pathIndices.Append(items.IndexOf(currentNode));
            if (currentNode is DataSourceDirectoryLink directoryLink) {
                items = directoryLink.EnumerateAllLinks(false).ToList();
            } else {
                break;
            }
        }
        while (!currentNode.DataRelativePath.Equals(path));

        var indexPathPart = new IndexPath();
        foreach (var pathIndex in pathIndices.SkipLast(1)) {
            indexPathPart = indexPathPart.Append(pathIndex);

            var rowIndex = AssetTreeSource.Rows.ModelIndexToRowIndex(indexPathPart);
            if (AssetTreeSource.Rows[rowIndex] is HierarchicalRow<IDataSourceLink> hierarchicalRow) {
                hierarchicalRow.IsExpanded = true;
            } else {
                break;
            }
        }

        AssetTreeSource.RowSelection!.Clear();
        AssetTreeSource.RowSelection.Select(pathIndices);
    }

    public IAssetLinkGetter? GetAssetLink(IDataSourceLink fileLink) {
        return AssetTypeService.GetAssetLink(fileLink.DataRelativePath);
    }

    private string SearchTextPattern() {
        // Surround with * and remove invalid chars
        return '*' + DataSource.FileSystem.Path.GetInvalidPathChars()
            .Aggregate(SearchText, (current, c) => current.Replace(c.ToString(), string.Empty))
            .Trim() + '*';
    }

    private IEnumerable<IDataSourceLink> GetFilteredFileSystemChildren(DataSourceDirectoryLink directory) {
        if (_filteredFileSystemChildrenCache.TryGetValue(directory.DataRelativePath.Path, out var cachedChildren)) return cachedChildren;

        var removeChars = directory.DataRelativePath.Path.Length + 1;
        var filteredLinks = directory
            .EnumerateFileLinks(SearchTextPattern(), true)
            .Where(FilterLink)
            .Select(link => {
                // Remove common start
                var span = link.DataRelativePath.Path.AsSpan();
                span = span[removeChars..];

                // Check for directory separators and remove everything after the first one (only top level files and directories count)
                var indexOfAny = span.IndexOfAny(directory.FileSystem.Path.DirectorySeparatorChar,
                    directory.FileSystem.Path.AltDirectorySeparatorChar);
                if (indexOfAny >= 0) {
                    span = span[..indexOfAny];
                }

                return span.ToString();
            })
            .DistinctBy(x => x, DataRelativePath.PathComparer)
            .ToHashSet();

        if (filteredLinks.Count == 0) return [];

        var filteredFileSystemChildren = GetAllFileSystemChildren(directory)
            .Where(link => {
                // Remove common start
                var span = link.DataRelativePath.Path.AsSpan();
                span = span[removeChars..];
                return filteredLinks.Contains(span.ToString());
            })
            .ToList();

        // Cache the filtered children for performance
        _filteredFileSystemChildrenCache[directory.DataRelativePath.Path] = filteredFileSystemChildren;

        return filteredFileSystemChildren;
    }

    private bool SearchAndFilterFile(DataSourceFileLink fileLink) {
        return _searchFilter.Filter(fileLink.DataRelativePath.Path, SearchTextPattern()) && FilterLink(fileLink);
    }

    private bool SearchAndFilterDirectory(DataSourceDirectoryLink directoryLink) {
        return (ShowIgnoredDirectories || !_ignoredDirectoriesProvider.IsIgnored(directoryLink.DataRelativePath))
         && (ShowEmptyDirectories || GetFilteredFileSystemChildren(directoryLink).Any());
    }

    private bool FilterLink(DataSourceFileLink fileLink) {
        var assetLink = AssetTypeService.GetAssetLink(fileLink.DataRelativePath);
        if (assetLink is not null) {
            switch (assetLink.Type) {
                case SkyrimBehaviorAssetType:
                    if (!ShowBehaviors) return false;

                    break;
                case SkyrimBodyTextureAssetType:
                    if (!ShowBodyTextures) return false;

                    break;
                case SkyrimDeformedModelAssetType:
                    if (!ShowDeformedModels) return false;

                    break;
                case SkyrimInterfaceAssetType:
                    if (!ShowInterfaces) return false;

                    break;
                case SkyrimModelAssetType:
                    if (!ShowModels) return false;

                    break;
                case SkyrimMusicAssetType:
                    if (!ShowMusic) return false;

                    break;
                case SkyrimScriptCompiledAssetType:
                    if (!ShowScripts) return false;

                    break;
                case SkyrimScriptSourceAssetType:
                    if (!ShowScriptSources) return false;

                    break;
                case SkyrimSeqAssetType:
                    if (!ShowSeq) return false;

                    break;
                case SkyrimSoundAssetType:
                    if (!ShowSounds) return false;

                    break;
                case SkyrimTextureAssetType:
                    if (!ShowTextures) return false;

                    break;
                case SkyrimTranslationAssetType:
                    if (!ShowTranslations) return false;

                    break;
            }

            var hasReferences = _referenceService.GetAssetReferences(assetLink).Any()
             || _referenceService.GetRecordReferences(assetLink).Any();

            if (hasReferences) {
                if (!ShowReferencedFiles) {
                    return false;
                }
            } else {
                if (!ShowOrphanedFiles) {
                    return false;
                }
            }
        } else {
            if (!ShowOtherFiles) {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<IDataSourceLink> GetAllFileSystemChildren(DataSourceDirectoryLink directory) {
        return directory.EnumerateDirectoryLinks(false)
            .Concat<IDataSourceLink>(directory.EnumerateFileLinks(false));
    }

    private static Type RowsType => typeof(SortableRowsBase<IDataSourceLink, HierarchicalRow<IDataSourceLink>>);
    private static MethodInfo OnItemsCollectionChangedMethod =>
        RowsType.GetMethod("OnItemsCollectionChanged", BindingFlags.Instance | BindingFlags.NonPublic)
     ?? throw new InvalidOperationException("Could not find OnItemsCollectionChanged method on childRows.");
    private static FieldInfo ItemsField => RowsType.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)
     ?? throw new InvalidOperationException("Could not find _items field on childRows.");

    private void Tree_AddLink(IDataSourceLink link) {
        switch (link) {
            case DataSourceFileLink fileLink:
                if (!FilterLink(fileLink)) return;

                break;
            case DataSourceDirectoryLink directoryLink:
                if (!SearchAndFilterDirectory(directoryLink)) return;

                break;
        }

        var parentRow = FindParentRow(link);
        if (parentRow?.Children is not SortableRowsBase<IDataSourceLink, HierarchicalRow<IDataSourceLink>> childRows) return;

        _filteredFileSystemChildrenCache.Remove(parentRow.Model.DataRelativePath.Path);
        _filteredFileSystemChildrenCache.Remove(link.DataRelativePath.Path);

        var value = ItemsField.GetValue(childRows);
        if (value is not TreeDataGridItemsSourceView view) return;

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, link, childRows.Count);
        view.Inner.Add(link);
        OnItemsCollectionChangedMethod.Invoke(childRows, [null, args]);
    }

    private void Tree_RemoveLink(IDataSourceLink fileLink) {
        var parentRow = FindParentRow(fileLink);
        if (parentRow?.Children is not SortableRowsBase<IDataSourceLink, HierarchicalRow<IDataSourceLink>> childRows) return;
        if (childRows.All(r => !Equals(r.Model, fileLink))) return;

        _filteredFileSystemChildrenCache.Remove(parentRow.Model.DataRelativePath.Path);
        _filteredFileSystemChildrenCache.Remove(fileLink.DataRelativePath.Path);

        var value = ItemsField.GetValue(childRows);
        if (value is not TreeDataGridItemsSourceView view) return;

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, fileLink, view.Inner.IndexOf(fileLink));
        view.Inner.Remove(fileLink);
        OnItemsCollectionChangedMethod.Invoke(childRows, [null, args]);
    }

    private void Tree_RenameLink(IUpdate<IDataSourceLink> rename) {
        Tree_RemoveLink(rename.Old);
        Tree_AddLink(rename.New);
    }

    private void Tree_UpdateFileLink(DataSourceFileLink fileLink) {
        if (!SearchAndFilterFile(fileLink)) {
            Tree_RemoveLink(fileLink);
            return;
        }

        var parentRow = FindParentRow(fileLink);
        if (parentRow?.Children is not SortableRowsBase<IDataSourceLink, HierarchicalRow<IDataSourceLink>> childRows) return;

        _filteredFileSystemChildrenCache.Remove(parentRow.Model.DataRelativePath.Path);

        var value = ItemsField.GetValue(childRows);
        if (value is not TreeDataGridItemsSourceView view) return;

        var index = view.Inner.IndexOf(fileLink);
        if (index < 0) return;

        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, fileLink, fileLink, index);
        view.Inner[index] = fileLink;
        OnItemsCollectionChangedMethod.Invoke(childRows, [null, args]);
    }

    private void Tree_UpdateAll() {
        _filteredFileSystemChildrenCache.Clear();

        var rows = AssetTreeSource.Rows.OfType<HierarchicalRow<IDataSourceLink>>().ToList();
        foreach (var row in rows) {
            // Handle removing of root items
            if (row.ModelIndexPath.Count == 1) {
                switch (row.Model) {
                    case DataSourceFileLink fileLink: {
                        if (!SearchAndFilterFile(fileLink)) {
                            RootItems.Remove(row.Model);
                        }
                        break;
                    }
                    case DataSourceDirectoryLink dirLink: {
                        // Specifically handle ignored directories here too to make sure they are removed when the setting changes
                        if (!SearchAndFilterDirectory(dirLink)) {
                            RootItems.Remove(row.Model);
                        }
                        break;
                    }
                }
            }

            if (row.Children is not SortableRowsBase<IDataSourceLink, HierarchicalRow<IDataSourceLink>> childRows) continue;

            var value = ItemsField.GetValue(childRows);
            if (value is not TreeDataGridItemsSourceView view) continue;

            if (row.Model is not DataSourceDirectoryLink directoryLink) continue;

            var dataSourceLinks = GetFilteredFileSystemChildren(directoryLink).ToArray();
            var oldLinks = view.Inner.OfType<IDataSourceLink>().Except(dataSourceLinks).ToHashSet();
            var newLinks = dataSourceLinks.Except(view.Inner.OfType<IDataSourceLink>()).ToArray();

            if (oldLinks.Count > 0) {
                if (oldLinks.Count > 100) {
                    // If there are too many links to remove, we can just reset the collection
                    if (view.Inner is IList<IDataSourceLink> list) {
                        list.Remove(oldLinks);
                    } else {
                        foreach (var oldLink in oldLinks) view.Inner.Remove(oldLink);
                    }
                    OnItemsCollectionChangedMethod.Invoke(childRows,
                        [null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)]);
                } else {
                    foreach (var oldLink in oldLinks) {
                        var indexOf = view.Inner.IndexOf(oldLink);
                        if (indexOf < 0) continue;

                        var removeArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldLink, indexOf);
                        view.Inner.Remove(oldLink);
                        OnItemsCollectionChangedMethod.Invoke(childRows, [null, removeArgs]);
                    }
                }
            }

            if (newLinks.Length > 0) {
                var addArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newLinks, childRows.Count);
                foreach (var link in newLinks) view.Inner.Add(link);
                OnItemsCollectionChangedMethod.Invoke(childRows, [null, addArgs]);
            }
        }

        // Handle re-adding of root items
        var rootRows = rows.Where(r => r.ModelIndexPath.Count == 1).ToArray();
        foreach (var rootItem in GetAllRootItems()) {
            if (rootRows.Exists(r => r.Model.Equals(rootItem))) continue;

            switch (rootItem) {
                case DataSourceFileLink fileLink: {
                    if (SearchAndFilterFile(fileLink)) {
                        RootItems.Add(rootItem);
                    }
                    break;
                }
                case DataSourceDirectoryLink directoryLink: {
                    if (SearchAndFilterDirectory(directoryLink)) {
                        RootItems.Add(rootItem);
                    }
                    break;
                }
            }
        }
    }
}
