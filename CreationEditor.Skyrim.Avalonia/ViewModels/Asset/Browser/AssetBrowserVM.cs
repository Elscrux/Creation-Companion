using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.References;
using DynamicData.Binding;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Asset.Browser;

public sealed partial class AssetBrowserVM : ViewModel, IAssetBrowserVM {
    private readonly Func<object?, IReadOnlyList<IReferenceVM>, ReferenceBrowserVM> _referenceBrowserVMFactory;
    private readonly IMenuItemProvider _menuItemProvider;
    private readonly IAssetController _assetController;
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly MainWindow _mainWindow;
    private readonly IIgnoredDirectoriesProvider _ignoredDirectoriesProvider;
    private readonly ISearchFilter _searchFilter;
    private DataSourceDirectoryLink _rootDirectory = null!;

    private readonly Dictionary<string, IReadOnlyList<IDataSourceLink>> _filteredFileSystemChildrenCache = [];
    private readonly IReferenceService _referenceService;

    public IObservableCollection<IDataSourceLink> RootItems { get; } = new ObservableCollectionExtended<IDataSourceLink>();

    public IDataSourceService DataSourceService { get; }
    public ReadOnlyObservableCollection<IDataSource> DataSourceSelections { get; }
    public IAssetTypeService AssetTypeService { get; }
    public IAssetIconService AssetIconService { get; }

    [Reactive] public partial IDataSource DataSource { get; set; }
    [Reactive] public partial string SearchText { get; set; }
    [Reactive] public partial bool ShowEmptyDirectories { get; set; }
    [Reactive] public partial bool ShowIgnoredDirectories { get; set; }
    [Reactive] public partial bool ShowReferencedFiles { get; set; }
    [Reactive] public partial bool ShowOrphanedFiles { get; set; }
    [Reactive] public partial bool ShowOtherFiles { get; set; }
    [Reactive] public partial bool ShowTextures { get; set; }
    [Reactive] public partial bool ShowModels { get; set; }
    [Reactive] public partial bool ShowScriptSources { get; set; }
    [Reactive] public partial bool ShowScripts { get; set; }
    [Reactive] public partial bool ShowSounds { get; set; }
    [Reactive] public partial bool ShowMusic { get; set; }
    [Reactive] public partial bool ShowBehaviors { get; set; }
    [Reactive] public partial bool ShowBodyTextures { get; set; }
    [Reactive] public partial bool ShowDeformedModels { get; set; }
    [Reactive] public partial bool ShowInterfaces { get; set; }
    [Reactive] public partial bool ShowSeq { get; set; }
    [Reactive] public partial bool ShowTranslations { get; set; }

    [Reactive] public partial bool IsBusyLoadingAssets { get; set; }
    [Reactive] public partial bool IsBusyLoadingReferences { get; set; }

    public ReactiveCommand<DataRelativePath, Unit> MoveTo { get; }

    public ReactiveCommand<Unit, Unit> Undo { get; }
    public ReactiveCommand<Unit, Unit> Redo { get; }

    public ReactiveCommand<IReadOnlyList<IDataSourceLink?>, Unit> Open { get; }
    public ReactiveCommand<IReadOnlyList<IDataSourceLink?>, Unit> Delete { get; }
    public ReactiveCommand<IDataSourceLink, Unit> Rename { get; }
    public ReactiveCommand<IDataSourceLink, Unit> CopyPath { get; }
    public ReactiveCommand<DataSourceFileLink, Unit> OpenReferences { get; }
    public ReactiveCommand<DataSourceDirectoryLink, Unit> AddFolder { get; }

    public HierarchicalTreeDataGridSource<IDataSourceLink> AssetTreeSource { get; }

