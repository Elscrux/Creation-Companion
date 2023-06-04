using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Attached;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Asset;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Avalonia.Services.Record.List;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.FormLink;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Key = Avalonia.Input.Key;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Asset.Browser;

public sealed class AssetBrowserVM : ViewModel, IAssetBrowserVM {
    private readonly IFileSystem _fileSystem;
    private readonly IAssetController _assetController;
    private readonly IModelModificationService _modelModificationService;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IRecordController _recordController;
    private readonly IRecordListFactory _recordListFactory;
    private readonly MainWindow _mainWindow;
    private readonly string _root;

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
    public ReactiveCommand<AssetTreeItem, Unit> OpenReferences { get; }
    public ReactiveCommand<AssetDirectory, Unit> AddFolder { get; }
    public ReactiveCommand<AssetDirectory, Unit> OpenAssetBrowser { get; }

    public HierarchicalTreeDataGridSource<AssetTreeItem> AssetTreeSource { get; }

    public AssetBrowserVM(
        IFileSystem fileSystem,
        IAssetReferenceController assetReferenceController,
        IAssetController assetController,
        IModelModificationService modelModificationService,
        IEditorEnvironment editorEnvironment,
        IDataDirectoryProvider dataDirectoryProvider,
        IRecordController recordController,
        IAssetSymbolService assetSymbolService,
        IDockFactory dockFactory,
        IRecordListFactory recordListFactory,
        MainWindow mainWindow,
        IAssetProvider assetProvider,
        string root) {
        _fileSystem = fileSystem;
        _assetController = assetController;
        _modelModificationService = modelModificationService;
        _editorEnvironment = editorEnvironment;
        _dataDirectoryProvider = dataDirectoryProvider;
        _recordController = recordController;
        _recordListFactory = recordListFactory;
        _mainWindow = mainWindow;
        _root = root;

        assetReferenceController.IsLoading
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
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .Select(x => {
                    var hideBsa = !x.ShowBsaAssets;
                    var hideEmptyDirectories = !x.ShowEmptyDirectories;
                    var showOnlyOrphaned = x.ShowOnlyOrphaned;
                    var filterText = !string.IsNullOrWhiteSpace(x.SearchText);

                    return new Func<IAsset, bool>(ShowAsset);

                    bool ShowAsset(IAsset asset) {
                        if (asset == null) return false;

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
                                if (!asset.Path.Contains(x.SearchText, AssetCompare.PathComparison)) return false;
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
                            if (asset == null) return null;

                            var textBlock = new TextBlock {
                                Text = _fileSystem.Path.GetFileName(asset.Path),
                                [ToolTip.TipProperty] = _fileSystem.Path.GetRelativePath(_root, asset.Path),
                                VerticalAlignment = VerticalAlignment.Center
                            };

                            if (asset.IsVirtual) {
                                textBlock.Foreground = Brushes.Gray;
                            }

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
                        new GridLength(),
                        new TemplateColumnOptions<AssetTreeItem> {
                            CanUserResizeColumn = true,
                            CanUserSortColumn = false,
                            IsTextSearchEnabled = true,
                            TextSearchValueSelector = asset => _fileSystem.Path.GetFileName(asset.Path),
                        }),
                    directory => directory.Children,
                    directory => directory.HasChildren),
            },
        };

        AssetTreeSource.RowSelection!.SingleSelect = false;

