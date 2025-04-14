using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Asset.Browser;

public sealed partial class AssetBrowserVM : ViewModel, IAssetBrowserVM {
    private readonly Func<object?, IReferenceVM[], ReferenceBrowserVM> _referenceBrowserVMFactory;
    private readonly IArchiveService _archiveService;
    private readonly IMenuItemProvider _menuItemProvider;
    private readonly IAssetReferenceController _assetReferenceController = null!;
    private readonly IAssetController _assetController = null!;
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IRecordReferenceController _recordReferenceController = null!;
    private readonly IAssetTypeService _assetTypeService;
    private readonly MainWindow _mainWindow;
    private readonly ISearchFilter _searchFilter;
    private readonly FileSystemDataSource _dataSource;
    private readonly IDataSourceWatcher _dataSourceWatcher;

    [Reactive] public partial string SearchText { get; set; }
    [Reactive] public partial bool ShowBsaAssets { get; set; }
    [Reactive] public partial bool ShowEmptyDirectories { get; set; }
    [Reactive] public partial bool ShowOnlyOrphaned { get; set; }

    [Reactive] public partial bool IsBusyLoadingAssets { get; set; }
    [Reactive] public partial bool IsBusyLoadingReferences { get; set; }

    public ReactiveCommand<DataRelativePath, Unit> MoveTo { get; }

    public ReactiveCommand<Unit, Unit> Undo { get; }
    public ReactiveCommand<Unit, Unit> Redo { get; }

    public ReactiveCommand<IReadOnlyList<FileSystemLink?>, Unit> Open { get; }
    public ReactiveCommand<IReadOnlyList<FileSystemLink?>, Unit> Delete { get; }
    public ReactiveCommand<FileSystemLink, Unit> Rename { get; }
    public ReactiveCommand<FileSystemLink, Unit> CopyPath { get; }
    public ReactiveCommand<FileSystemLink, Unit> OpenReferences { get; }
    public ReactiveCommand<FileSystemLink, Unit> AddFolder { get; }
    public ReactiveCommand<FileSystemLink, Unit> OpenAssetBrowser { get; }

    public HierarchicalTreeDataGridSource<FileSystemLink> AssetTreeSource { get; }

