using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Asset;
using CreationEditor.Avalonia.Models.Reference;
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
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Asset.Browser;

public sealed class AssetBrowserVM : ViewModel, IAssetBrowserVM {
    private readonly Func<object?, IReference[], ReferenceBrowserVM> _referenceBrowserVMFactory;
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IMenuItemProvider _menuItemProvider;
    private readonly IAssetReferenceController _assetReferenceController;
    private readonly IAssetController _assetController;
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IRecordReferenceController _recordReferenceController;
    private readonly IAssetTypeService _assetTypeService;
    private readonly MainWindow _mainWindow;
    private readonly IAssetProvider _assetProvider;
    private readonly string _root;

    private AssetDirectory? _dataDirectory;
    public AssetDirectory DataDirectory => _dataDirectory ??= _assetProvider.GetAssetContainer(_dataDirectoryProvider.Path);

    [Reactive] public string SearchText { get; set; } = string.Empty;
    [Reactive] public bool ShowBsaAssets { get; set; }
    [Reactive] public bool ShowEmptyDirectories { get; set; }
    [Reactive] public bool ShowOnlyOrphaned { get; set; }

    [Reactive] public bool IsBusyLoadingAssets { get; set; } = true;
    [Reactive] public bool IsBusyLoadingReferences { get; set; } = true;

    public ReactiveCommand<string, Unit> MoveTo { get; }

    public ReactiveCommand<Unit, Unit> Undo { get; }
    public ReactiveCommand<Unit, Unit> Redo { get; }

    public ReactiveCommand<IReadOnlyList<AssetTreeItem?>, Unit> Open { get; }
    public ReactiveCommand<IReadOnlyList<AssetTreeItem?>, Unit> Delete { get; }
    public ReactiveCommand<AssetTreeItem, Unit> Rename { get; }
    public ReactiveCommand<AssetTreeItem, Unit> CopyPath { get; }
    public ReactiveCommand<AssetTreeItem, Unit> OpenReferences { get; }
    public ReactiveCommand<AssetDirectory, Unit> AddFolder { get; }
    public ReactiveCommand<AssetDirectory, Unit> OpenAssetBrowser { get; }

    public HierarchicalTreeDataGridSource<AssetTreeItem> AssetTreeSource { get; }