    public AssetBrowserVM(
        Func<object?, IReadOnlyList<IReferenceVM>, ReferenceBrowserVM> referenceBrowserVMFactory,
        IDataSourceWatcherProvider dataSourceWatcherProvider,
        IDataSourceService dataSourceService,
        IMenuItemProvider menuItemProvider,
        IReferenceService referenceService,
        IAssetController assetController,
        IAssetTypeService assetTypeService,
        IAssetIconService assetIconService,
        ILinkCacheProvider linkCacheProvider,
        MainWindow mainWindow,
        IIgnoredDirectoriesProvider ignoredDirectoriesProvider,
        ISearchFilter searchFilter) {
        DataSourceService = dataSourceService;
        SearchText = "";
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _menuItemProvider = menuItemProvider;
        _referenceService = referenceService;
        _assetController = assetController;
        AssetTypeService = assetTypeService;
        AssetIconService = assetIconService;
        _linkCacheProvider = linkCacheProvider;
        _mainWindow = mainWindow;
        _ignoredDirectoriesProvider = ignoredDirectoriesProvider;
        _searchFilter = searchFilter;

        ShowReferencedFiles = true;
        ShowOrphanedFiles = true;
        ShowOtherFiles = true;

        ShowTextures = true;
        ShowModels = true;
        ShowScriptSources = true;
        ShowScripts = true;
        ShowSounds = true;
        ShowMusic = true;
        ShowBehaviors = true;
        ShowBodyTextures = true;
        ShowDeformedModels = true;
        ShowInterfaces = true;
        ShowSeq = true;
        ShowTranslations = true;

        SearchText = string.Empty;
        IsBusyLoadingAssets = true;
        IsBusyLoadingReferences = true;

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
                        return new TextBlock { Text = referenceCount.ToString(), VerticalAlignment = VerticalAlignment.Center};
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
                        if (missingLinks.Length == 0) return null;

                        return new SymbolIcon {
                            Symbol = Symbol.ImportantFilled,
                            Foreground = StandardBrushes.InvalidBrush,
                            VerticalAlignment = VerticalAlignment.Center,
                            [ToolTip.TipProperty] = "Missing Links\n" + string.Join(",\n", missingLinks),
                        };
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

        Undo = ReactiveCommand.Create(() => _assetController.Undo());
        Redo = ReactiveCommand.Create(() => _assetController.Redo());

        Open = ReactiveCommand.Create<IReadOnlyList<IDataSourceLink?>>(OpenAssets);

        Delete = ReactiveCommand.CreateFromTask<IReadOnlyList<IDataSourceLink?>>(async assets => {
            var deleteAssets = assets.WhereNotNull().ToArray();
            Control? content = null;

            var referenceBrowserVM = GetReferenceBrowserVM(deleteAssets);
            if (referenceBrowserVM is not null) {
                var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
                content = new StackPanel {
                    Children = {
                        new TextBlock { Text = "Do you really want to proceed? There are still references to these assets." },
                        referenceBrowser,
                    },
                };
            }

            var deleteDialog = assets.Count == 1
                ? CreateAssetDialog(deleteAssets, $"Delete {deleteAssets[0].Name}", content)
                : CreateAssetDialog(deleteAssets, $"Delete {assets.Count}", content);

            if (await deleteDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
                foreach (var asset in deleteAssets) {
                    _assetController.Delete(asset);
                }
            }
        });

        Rename = ReactiveCommand.CreateFromTask<IDataSourceLink>(RenameAsset);

        CopyPath = ReactiveCommand.Create<IDataSourceLink>(CopyAssetPath);

        OpenReferences = ReactiveCommand.Create<DataSourceFileLink>(OpenAssetReferences,
            this.WhenAnyValue(x => x.AssetTreeSource.RowSelection!.SelectedItem)
                .Select(link => {
                    if (link is null) return false;

                    var assetLink = AssetTypeService.GetAssetLink(link.DataRelativePath);
                    if (assetLink is null) return false;

                    return _referenceService.GetReferenceCount(assetLink) > 0;
                })
        );

        AddFolder = ReactiveCommand.CreateFromTask<DataSourceDirectoryLink>(AddAssetFolder);
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

    private async Task AddAssetFolder(DataSourceDirectoryLink dir) {
        var textBox = new TextBox { Text = string.Empty, Watermark = "New folder" };
        var relativePath = dir.DataRelativePath.Path;
        var folderDialog = CreateAssetDialog($"Add new Folder at {relativePath}", textBox);

        if (await folderDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            var newFolder = dir.FileSystem.Path.Combine(dir.FullPath, textBox.Text);
            dir.FileSystem.Directory.CreateDirectory(newFolder);
        }
    }

    private void OpenAssetReferences(DataSourceFileLink asset) {
        var referenceBrowserVM = GetReferenceBrowserVM(asset);
        var relativePath = asset.DataRelativePath.Path;

        var referenceWindow = new ReferenceWindow(relativePath, referenceBrowserVM);
        referenceWindow.Show(_mainWindow);
    }

    private void CopyAssetPath(IDataSourceLink asset) {
        var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;

        clipboard?.SetTextAsync(asset.DataRelativePath.Path);
    }

    private async Task RenameAsset(IDataSourceLink asset) {
        if (asset is not DataSourceFileLink fileLink) return;

        var nameWithoutExtension = fileLink.NameWithoutExtension;
        var textBox = new TextBox { Text = nameWithoutExtension };
        var content = new StackPanel { Children = { textBox } };

        var referenceBrowserVM = GetReferenceBrowserVM(fileLink);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content.Children.Add(new TextBlock {
                Text = "Do you really want to proceed? These references will be modified to point to the new path.",
            });
            content.Children.Add(referenceBrowser);
        }

        var renameDialog = CreateAssetDialog($"Rename {fileLink.Name}", content);
        if (await renameDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            if (string.Equals(nameWithoutExtension, textBox.Text, DataRelativePath.PathComparison)) return;

            _assetController.Rename(fileLink, textBox.Text + fileLink.Extension);
        }
    }