    public AssetBrowserVM(
        Func<object?, IReferenceVM[], ReferenceBrowserVM> referenceBrowserVMFactory,
        IDataSourceWatcherProvider dataSourceWatcherProvider,
        IDataSourceService dataSourceService,
        ModelAssetQuery modelAssetQuery,
        IArchiveService archiveService,
        IMenuItemProvider menuItemProvider,
        IAssetReferenceController assetReferenceController,
        IAssetController assetController,
        IAssetTypeService assetTypeService,
        IAssetSymbolService assetSymbolService,
        ILinkCacheProvider linkCacheProvider,
        IRecordReferenceController recordReferenceController,
        IDockFactory dockFactory,
        MainWindow mainWindow,
        ISearchFilter searchFilter
    ) {
        SearchText = "";
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _archiveService = archiveService;
        _menuItemProvider = menuItemProvider;
        _assetReferenceController = assetReferenceController;
        _assetController = assetController;
        _assetTypeService = assetTypeService;
        _linkCacheProvider = linkCacheProvider;
        _recordReferenceController = recordReferenceController;
        _mainWindow = mainWindow;
        _searchFilter = searchFilter;
        _dataSource = dataSourceService.PriorityOrder.OfType<FileSystemDataSource>().First();
        _dataSourceWatcher = dataSourceWatcherProvider.GetWatcher(_dataSource);

        SearchText = string.Empty;
        IsBusyLoadingAssets = true;
        IsBusyLoadingReferences = true;

        // _assetReferenceController.IsLoading
        //     .ObserveOnGui()
        //     .Subscribe(loadingReferences => IsBusyLoadingReferences = loadingReferences)
        //     .DisposeWith(this);

        var filter =
            this.WhenAnyValue(x => x.ShowBsaAssets)
                .CombineLatest(
                    this.WhenAnyValue(x => x.ShowEmptyDirectories),
                    this.WhenAnyValue(x => x.ShowOnlyOrphaned),
                    this.WhenAnyValue(x => x.SearchText),
                    (showBsaAssets, showEmptyDirectories, showOnlyOrphaned, searchText) => (
                        ShowBsaAssets: showBsaAssets,
                        ShowEmptyDirectories: showEmptyDirectories,
                        ShowOnlyOrphaned: showOnlyOrphaned,
                        SearchText: searchText))
                .ThrottleMedium()
                .Select(tuple => FilterBuilder(tuple.ShowBsaAssets, tuple.ShowEmptyDirectories, tuple.ShowOnlyOrphaned, tuple.SearchText));

        AssetTreeSource = new HierarchicalTreeDataGridSource<FileSystemLink>([]) {
            Columns = {
                new HierarchicalExpanderColumn<FileSystemLink>(
                    new TemplateColumn<FileSystemLink>(
                        "Name",
                        new FuncDataTemplate<FileSystemLink>((asset, _) => {
                            if (asset is null) return null;

                            // Name
                            var textBlock = new TextBlock {
                                Text = asset.Name,
                                [ToolTip.TipProperty] = GetRootRelativePath(asset),
                                VerticalAlignment = VerticalAlignment.Center,
                            };

                            if (asset.DataSource.IsReadOnly) {
                                textBlock.Foreground = Brushes.Gray;
                            }

                            // Symbol
                            var symbolIcon = new SymbolIcon {
                                Symbol = asset.IsFile
                                    ? assetSymbolService.GetSymbol(asset.Extension)
                                    : Symbol.Folder,
                                VerticalAlignment = VerticalAlignment.Center,
                            };

                            if (asset.IsFile) {
                                // symbolIcon[!TemplatedControl.ForegroundProperty] = asset.AnyOrphaned.Select(x => x
                                //     ? Brushes.IndianRed
                                //     : Brushes.ForestGreen).ToBinding();
                                symbolIcon.Foreground = Brushes.ForestGreen;
                            } else {
                                symbolIcon.Foreground = Brushes.Goldenrod;
                            }

                            return new StackPanel {
                                Orientation = Orientation.Horizontal,
                                Spacing = 5,
                                Children = {
                                    symbolIcon,
                                    textBlock,
                                },
                            };
                        }),
                        null,
                        new GridLength(),
                        new TemplateColumnOptions<FileSystemLink> {
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
                    directory => directory.EnumerateDirectoryLinks(false).Concat(directory.EnumerateFileLinks(false))),
                // directory => ), // TODO add this back when https://github.com/AvaloniaUI/Avalonia.Controls.TreeDataGrid/issues/132 is fixed and still needed
                new TemplateColumn<FileSystemLink>(
                    "Count",
                    new FuncDataTemplate<FileSystemLink>((asset, _) => {
                        if (asset is null) return null;

                        return new TextBlock {
                            Text = ""
                        };
                        // if (asset is null || asset.Asset is FileSystemLink) return null;
                        //
                        // return new TextBlock {
                        //     Text = asset.GetReferencedAssets()
                        //         .Select(x => x.RecordReferences.Count + x.AssetReferences.Count)
                        //         .Sum()
                        //         .ToString(),
                        // };
                    }),
                    null,
                    new GridLength(),
                    new TemplateColumnOptions<FileSystemLink> {
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
                new TemplateColumn<FileSystemLink>(
                    "Flags",
                    new FuncDataTemplate<FileSystemLink>((asset, _) => {
                        if (asset is null) return null;
                        if (asset.IsDirectory) return null;

                        var assetLink = _assetTypeService.GetAssetLink(asset.DataRelativePath);
                        if (assetLink is null) return null;

                        if (assetLink.AssetTypeInstance != SkyrimModelAssetType.Instance) return null;

                        // Missing Assets - todo move this to (nif/file) analyzer system which we can hook into here
                        var assetLinks = modelAssetQuery
                            .ParseAssets(asset, asset.DataRelativePath)
                            .Select(r => r.AssetLink)
                            .Where(assetLink => dataSourceService.FileExists(assetLink.DataRelativePath.Path))
                            .ToArray();
                        if (assetLinks.Length == 0) return null;

                        return new SymbolIcon {
                            Symbol = Symbol.ImportantFilled,
                            Foreground = StandardBrushes.InvalidBrush,
                            [ToolTip.TipProperty] = "Missing Assets\n" + string.Join(",\n", assetLinks.Select(x => x.DataRelativePath)),
                        };
                    })),
            },
        };

        AssetTreeSource.RowSelection!.SingleSelect = false;

        MoveTo = ReactiveCommand.Create<DataRelativePath>(MoveToPath);

        var src = new CancellationTokenSource().DisposeWith(this);
        var cancellationToken = src.Token;
        Task.Run(() => UpdateAssetContentsAsync(filter, cancellationToken), cancellationToken);

        Undo = ReactiveCommand.Create(() => _assetController.Undo());
        Redo = ReactiveCommand.Create(() => _assetController.Redo());

        Open = ReactiveCommand.Create<IReadOnlyList<FileSystemLink?>>(OpenAssets);

        Delete = ReactiveCommand.CreateFromTask<IReadOnlyList<FileSystemLink?>>(async assets => {
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
                ? CreateAssetDialog(deleteAssets, $"Delete {GetRootRelativePath(deleteAssets[0])}", content)
                : CreateAssetDialog(deleteAssets, $"Delete {assets.Count}", content);

            if (await deleteDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
                foreach (var asset in deleteAssets) {
                    _assetController.Delete(asset);
                }
            }
        });

        Rename = ReactiveCommand.CreateFromTask<FileSystemLink>(RenameAsset);

        CopyPath = ReactiveCommand.Create<FileSystemLink>(CopyAssetPath);

        OpenReferences = ReactiveCommand.Create<FileSystemLink>(OpenAssetReferences,
            this.WhenAnyValue(x => x.AssetTreeSource.RowSelection!.SelectedItem)
                .Select(x => true)
            // .Select(x => x?.AnyOrphaned.Select(o => !o) ?? Observable.Return(false))
            // .Switch()
        );

        AddFolder = ReactiveCommand.CreateFromTask<FileSystemLink>(AddAssetFolder);

        OpenAssetBrowser = ReactiveCommand.CreateFromTask<FileSystemLink>(
            asset => dockFactory.Open(DockElement.AssetBrowser, parameter: asset));
    }

    private void UpdateAssetContentsAsync(IObservable<Func<FileSystemLink?, bool>> filter, CancellationToken cancellationToken) {
        Dispatcher.UIThread.Post(() => {
            IsBusyLoadingAssets = true;
        });

        var rootDirectory = new FileSystemLink(_dataSource, new DataRelativePath(string.Empty));
        var links = rootDirectory.EnumerateAllLinks(false).ToArray();

        Dispatcher.UIThread.Post(() => {
            AssetTreeSource.Items = links;
            IsBusyLoadingAssets = false;
        });
    }

    private async Task AddAssetFolder(FileSystemLink dir) {
        var textBox = new TextBox { Text = "New Folder" };
        var relativePath = GetRootRelativePath(dir);
        var folderDialog = CreateAssetDialog($"Add new Folder at {relativePath}", textBox);

        if (await folderDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            var newFolder = dir.FileSystem.Path.Combine(dir.FullPath, textBox.Text);
            dir.FileSystem.Directory.CreateDirectory(newFolder);
        }
    }

    private void OpenAssetReferences(FileSystemLink asset) {
        var referenceBrowserVM = GetReferenceBrowserVM(asset);
        var relativePath = GetRootRelativePath(asset);

        var referenceWindow = new ReferenceWindow(relativePath, referenceBrowserVM);
        referenceWindow.Show(_mainWindow);
    }

    private void CopyAssetPath(FileSystemLink asset) {
        var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;

        clipboard?.SetTextAsync(GetRootRelativePath(asset));
    }

    private async Task RenameAsset(FileSystemLink asset) {
        if (asset.IsDirectory) return;

        var name = asset.NameWithoutExtension;
        var textBox = new TextBox { Text = name };
        var content = new StackPanel { Children = { textBox } };

        var referenceBrowserVM = GetReferenceBrowserVM(asset);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content.Children.Add(new TextBlock {
                Text = "Do you really want to proceed? These references will be modified to point to the new path.",
            });
            content.Children.Add(referenceBrowser);
        }

        var renameDialog = CreateAssetDialog($"Rename {name}", content);
        if (await renameDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            var oldName = asset.NameWithoutExtension;
            if (string.Equals(oldName, textBox.Text, DataRelativePath.PathComparison)) return;

            _assetController.Rename(asset, textBox.Text + asset.Extension);
        }
    }

    private ReferenceBrowserVM? GetReferenceBrowserVM(params FileSystemLink[] assets) {
        var recordReferences = new HashSet<IFormLinkIdentifier>(FormLinkIdentifierEqualityComparer.Instance);
        var assetReferences = new HashSet<DataRelativePath>();

        // Gather all references to all assets
        foreach (var (_, dataRelativePath) in assets) {
            var assetLink = _assetTypeService.GetAssetLink(dataRelativePath);
            if (assetLink is null) continue;

            assetReferences.AddRange(_assetReferenceController.GetAssetReferences(assetLink));
            recordReferences.AddRange(_assetReferenceController.GetRecordReferences(assetLink));
        }

        var references = assetReferences
            .Select(path => new AssetReferenceVM(path, _linkCacheProvider, _assetTypeService, _assetReferenceController, _recordReferenceController))
            .Cast<IReferenceVM>()
            .Combine(recordReferences.Select(x => new RecordReferenceVM(x, _linkCacheProvider, _recordReferenceController)))
            .ToArray();

        if (references.Length == 0) return null;

        return _referenceBrowserVMFactory(assets.Length == 1 ? assets[0] : assets, references);
    }

    public async Task Drop(FileSystemLink dstDirectory, DragInfo dragInfo) {
        var draggedAssets = dragInfo.Indexes
            .Select(indexPath => {
                var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                var row = dragInfo.Source.Rows[rowIndex];
                return row.Model;
            })
            .OfType<FileSystemLink>()
            .ToArray();

        var firstDraggedAsset = draggedAssets.FirstOrDefault();
        if (firstDraggedAsset is null) return;

        var srcDirectory = firstDraggedAsset.ParentDirectory;
        if (srcDirectory is null) return;

        var relativeSrcDirectory = GetRootRelativePath(srcDirectory);
        var relativeDstDirectory = GetRootRelativePath(dstDirectory);

        // No need to move if the source and destination are the same
        if (string.Equals(relativeSrcDirectory, relativeDstDirectory, DataRelativePath.PathComparison)) return;

        var content = new StackPanel {
            Children = {
                draggedAssets.Length > 1
                    ? new TextBlock {
                        Inlines = [
                            new Run("Move "),
                            new Run(draggedAssets.Length.ToString()) {
                                Foreground = StandardBrushes.ValidBrush,
                            },
                            new Run(" Items from "),
                            new Run(relativeSrcDirectory) {
                                Foreground = StandardBrushes.ValidBrush,
                            },
                            new Run(" to "),
                            new Run(relativeDstDirectory) {
                                Foreground = StandardBrushes.ValidBrush,
                            },
                        ],
                        FontSize = 14,
                    }
                    : new TextBlock {
                        Inlines = [
                            new Run("Move "),
                            new Run(GetRootRelativePath(firstDraggedAsset)) {
                                Foreground = StandardBrushes.ValidBrush,
                            },
                            new Run(" to "),
                            new Run(relativeDstDirectory) {
                                Foreground = StandardBrushes.ValidBrush,
                            },
                        ],
                        FontSize = 14,
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

    public IEnumerable<Control> GetContextMenuItems(FileSystemLink asset) {
        List<Control> items = [
            _menuItemProvider.File(Open, AssetTreeSource.RowSelection!.SelectedItems),
            _menuItemProvider.Rename(Rename, asset),
            _menuItemProvider.Delete(Delete, AssetTreeSource.RowSelection!.SelectedItems),
            new Separator(),
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = "Copy Path",
                Command = CopyPath,
                CommandParameter = asset,
            },
        ];

        var assetLink = _assetTypeService.GetAssetLink(asset.DataRelativePath);
        if (assetLink is not null) {
            var assetReferences = _assetReferenceController.GetAssetReferences(assetLink);
            var recordReferences = _assetReferenceController.GetRecordReferences(assetLink);
            if (assetReferences.Any() || recordReferences.Any()) {
                items.Add(_menuItemProvider.References(OpenReferences, assetLink));
            }
        }

        if (asset.IsDirectory) {
            items.Add(new Separator());
            items.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Open },
                Header = "Open in new Asset Browser",
                Command = OpenAssetBrowser,
                CommandParameter = asset,
            });
            items.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.NewFolder },
                Header = "Add Folder",
                Command = AddFolder,
                CommandParameter = asset,
            });
        }

        return items;
    }