        MoveTo = ReactiveCommand.Create<string>(path => {
            var pathIndices = new IndexPath();
            AssetTreeItem? currentNode;
            var items = AssetTreeSource.Items.ToArray();
            do {
                currentNode = items.FirstOrDefault(a => path.StartsWith(_fileSystem.Path.GetRelativePath(_root, a.Path), AssetCompare.PathComparison));
                if (currentNode == null) break;

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

        Task.Run(() => {
            var assetContainer = assetProvider.GetAssetContainer(_root);
            var rootTree = new AssetTreeItem(_root, assetContainer, _fileSystem, filter);
            Dispatcher.UIThread.Post(() => {
                AssetTreeSource.Items = rootTree.Children;
                IsBusyLoadingAssets = false;
            });
        });

        Undo = ReactiveCommand.Create(() => _assetController.Undo());
        Redo = ReactiveCommand.Create(() => _assetController.Redo());

        Open = ReactiveCommand.Create<IReadOnlyList<AssetTreeItem?>>(assets => {
            foreach (var asset in assets.NotNull()) {
                if (!_fileSystem.Path.Exists(asset.Path)) return;

                Process.Start(new ProcessStartInfo {
                    FileName = asset.Path,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        });

        Delete = ReactiveCommand.CreateFromTask<IReadOnlyList<AssetTreeItem?>>(async assets => {
            var deleteAssets = assets.NotNull().ToArray();
            var referenceTabs = GetReferenceTabs(deleteAssets);
            Control? content = null;
            if (referenceTabs.Count > 0) {
                var referenceList = new ReferenceList(referenceTabs);
                content = new StackPanel {
                    Children = {
                        new TextBlock { Text = "Do you really want to proceed? There are still references to these assets." },
                        referenceList,
                    }
                };
            }

            var deleteDialog = assets.Count == 1
                ? CreateAssetDialog(deleteAssets, $"Delete {_fileSystem.Path.GetRelativePath(_root, deleteAssets[0].Path)}", content)
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
            if (directory == null) return;

            var textBox = new TextBox { Text = name };
            var content = new StackPanel { Children = { textBox } };

            var referenceTabs = GetReferenceTabs(asset);
            if (referenceTabs.Count > 0) {
                var referenceList = new ReferenceList(referenceTabs);
                content.Children.Add(new TextBlock { Text = "Do you really want to proceed? These references will be modified to point to the new path." });
                content.Children.Add(referenceList);
            }

            var renameDialog = CreateAssetDialog($"Rename {name}", content);
            if (await renameDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
                var newName = _fileSystem.Path.Combine(directory, textBox.Text);
                if (string.Equals(newName, name, AssetCompare.PathComparison)) return;

                MoveAndRemapReferences(asset, newName + _fileSystem.Path.GetExtension(asset.Path));
            }
        });

        OpenReferences = ReactiveCommand.Create<AssetTreeItem>(asset => {
                var referenceTabs = GetReferenceTabs(asset);
                var relativePath = _fileSystem.Path.GetRelativePath(_root, asset.Path);

                var referenceWindow = new ReferenceWindow(relativePath, referenceTabs);
                referenceWindow.Show(mainWindow);
            },
            this.WhenAnyValue(x => x.AssetTreeSource.RowSelection!.SelectedItem)
                .Select(x => x?.AnyOrphaned.Select(o => !o) ?? Observable.Return(false))
                .Switch());

        AddFolder = ReactiveCommand.CreateFromTask<AssetDirectory>(async dir => {
            var textBox = new TextBox { Text = "New Folder" };
            var relativePath = _fileSystem.Path.GetRelativePath(_root, dir.Path);
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

    private List<TabItem> GetReferenceTabs(params AssetTreeItem[] assets) {
        var recordReferences = new HashSet<IFormLinkGetter>(FormLinkIdentifierEqualityComparer.Instance);
        var assetReferences = new HashSet<string>(AssetCompare.PathComparer);

        // Gather all references to all assets
        var references = assets
            .NotNull()
            .SelectMany(a => a.GetReferencedAssets())
            .DistinctBy(a => a.AssetLink.DataRelativePath.ToLowerInvariant());

        foreach (var referencedAsset in references) {
            recordReferences.AddRange(referencedAsset.RecordReferences);
            assetReferences.AddRange(referencedAsset.NifReferences);
        }

        // Create record reference tab
        var tabs = new List<TabItem>();
        if (recordReferences.Count > 0) {
            tabs.Add(new TabItem {
                Header = "Record",
                Content = _recordListFactory.FromIdentifiers(recordReferences)
            });
        }

        // Create asset reference tab
        if (assetReferences.Count > 0) {
            var stackPanel = new StackPanel();
            var listBox = new ListBox {
                [!Layoutable.MaxHeightProperty] = stackPanel.GetObservable(Visual.BoundsProperty).Select(b => b.Height).ToBinding(),
                Classes = { "Compact" },
                ItemsSource = assetReferences,
                ItemTemplate = new FuncDataTemplate<string>((path, _) => new TextBlock {
                    Text = path,
                    [DoubleTappedProperty.CommandProperty] = MoveTo,
                    [DoubleTappedProperty.CommandParameterProperty] = path,
                }),
            };
            stackPanel.Children.Add(listBox);
            tabs.Add(new TabItem {
                Header = "Asset",
                Content = stackPanel
            });
        }

        return tabs;
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
        if (firstDraggedAsset == null) return;

        var srcDirectory = _fileSystem.Path.GetDirectoryName(firstDraggedAsset.Path);
        if (srcDirectory == null) return;

        var relativeSrcDirectory = _fileSystem.Path.GetRelativePath(_root, srcDirectory);
        var relativeDstDirectory = _fileSystem.Path.GetRelativePath(_root, directory.Path);

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
                            new Run(_fileSystem.Path.GetRelativePath(_root, firstDraggedAsset.Path)) {
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

        var referenceTabs = GetReferenceTabs(draggedAssets);
        if (referenceTabs.Count > 0) {
            var referenceList = new ReferenceList(referenceTabs);
            content.Children.Add(new TextBlock { Text = "Do you really want to proceed? These references will be modified to point to the new path." });
            content.Children.Add(referenceList);
        }

        // Show a confirmation dialog
        var moveDialog = CreateAssetDialog(draggedAssets, "Confirm", content);
        if (await moveDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            // Move all assets and remap their references
            foreach (var asset in draggedAssets) MoveAndRemapReferences(asset, directory.Path);

        }
    }

    private void MoveAndRemapReferences(AssetTreeItem assetTreeItem, string destination) {
        var basePath = assetTreeItem.Path;
        var destinationIsFile = _fileSystem.Path.HasExtension(destination);

        foreach (var childOrSelf in assetTreeItem.GetAllChildren(a => a.Children, true)) {
            if (childOrSelf.Asset is not AssetFile file) continue;

            // Calculate the full path of that the file should be moved to
            var fullNewPath = destinationIsFile
                // If the destination is a file, we already have the full path
                ? destination
                // Otherwise, combine the destination directory with the rest of the path to the file
                // Example meshes\clutter to meshes\clutter-new
                // basePath:          meshes\clutter
                // childOrSelf.Path:  meshes\clutter\test.nif
                // destination:       meshes\clutter-new\
                // fullNewPath:       meshes\clutter-new\test.nif
                : _fileSystem.Path.Combine(
                    destination,
                    string.Equals(basePath, childOrSelf.Path, AssetCompare.PathComparison)
                        ? _fileSystem.Path.GetFileName(basePath)
                        : _fileSystem.Path.GetRelativePath(basePath, childOrSelf.Path));

            var dataRelativePath = _fileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, fullNewPath);

            // Path without the base folder prefix
            // Change meshes\clutter\test\test.nif to clutter\test.nif
            var shortenedPath = _fileSystem.Path.GetRelativePath(file.ReferencedAsset.AssetLink.Type.BaseFolder, dataRelativePath);

            // Move the asset
            _assetController.Move(childOrSelf.Path, fullNewPath);

            // Remap references in records
            foreach (var formLink in file.ReferencedAsset.RecordReferences) {
                if (!_editorEnvironment.LinkCache.TryResolve(formLink, out var record)) continue;

                var recordOverride = _recordController.GetOrAddOverride(record);
                recordOverride.RemapListedAssetLinks(new Dictionary<IAssetLinkGetter, string>(AssetLinkEqualityComparer.Instance) { { file.ReferencedAsset.AssetLink, shortenedPath } });
            }

            // Remap references in NIFs
            foreach (var reference in file.ReferencedAsset.NifReferences) {
                var fullPath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, reference);
                _modelModificationService.RemapReferences(fullPath, path => !path.IsNullOrWhitespace() && childOrSelf.Path.EndsWith(path, AssetCompare.PathComparison), dataRelativePath);
            }
        }
    }

    public void ContextMenu(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control { DataContext: AssetTreeItem asset } control) return;

        var contextFlyout = new MenuFlyout {
            Items = {
                new MenuItem {
                    Icon = new SymbolIcon { Symbol = Symbol.OpenFile },
                    Header = "Open",
                    Command = Open,
                    CommandParameter = AssetTreeSource.RowSelection!.SelectedItems
                },
                new MenuItem {
                    Icon = new SymbolIcon { Symbol = Symbol.Rename },
                    Header = "Rename",
                    InputGesture = new KeyGesture(Key.F2),
                    HotKey = new KeyGesture(Key.F2),
                    Command = Rename,
                    CommandParameter = asset
                },
                new MenuItem {
                    Icon = new SymbolIcon { Symbol = Symbol.Delete },
                    Header = "Delete",
                    InputGesture = new KeyGesture(Key.Delete),
                    HotKey = new KeyGesture(Key.Delete),
                    Command = Delete,
                    CommandParameter = AssetTreeSource.RowSelection!.SelectedItems
                },
                new MenuItem {
                    Icon = new SymbolIcon { Symbol = Symbol.List },
                    Header = "Open References",
                    InputGesture = new KeyGesture(Key.R, KeyModifiers.Control),
                    HotKey = new KeyGesture(Key.R, KeyModifiers.Control),
                    Command = OpenReferences,
                    CommandParameter = asset
                },
            }
        };

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
                ItemTemplate = new FuncDataTemplate<AssetTreeItem>((asset, _) => new TextBlock { Text = _fileSystem.Path.GetRelativePath(_root, asset.Path) }),
            };
        }

        return dialog;
    }
}