    private ReferenceBrowserVM? GetReferenceBrowserVM(params IReadOnlyList<IDataSourceLink> assets) {
        var recordReferences = new HashSet<IFormLinkIdentifier>(FormLinkIdentifierEqualityComparer.Instance);
        var assetReferences = new HashSet<DataRelativePath>();

        // Gather all references to all assets
        foreach (var asset in assets) {
            if (!asset.Exists()) continue;

            if (asset is DataSourceDirectoryLink directoryLink) {
                foreach (var fileLink in directoryLink.EnumerateFileLinks(true)) {
                    AddReferences(fileLink);
                }
            } else {
                AddReferences(asset);
            }

            void AddReferences(IDataSourceLink fileLink) {
                var assetLink = AssetTypeService.GetAssetLink(fileLink.DataRelativePath);
                if (assetLink is null) return;

                assetReferences.AddRange(_referenceService.GetAssetReferences(assetLink));
                recordReferences.AddRange(_referenceService.GetRecordReferences(assetLink));
            }
        }

        var references = assetReferences
            .Select(path => new AssetReferenceVM(path, _linkCacheProvider, AssetTypeService, _referenceService))
            .Cast<IReferenceVM>()
            .Combine(recordReferences.Select(x => new RecordReferenceVM(x, _linkCacheProvider, _referenceService)))
            .ToArray();

        if (references.Length == 0) return null;

        return _referenceBrowserVMFactory(assets.Count == 1 ? assets[0] : assets, references);
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

        var firstDraggedAsset = draggedAssets.FirstOrDefault();
        if (firstDraggedAsset is null) return;

        var srcDirectory = firstDraggedAsset.ParentDirectory;
        if (srcDirectory is null) return;

        var relativeSrcDirectory = srcDirectory.DataRelativePath.Path;
        var relativeDstDirectory = dstDirectory.DataRelativePath.Path;

        // No need to move if the source and destination are the same
        if (string.Equals(relativeSrcDirectory, relativeDstDirectory, DataRelativePath.PathComparison)) return;

        var content = new StackPanel {
            Children = {
                draggedAssets.Length > 1
                    ? new TextBlock {
                        Inlines = [
                            new Run("Move "),
                            new Run(draggedAssets.Length.ToString()) {
                                Foreground = StandardBrushes.HighlightBrush,
                            },
                            new Run(" Items from "),
                            new Run(relativeSrcDirectory) {
                                Foreground = StandardBrushes.HighlightBrush,
                            },
                            new Run(" to "),
                            new Run(relativeDstDirectory) {
                                Foreground = StandardBrushes.HighlightBrush,
                            },
                        ],
                        FontSize = 14,
                        TextWrapping = TextWrapping.Wrap,
                    }
                    : new TextBlock {
                        Inlines = [
                            new Run("Move "),
                            new Run(firstDraggedAsset.Name) {
                                Foreground = StandardBrushes.HighlightBrush,
                            },
                            new Run(" to "),
                            new Run(relativeDstDirectory) {
                                Foreground = StandardBrushes.HighlightBrush,
                            },
                        ],
                        FontSize = 14,
                        TextWrapping = TextWrapping.Wrap,
                    },
            },
        };

        var referenceBrowserVM = GetReferenceBrowserVM(draggedAssets);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content.Children.Add(new TextBlock {
                Text = "Do you really want to proceed? These references will be modified to point to the new path.",
            });
            content.Children.Add(referenceBrowser);
        }