    private TaskDialog CreateAssetDialog(string header, object? content = null) {
        var assetDialog = new TaskDialog {
            Header = header,
            Content = content,
            XamlRoot = _mainWindow,
            Buttons = {
                TaskDialogButton.OKButton,
                TaskDialogButton.CancelButton,
            },
        };

        if (IsBusyLoadingReferences) {
            assetDialog.IconSource = new SymbolIconSource { Symbol = Symbol.ReportHacked };
            assetDialog.Header += " - Not all references have been loaded yet.";
        }

        return assetDialog;
    }

    private TaskDialog CreateAssetDialog(IEnumerable<FileSystemLink> footerAssets, string header, object? content = null) {
        var dialog = CreateAssetDialog(header, content);

        var fileLinks = footerAssets.ToArray();
        if (fileLinks.Length > 1) {
            dialog.FooterVisibility = TaskDialogFooterVisibility.Auto;
            dialog.Footer = new ItemsControl {
                ItemsSource = fileLinks,
                ItemTemplate = new FuncDataTemplate<FileSystemLink>((asset, _) => new TextBlock { Text = GetRootRelativePath(asset) }),
            };
        }

        return dialog;
    }

    private string GetRootRelativePath(FileSystemLink link) {
        return link.FileSystem.Path.GetRelativePath(_dataSource.Path, link.FullPath);
    }