    public AssetBrowserVM(
        Func<object?, IReference[], ReferenceBrowserVM> referenceBrowserVMFactory,
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
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
        IAssetProvider assetProvider,
        ISearchFilter searchFilter,
        string root) {
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        _menuItemProvider = menuItemProvider;
        _assetReferenceController = assetReferenceController;
        _assetController = assetController;
        _assetTypeService = assetTypeService;
        _linkCacheProvider = linkCacheProvider;
        _recordReferenceController = recordReferenceController;
        _mainWindow = mainWindow;
        _assetProvider = assetProvider;
        _root = root;

        _assetReferenceController.IsLoading
            .ObserveOnGui()
            .Subscribe(loadingReferences => IsBusyLoadingReferences = loadingReferences)
            .DisposeWith(this);

        var filter =
            this.WhenAnyValue(x => x.ShowBsaAssets)
                .CombineLatest(
                    this.WhenAnyValue(x => x.ShowEmptyDirectories),
                    this.WhenAnyValue(x => x.ShowOnlyOrphaned),
                    this.WhenAnyValue(x => x.SearchText),
                    (showBsaAssets, showEmptyDirectories, showOnlyOrphaned, searchText) =>
                        (ShowBsaAssets: showBsaAssets, ShowEmptyDirectories: showEmptyDirectories, ShowOnlyOrphaned: showOnlyOrphaned, SearchText: searchText))
                .ThrottleMedium()
                .Select(x => {
                    var hideBsa = !x.ShowBsaAssets;
                    var hideEmptyDirectories = !x.ShowEmptyDirectories;
                    var showOnlyOrphaned = x.ShowOnlyOrphaned;
                    var filterText = !string.IsNullOrWhiteSpace(x.SearchText);

                    return new Func<IAsset, bool>(ShowAsset);

                    bool ShowAsset(IAsset asset) {
                        if (asset is null) return false;

                        if (hideBsa && asset.IsVirtual) return false;
                        if (hideEmptyDirectories && asset is { IsDirectory: true, HasChildren: false }) return false;

                        if (showOnlyOrphaned) {
                            if (asset is AssetFile file) {
                                if (file.ReferencedAsset.HasReferences) return false;
                            } else {
                                if (!asset.Children.ToList().Any(ShowAsset)) return false;
                            }
                        }

                        if (filterText) {
                            if (asset.IsDirectory) {
                                if (!asset.Children.ToList().Any(ShowAsset)) return false;
                            } else {
                                if (!searchFilter.Filter(asset.Path, x.SearchText)) return false;
                            }
                        }

                        return true;
                    }
                });

        AssetTreeSource = new HierarchicalTreeDataGridSource<AssetTreeItem>(Array.Empty<AssetTreeItem>()) {
            Columns = {
                new HierarchicalExpanderColumn<AssetTreeItem>(
                    new TemplateColumn<AssetTreeItem>(
                        "Name",
                        new FuncDataTemplate<AssetTreeItem>((asset, _) => {
                            if (asset is null) return null;

                            // Name
                            var textBlock = new TextBlock {
                                Text = _fileSystem.Path.GetFileName(asset.Path),
                                [ToolTip.TipProperty] = GetRootRelativePath(asset.Path),
                                VerticalAlignment = VerticalAlignment.Center
                            };

                            if (asset.IsVirtual) {
                                textBlock.Foreground = Brushes.Gray;
                            }

                            // Symbol
                            var symbolIcon = new SymbolIcon {
                                Symbol = asset.Asset is AssetFile file ? assetSymbolService.GetSymbol(file.ReferencedAsset.AssetLink.Extension) : Symbol.Folder,
                                VerticalAlignment = VerticalAlignment.Center
                            };

                            switch (asset.Asset) {
                                case AssetFile:
                                    symbolIcon[!TemplatedControl.ForegroundProperty] = asset.AnyOrphaned.Select(x => x ? Brushes.IndianRed : Brushes.ForestGreen).ToBinding();
                                    break;
                                case AssetDirectory:
                                    symbolIcon.Foreground = Brushes.Goldenrod;
                                    break;
                                default:
                                    symbolIcon.Foreground = StandardBrushes.ValidBrush;
                                    break;
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
                        new TemplateColumnOptions<AssetTreeItem> {
                            CanUserResizeColumn = true,
                            CanUserSortColumn = true,
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = asset => _fileSystem.Path.GetFileName(asset.Path),
                            CompareAscending = (x, y) => {
                                var checkNull = ObjectComparers.CheckNull(x, y);
                                return -(checkNull ?? AssetComparers.PathComparer.Compare(x?.Asset, y?.Asset));
                            },
                            CompareDescending = (x, y) => {
                                var checkNull = ObjectComparers.CheckNull(x, y);
                                return checkNull ?? AssetComparers.PathComparer.Compare(x?.Asset, y?.Asset);
                            },
                        }),
                    directory => directory.Children,
                    directory => directory.HasChildren),
                new TemplateColumn<AssetTreeItem>(
                    "Count",
                    new FuncDataTemplate<AssetTreeItem>((asset, _) => {
                        if (asset is null || asset.Asset is AssetDirectory) return null;

                        return new TextBlock {
                            Text = asset.GetReferencedAssets()
                                .Select(x => x.RecordReferences.Count + x.NifReferences.Count())
                                .Sum()
                                .ToString()
                        };
                    }),
                    null,
                    new GridLength(),
                    new TemplateColumnOptions<AssetTreeItem> {
                        CanUserResizeColumn = true,
                        CanUserSortColumn = true,
                        CompareAscending = (x, y) => {
                            var checkNull = ObjectComparers.CheckNull(x, y);
                            return checkNull ?? AssetComparers.ReferenceCountComparer.Compare(x?.Asset, y?.Asset);
                        },
                        CompareDescending = (x, y) => {
                            var checkNull = ObjectComparers.CheckNull(x, y);
                            return -(checkNull ?? AssetComparers.ReferenceCountComparer.Compare(x?.Asset, y?.Asset));
                        },
                    }),
                new TemplateColumn<AssetTreeItem>(
                    "Flags",
                    new FuncDataTemplate<AssetTreeItem>((asset, _) => {
                        if (asset?.Asset is AssetFile assetFile && assetFile.ReferencedAsset.AssetLink.Type == SkyrimModelAssetType.Instance) {
                            // Missing Assets - todo move this to (nif/file) analyzer system which we can hook into here
                            var assetLinks = modelAssetQuery
                                .ParseAssets(_fileSystem.Path.Combine(_dataDirectoryProvider.Path, assetFile.ReferencedAsset.AssetLink.DataRelativePath))
                                .Select(r => r.AssetLink)
                                .Where(assetLink => !DataDirectory.Contains(_fileSystem.Path.Combine(_dataDirectoryProvider.Path, assetLink.DataRelativePath)))
                                .ToArray();

                            if (assetLinks.Length > 0) {
                                return new SymbolIcon {
                                    Symbol = Symbol.ImportantFilled,
                                    Foreground = StandardBrushes.InvalidBrush,
                                    [ToolTip.TipProperty] = "Missing Assets\n" + string.Join(",\n", assetLinks.Select(x => x.DataRelativePath))
                                };
                            }
                        }

                        return null;
                    }))
            },
        };

        AssetTreeSource.RowSelection!.SingleSelect = false;

        MoveTo = ReactiveCommand.Create<string>(path => {
            var pathIndices = new IndexPath();
            AssetTreeItem? currentNode;
            var items = AssetTreeSource.Items.ToArray();
            do {
                currentNode = items.FirstOrDefault(a => path.StartsWith(GetRootRelativePath(a.Path), AssetCompare.PathComparison));
                if (currentNode is null) break;

                pathIndices = pathIndices.Append(items.IndexOf(currentNode));
                items = currentNode.Children.ToArray();
            }
            while (!string.Equals(path, currentNode.Path, AssetCompare.PathComparison));

            var indexPathPart = new IndexPath();
            foreach (var pathIndex in pathIndices.SkipLast(1)) {
                indexPathPart = indexPathPart.Append(pathIndex);

                var rowIndex = AssetTreeSource.Rows.ModelIndexToRowIndex(indexPathPart);
                if (AssetTreeSource.Rows[rowIndex] is HierarchicalRow<AssetTreeItem> hierarchicalRow) {
                    hierarchicalRow.IsExpanded = true;
                } else {
                    break;
                }
            }

            AssetTreeSource.RowSelection.Clear();
            AssetTreeSource.RowSelection.Select(pathIndices);
        });

        var src = new CancellationTokenSource();
        var cancellationToken = src.Token;
        Task.Run(() => {
            var assetContainer = assetProvider.GetAssetContainer(_root, cancellationToken);
            var rootTree = new AssetTreeItem(_root, assetContainer, _fileSystem, filter);
            Dispatcher.UIThread.Post(() => {
                AssetTreeSource.Items = rootTree.Children;
                IsBusyLoadingAssets = false;
            });
        }, cancellationToken);

        Undo = ReactiveCommand.Create(() => _assetController.Undo());
        Redo = ReactiveCommand.Create(() => _assetController.Redo());

        Open = ReactiveCommand.Create<IReadOnlyList<AssetTreeItem?>>(assets => {
            foreach (var asset in assets.NotNull()) {
                var openPath = asset.Path;
                if (asset is { Asset: AssetFile, IsVirtual: true }) {
                    // When the asset is virtual, we need to extract it to a temp file first
                    var tempFilePath = archiveService.TryGetFileAsTempFile(asset.Path);
                    if (tempFilePath is not null) {
                        openPath = tempFilePath;
                    }
                } else if (!_fileSystem.Path.Exists(asset.Path)) {
                    return;
                }

                // Open the file via the standard program
                Process.Start(new ProcessStartInfo {
                    FileName = openPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        });

        Delete = ReactiveCommand.CreateFromTask<IReadOnlyList<AssetTreeItem?>>(async assets => {
            var deleteAssets = assets.NotNull().ToArray();
            Control? content = null;

            var referenceBrowserVM = GetReferenceBrowserVM(deleteAssets);
            if (referenceBrowserVM is not null) {
                var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
                content = new StackPanel {
                    Children = {
                        new TextBlock { Text = "Do you really want to proceed? There are still references to these assets." },
                        referenceBrowser,
                    }
                };
            }

            var deleteDialog = assets.Count == 1
                ? CreateAssetDialog(deleteAssets, $"Delete {GetRootRelativePath(deleteAssets[0].Path)}", content)
                : CreateAssetDialog(deleteAssets, $"Delete {assets.Count}", content);

            if (await deleteDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
                foreach (var asset in deleteAssets) {
                    _assetController.Delete(asset.Path);
                }
            }
        });

        Rename = ReactiveCommand.CreateFromTask<AssetTreeItem>(async asset => {
            var name = _fileSystem.Path.GetFileNameWithoutExtension(asset.Path);
            var directory = _fileSystem.Path.GetDirectoryName(asset.Path);
            if (directory is null) return;

            var textBox = new TextBox { Text = name };
            var content = new StackPanel { Children = { textBox } };

            var referenceBrowserVM = GetReferenceBrowserVM(asset);
            if (referenceBrowserVM is not null) {
                var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
                content.Children.Add(new TextBlock { Text = "Do you really want to proceed? These references will be modified to point to the new path." });
                content.Children.Add(referenceBrowser);
            }

            var renameDialog = CreateAssetDialog($"Rename {name}", content);
            if (await renameDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
                var oldName = _fileSystem.Path.GetFileNameWithoutExtension(asset.Path);
                if (string.Equals(oldName, textBox.Text, AssetCompare.PathComparison)) return;

                _assetController.Rename(asset.Path, textBox.Text + _fileSystem.Path.GetExtension(asset.Path));
            }
        });

        CopyPath = ReactiveCommand.Create<AssetTreeItem>(asset => {
            var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;

            clipboard?.SetTextAsync(GetRootRelativePath(asset.Path));
        });

        OpenReferences = ReactiveCommand.Create<AssetTreeItem>(asset => {
                var referenceBrowserVM = GetReferenceBrowserVM(asset);
                var relativePath = GetRootRelativePath(asset.Path);

                var referenceWindow = new ReferenceWindow(relativePath, referenceBrowserVM);
                referenceWindow.Show(mainWindow);
            },
            this.WhenAnyValue(x => x.AssetTreeSource.RowSelection!.SelectedItem)
                .Select(x => x?.AnyOrphaned.Select(o => !o) ?? Observable.Return(false))
                .Switch());

        AddFolder = ReactiveCommand.CreateFromTask<AssetDirectory>(async dir => {
            var textBox = new TextBox { Text = "New Folder" };
            var relativePath = GetRootRelativePath(dir.Path);
            var folderDialog = CreateAssetDialog($"Add new Folder at {relativePath}", textBox);

            if (await folderDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
                var newFolder = _fileSystem.Path.Combine(dir.Path, textBox.Text);
                _fileSystem.Directory.CreateDirectory(newFolder);
            }
        });

        OpenAssetBrowser = ReactiveCommand.Create<AssetDirectory>(asset => {
            dockFactory.Open(DockElement.AssetBrowser, parameter: asset.Path);
        });
    }

    private ReferenceBrowserVM? GetReferenceBrowserVM(params AssetTreeItem[] assets) {
        var recordReferences = new HashSet<IFormLinkGetter>(FormLinkIdentifierEqualityComparer.Instance);
        var assetReferences = new HashSet<string>(AssetCompare.PathComparer);

        // Gather all references to all assets
        var referencedAssets = assets
            .NotNull()
            .SelectMany(a => a.GetReferencedAssets())
            .DistinctBy(a => a.AssetLink.DataRelativePath.ToLowerInvariant());

        foreach (var referencedAsset in referencedAssets) {
            recordReferences.AddRange(referencedAsset.RecordReferences);
            assetReferences.AddRange(referencedAsset.NifReferences);
        }

        var references = assetReferences
            .Select(path => new AssetReference(path, _linkCacheProvider, _assetTypeService, _assetReferenceController, _recordReferenceController))
            .Cast<IReference>()
            .Combine(recordReferences.Select(x => new RecordReference(x, _linkCacheProvider, _recordReferenceController)))
            .ToArray();

        if (references.Length == 0) return null;

        return _referenceBrowserVMFactory(assets.Length == 1 ? assets[0] : assets, references);
    }

    public async Task Drop(TreeDataGridRowDragEventArgs e) {
        if (e.TargetRow.Model is not AssetTreeItem { Asset: AssetDirectory directory }) return;
        if (e.Inner.Data.Get(DragInfo.DataFormat) is not DragInfo dragInfo) return;

        var draggedAssets = dragInfo.Indexes
            .Select(indexPath => {
                var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                var row = dragInfo.Source.Rows[rowIndex];
                return row.Model;
            })
            .OfType<AssetTreeItem>()
            .ToArray();

        var firstDraggedAsset = draggedAssets.FirstOrDefault();
        if (firstDraggedAsset is null) return;

        var srcDirectory = _fileSystem.Path.GetDirectoryName(firstDraggedAsset.Path);
        if (srcDirectory is null) return;

        var relativeSrcDirectory = GetRootRelativePath(srcDirectory);
        var relativeDstDirectory = GetRootRelativePath(directory.Path);

        // No need to move if the source and destination are the same
        if (string.Equals(relativeSrcDirectory, relativeDstDirectory, AssetCompare.PathComparison)) return;

        var content = new StackPanel {
            Children = {
                draggedAssets.Length > 1
                    ? new TextBlock {
                        Inlines = new InlineCollection {
                            new Run("Move "),
                            new Run(draggedAssets.Length.ToString()) {
                                Foreground = StandardBrushes.ValidBrush
                            },
                            new Run(" Items from "),
                            new Run(relativeSrcDirectory) {
                                Foreground = StandardBrushes.ValidBrush
                            },
                            new Run(" to "),
                            new Run(relativeDstDirectory) {
                                Foreground = StandardBrushes.ValidBrush
                            },
                        },
                        FontSize = 14,
                    }
                    : new TextBlock {
                        Inlines = new InlineCollection {
                            new Run("Move "),
                            new Run(GetRootRelativePath(firstDraggedAsset.Path)) {
                                Foreground = StandardBrushes.ValidBrush
                            },
                            new Run(" to "),
                            new Run(relativeDstDirectory) {
                                Foreground = StandardBrushes.ValidBrush
                            },
                        },
                        FontSize = 14,
                    }
            }
        };

        var referenceBrowserVM = GetReferenceBrowserVM(draggedAssets);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content.Children.Add(new TextBlock { Text = "Do you really want to proceed? These references will be modified to point to the new path." });
            content.Children.Add(referenceBrowser);
        }

        // Show a confirmation dialog
        var moveDialog = CreateAssetDialog(draggedAssets, "Confirm", content);
        if (await moveDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            // Move all assets and remap their references
            foreach (var asset in draggedAssets) _assetController.Move(asset.Path, directory.Path);

        }
    }

    public void ContextMenu(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control { DataContext: AssetTreeItem asset } control) return;

        var contextFlyout = new MenuFlyout {
            Items = {
                _menuItemProvider.File(Open, AssetTreeSource.RowSelection!.SelectedItems),
                _menuItemProvider.Rename(Rename, asset),
                _menuItemProvider.Delete(Delete, AssetTreeSource.RowSelection!.SelectedItems),
                new Separator(),
                new MenuItem {
                    Icon = new SymbolIcon { Symbol = Symbol.Copy },
                    Header = "Copy Path",
                    Command = CopyPath,
                    CommandParameter = asset
                }
            }
        };

        if (asset.GetReferencedAssets().Any()) {
            contextFlyout.Items.Add(_menuItemProvider.References(OpenReferences, asset));
        }

        if (asset.Asset is AssetDirectory dir) {
            contextFlyout.Items.Add(new Separator());
            contextFlyout.Items.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Open },
                Header = "Open in new Asset Browser",
                Command = OpenAssetBrowser,
                CommandParameter = dir
            });
            contextFlyout.Items.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.NewFolder },
                Header = "Add Folder",
                Command = AddFolder,
                CommandParameter = dir
            });
        }

        contextFlyout.ShowAt(control, true);

        e.Handled = true;
    }

    private TaskDialog CreateAssetDialog(string header, object? content = null) {
        var assetDialog = new TaskDialog {
            Header = header,
            Content = content,
            XamlRoot = _mainWindow,
            Buttons = {
                TaskDialogButton.OKButton,
                TaskDialogButton.CancelButton
            },
        };

        if (IsBusyLoadingReferences) {
            assetDialog.Header += " - WARNING: Not all references have been loaded yet.";
        }

        return assetDialog;
    }

    private TaskDialog CreateAssetDialog(IEnumerable<AssetTreeItem> footerAssets, string header, object? content = null) {
        var dialog = CreateAssetDialog(header, content);

        var assetTreeItems = footerAssets.ToArray();
        if (assetTreeItems.Length > 1) {
            dialog.FooterVisibility = TaskDialogFooterVisibility.Auto;
            dialog.Footer = new ItemsRepeater {
                ItemsSource = assetTreeItems,
                ItemTemplate = new FuncDataTemplate<AssetTreeItem>((asset, _) => new TextBlock { Text = GetRootRelativePath(asset.Path) }),
            };
        }

        return dialog;
    }

    private string GetRootRelativePath(string assetPath) {
        return _fileSystem.Path.GetRelativePath(_root, assetPath);
    }
}