        // Show a confirmation dialog
        var moveDialog = CreateAssetDialog(draggedAssets, "Confirm", content);
        if (await moveDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            // Move all assets and remap their references
            foreach (var asset in draggedAssets) _assetController.Move(asset, dstDirectory);
        }
    }

    public IEnumerable<Control> GetContextMenuItems(IDataSourceLink asset) {
        var selectedItems = AssetTreeSource.RowSelection!.SelectedItems.ToArray();
        if (!selectedItems.Contains(asset)) {
            // We got an outdated selected items list - just use the current asset
            selectedItems = [asset];
        }

        List<Control> items = [
            _menuItemProvider.File(Open, selectedItems),
            _menuItemProvider.Rename(Rename, asset),
            _menuItemProvider.Delete(Delete, selectedItems),
            new Separator(),
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = "Copy Path",
                Command = CopyPath,
                CommandParameter = asset,
            },
        ];

        var assetLink = AssetTypeService.GetAssetLink(asset.DataRelativePath);
        if (assetLink is not null) {
            var assetReferences = _referenceService.GetAssetReferences(assetLink);
            var recordReferences = _referenceService.GetRecordReferences(assetLink);
            if (assetReferences.Any() || recordReferences.Any()) {
                var dataSourceLink = new DataSourceFileLink(asset.DataSource, asset.DataRelativePath);
                items.Add(_menuItemProvider.References(OpenReferences, dataSourceLink));
            }
        }

        if (asset is DataSourceDirectoryLink) {
            items.Add(new Separator());
            items.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.NewFolder },
                Header = "Add Folder",
                Command = AddFolder,
                CommandParameter = asset,
            });
        }

        return items;
    }

    private TaskDialog CreateAssetDialog(string header, Control? content = null) {
        var assetDialog = new TaskDialog {
            Header = header,
            Content = content,
            XamlRoot = _mainWindow,
            Buttons = {
                TaskDialogButton.OKButton,
                TaskDialogButton.CancelButton,
            },
            KeyBindings = {
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Enter),
                    Command = TaskDialogButton.OKButton.Command,
                },
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Escape),
                    Command = TaskDialogButton.CancelButton.Command,
                },
            },
        };

        content?.KeyBindings.AddRange(assetDialog.KeyBindings);

        if (IsBusyLoadingReferences) {
            assetDialog.IconSource = new SymbolIconSource { Symbol = Symbol.ReportHacked };
            assetDialog.SubHeader += "Warning: This change might break something - Not all references have been loaded yet.";
        }

        return assetDialog;
    }

    private TaskDialog CreateAssetDialog(IEnumerable<IDataSourceLink> footerAssets, string header, Control? content = null) {
        var dialog = CreateAssetDialog(header, content);

        var fileLinks = footerAssets.ToArray();
        if (fileLinks.Length > 1) {
            dialog.FooterVisibility = TaskDialogFooterVisibility.Auto;
            dialog.Footer = new ItemsControl {
                ItemsSource = fileLinks,
                ItemTemplate = new FuncDataTemplate<IDataSourceLink>((asset, _) => new TextBlock { Text = asset.DataRelativePath.Path }),
            };
        }

        return dialog;
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

    public void OpenAssets(IReadOnlyList<IDataSourceLink?> assets) {
        foreach (var asset in assets.WhereNotNull()) {
            _assetController.Open(asset);
        }
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