    private void MoveToPath(DataRelativePath path) {
        var pathIndices = new IndexPath();
        FileSystemLink? currentNode;
        var items = AssetTreeSource.Items.ToList();
        do {
            currentNode = items.Find(a => path.Path.StartsWith(GetRootRelativePath(a), DataRelativePath.PathComparison));
            if (currentNode is null) break;

            pathIndices = pathIndices.Append(items.IndexOf(currentNode));
            items = currentNode.EnumerateAllLinks(false).ToList();
        }
        while (!currentNode.DataRelativePath.Equals(path));

        var indexPathPart = new IndexPath();
        foreach (var pathIndex in pathIndices.SkipLast(1)) {
            indexPathPart = indexPathPart.Append(pathIndex);

            var rowIndex = AssetTreeSource.Rows.ModelIndexToRowIndex(indexPathPart);
            if (AssetTreeSource.Rows[rowIndex] is HierarchicalRow<FileSystemLink> hierarchicalRow) {
                hierarchicalRow.IsExpanded = true;
            } else {
                break;
            }
        }

        AssetTreeSource.RowSelection!.Clear();
        AssetTreeSource.RowSelection.Select(pathIndices);
    }

    public void OpenAssets(IReadOnlyList<FileSystemLink?> assets) {
        foreach (var asset in assets.WhereNotNull()) {
            if (!asset.Exists()) return;

            var openPath = asset.FullPath;
            // if (asset is { Asset: AssetFile, IsVirtual: true }) {
            //     // When the asset is virtual, we need to extract it to a temp file first
            //     var tempFilePath = _archiveService.TryGetFileAsTempFile(asset.FullPath);
            //     if (tempFilePath is not null) {
            //         openPath = tempFilePath;
            //     }
            // }

            // Open the file via the standard program
            Process.Start(new ProcessStartInfo {
                FileName = openPath,
                WorkingDirectory = asset.ParentDirectory?.FullPath,
                UseShellExecute = true,
                Verb = "open",
            });
        }
    }

    public Func<FileSystemLink?, bool> FilterBuilder(bool showReadOnly, bool showEmptyDirectories, bool showOnlyOrphaned, string searchText) {
        var hideReadOnly = !showReadOnly;
        var hideEmptyDirectories = !showEmptyDirectories;
        var filterText = !string.IsNullOrWhiteSpace(searchText);

        return ShowAssetFilter;

        bool ShowAssetFilter(FileSystemLink? fileLink) {
            if (fileLink is null) return false;

            if (hideReadOnly && fileLink.DataSource.IsReadOnly) return false;
            if (hideEmptyDirectories && fileLink.IsDirectory) return false;

            if (showOnlyOrphaned) {
                if (fileLink.IsDirectory) {
                    if (!HasMatchingChildren()) return false;
                } else {
                    var assetLink = _assetTypeService.GetAssetLink(fileLink.DataRelativePath);
                    if (assetLink is null) return false;

                    var assetReferences = _assetReferenceController.GetAssetReferences(assetLink);
                    if (assetReferences.Any()) return false;
                }
            }

            if (filterText) {
                if (fileLink.IsDirectory) {
                    if (!HasMatchingChildren()) return false;
                } else {
                    if (!_searchFilter.Filter(fileLink.DataRelativePath.Path, searchText)) return false;
                }
            }

            return true;

            bool HasMatchingChildren() {
                var assetLink = _assetTypeService.GetAssetLink(fileLink.DataRelativePath);
                if (assetLink is null) return false;

                var assetReferences = _assetReferenceController.GetAssetReferences(assetLink);
                return assetReferences.Any(r => ShowAssetFilter(fileLink with { DataRelativePath = r }));
            }
        }
    }
}
