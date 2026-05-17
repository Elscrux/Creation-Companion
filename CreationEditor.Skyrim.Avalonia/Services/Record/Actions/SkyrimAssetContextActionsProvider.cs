using System.Collections;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Prefix;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Basic;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.SourceGenerators;
using Key = Avalonia.Input.Key;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public partial class SkyrimAssetContextActionsProvider : IContextActionsProvider, IAssetContextActionsProvider {
    private readonly IReferenceBrowserVMFactory _referenceBrowserVMFactory;
    private readonly IAssetTypeProvider _assetTypeProvider;
    private readonly MainWindow _mainWindow;
    private readonly IDockFactory _dockFactory;
    private readonly IModelModificationService _modelModificationService;
    private readonly ITaskDialogProvider _taskDialogProvider;
    private readonly IRecordPrefixService _recordPrefixService;
    private readonly IAssetController _assetController;
    private readonly IRecordController _recordController;
    private readonly IReferenceService _referenceService;
    private readonly IList<ContextAction> _actions;

    public SkyrimAssetContextActionsProvider(
        IReferenceBrowserVMFactory referenceBrowserVMFactory,
        IFileSystem fileSystem,
        MainWindow mainWindow,
        IDockFactory dockFactory,
        IModelModificationService modelModificationService,
        IAssetTypeProvider assetTypeProvider,
        ITaskDialogProvider taskDialogProvider,
        IRecordPrefixService recordPrefixService,
        IAssetController assetController,
        IRecordController recordController,
        IReferenceService referenceService,
        IMenuItemProvider menuItemProvider) {
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _assetTypeProvider = assetTypeProvider;
        _mainWindow = mainWindow;
        _dockFactory = dockFactory;
        _modelModificationService = modelModificationService;
        _taskDialogProvider = taskDialogProvider;
        _recordPrefixService = recordPrefixService;
        _assetController = assetController;
        _recordController = recordController;
        _referenceService = referenceService;

        _actions = [
            new ContextAction(
                context => context is
                        { SelectedRecords: [{ ReferencedRecord.Record: IModeledGetter { Model.File.DataRelativePath: var rawFilePath } }], SelectedAssets.Count: 0 }
                 && !string.IsNullOrWhiteSpace(rawFilePath.Path),
                50,
                ContextActionGroup.Linking,
                GoToAssetCommand,
                context => {
                    var dataRelativePath = (context.SelectedRecords[0].ReferencedRecord.Record as IModeledGetter)!.Model!.File.DataRelativePath;

                    return menuItemProvider.Custom(
                        GoToAssetCommand,
                        $"Go to {fileSystem.Path.GetFileName(dataRelativePath.Path)}",
                        context,
                        FASymbol.Go);
                }
            ),
            new ContextAction(
                context => context.SelectedAssets.Count > 0,
                50,
                ContextActionGroup.Viewing,
                OpenAssetsCommand,
                context => menuItemProvider.File(OpenAssetsCommand, context)),
            new ContextAction(
                context => context.SelectedAssets is [{ DataSourceLink: var asset }] && asset.Exists(),
                40,
                ContextActionGroup.Viewing,
                OpenInFileExplorerCommand,
                context => menuItemProvider.OpenFolder(OpenInFileExplorerCommand, context, "Open in File Explorer")),
            new ContextAction(
                context => context.SelectedAssets is [{ DataSourceLink: var asset }] && asset.Exists(),
                60,
                ContextActionGroup.Modification,
                RenameAssetCommand,
                context => menuItemProvider.Rename(RenameAssetCommand, context)),
            new ContextAction(
                context => context.SelectedAssets.Count > 0,
                59,
                ContextActionGroup.Modification,
                DeleteAssetsCommand,
                context => menuItemProvider.Delete(DeleteAssetsCommand, context)),
            new ContextAction(
                context => context is { SelectedAssets: [{ DataSourceLink: DataSourceDirectoryLink }], SelectedRecords.Count: 0 },
                50,
                ContextActionGroup.Modification,
                AddFolderCommand,
                context => menuItemProvider.Custom(
                    AddFolderCommand,
                    "Add Folder",
                    context,
                    FASymbol.NewFolder,
                    new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift))),
            new ContextAction(
                context => context.HasAnyAssetOfType(_assetTypeProvider.Model),
                10,
                ContextActionGroup.Modification,
                RemapTexturesCommand,
                context => menuItemProvider.Custom(RemapTexturesCommand, "Remap Textures", context, FASymbol.Rename)),
            new ContextAction(
                context => context is { SelectedAssets.Count: > 0, SelectedRecords.Count: 0 },
                50,
                ContextActionGroup.Copy,
                CopyAssetPathCommand,
                context => menuItemProvider.Copy(
                    CopyAssetPathCommand,
                    context,
                    "Copy Path",
                    false)),
            new ContextAction(
                context => context.HasAnyAssetOfType(_assetTypeProvider.Model),
                10,
                ContextActionGroup.Misc,
                CreateStaticRecordCommand,
                context => menuItemProvider.Custom(CreateStaticRecordCommand, "Create Static Record for Mesh", context, FASymbol.Home)),
        ];
    }

    [ReactiveCommand]
    private async Task GoToAsset(SelectedListContext context) {
        if (context.SelectedRecords[0].ReferencedRecord.Record is not IModeledGetter { Model.File.DataRelativePath: var dataRelativePath }) return;

        var assetBrowser = await _dockFactory.GetOrOpenDock(DockElement.AssetBrowser);
        if (assetBrowser.DataContext is not IAssetBrowserVM assetBrowserVM) return;

        await assetBrowserVM.MoveToPathCommand.Execute(dataRelativePath);
    }

    [ReactiveCommand]
    private async Task OpenInFileExplorer(SelectedListContext context) {
        if (context.SelectedAssets is not [{ DataSourceLink: var asset }]) return;

        Process.Start(new ProcessStartInfo {
            FileName = "explorer.exe",
            Arguments = $"/select,\"{asset.FullPath}\"",
            UseShellExecute = true,
        });
    }

    [ReactiveCommand]
    public void OpenAssets(SelectedListContext context) {
        OpenAssets(context.SelectedAssets.Select(x => x.DataSourceLink));
    }

    public void OpenAssets(IEnumerable<IDataSourceLink> dataSourceLinks) {
        foreach (var link in dataSourceLinks) {
            _assetController.Open(link);
        }
    }

    [ReactiveCommand]
    public async Task RenameAsset(SelectedListContext context) {
        if (context is not { SelectedAssets: [{ DataSourceLink: var asset }] }) return;

        await RenameAsset(asset);
    }

    public async Task RenameAsset(IDataSourceLink asset) {
        var nameWithoutExtension = asset.NameWithoutExtension;
        var textBox = new TextBox { Text = nameWithoutExtension };
        var content = new StackPanel { Children = { textBox } };

        var referenceBrowserVM = _referenceBrowserVMFactory.GetReferenceBrowserVM(asset);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content.Children.Add(new TextBlock {
                Text = "Do you really want to proceed? These references will be modified to point to the new path.",
            });
            content.Children.Add(referenceBrowser);
        }

        var renameDialog = CreateAssetDialog($"Rename {asset.Name}", content);
        if (await renameDialog.ShowAsync(true) is FATaskDialogStandardResult.OK) {
            if (string.Equals(nameWithoutExtension, textBox.Text, DataRelativePath.PathComparison)) return;

            switch (asset) {
                case DataSourceDirectoryLink directoryLink:
                    _assetController.Rename(directoryLink, textBox.Text);
                    break;
                case DataSourceFileLink fileLink:
                    _assetController.Rename(fileLink, textBox.Text + fileLink.Extension);
                    break;
            }
        }
    }

    public async Task MoveAssets(DataSourceDirectoryLink dstDirectory, params IReadOnlyList<IDataSourceLink> movingAssets) {
        var firstDraggedAsset = movingAssets.FirstOrDefault();
        if (firstDraggedAsset is null) return;

        var srcDirectory = firstDraggedAsset.ParentDirectory;
        if (srcDirectory is null) return;

        var relativeSrcDirectory = srcDirectory.DataRelativePath.Path;
        var relativeDstDirectory = dstDirectory.DataRelativePath.Path;

        // No need to move if the source and destination are the same
        if (string.Equals(relativeSrcDirectory, relativeDstDirectory, DataRelativePath.PathComparison)) return;

        var content = new StackPanel {
            Children = {
                movingAssets.Count > 1
                    ? new TextBlock {
                        Inlines = [
                            new Run("Move "),
                            new Run(movingAssets.Count.ToString()) {
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

        var referenceBrowserVM = _referenceBrowserVMFactory.GetReferenceBrowserVM(movingAssets);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content.Children.Add(new TextBlock {
                Text = "Do you really want to proceed? These references will be modified to point to the new path.",
            });
            content.Children.Add(referenceBrowser);
        }

        // Show a confirmation dialog
        var moveDialog = CreateAssetDialog(movingAssets, "Confirm", content);
        if (await moveDialog.ShowAsync(true) is FATaskDialogStandardResult.OK) {
            // Move all assets and remap their references
            foreach (var asset in movingAssets) _assetController.Move(asset, dstDirectory);
        }
    }

    [ReactiveCommand]
    public async Task DeleteAssets(SelectedListContext context) {
        var deleteAssets = context.SelectedAssets.Select(x => x.DataSourceLink).ToArray();
        await DeleteAssets(deleteAssets);
    }

    public async Task DeleteAssets(IReadOnlyList<IDataSourceLink> deleteAssets) {
        Control? content = null;

        var referenceBrowserVM = _referenceBrowserVMFactory.GetReferenceBrowserVM(deleteAssets);
        if (referenceBrowserVM is not null) {
            var referenceBrowser = new ReferenceBrowser(referenceBrowserVM);
            content = new StackPanel {
                Children = {
                    new TextBlock {
                        Text = "Do you really want to proceed? There are still references to these assets."
                    },
                    referenceBrowser,
                },
            };
        }

        var deleteDialog = deleteAssets.Count == 1
            ? CreateAssetDialog(deleteAssets, $"Delete {deleteAssets[0].Name}", content)
            : CreateAssetDialog(deleteAssets, $"Delete {deleteAssets.Count}", content);

        if (await deleteDialog.ShowAsync(true) is FATaskDialogStandardResult.OK) {
            foreach (var asset in deleteAssets) {
                _assetController.Delete(asset);
            }
        }
    }

    [ReactiveCommand]
    private async Task RemapTextures(SelectedListContext context) {
        var assets = context.SelectedAssets
            .Where(assetContext => assetContext.DataSourceLink is DataSourceDirectoryLink
             || assetContext.ReferencedAsset?.AssetLink.AssetTypeInstance == _assetTypeProvider.Model)
            .SelectMany(assetContext => assetContext.DataSourceLink.EnumerateAllFileLinks())
            .ToArray();

        if (assets.Length == 0) return;

        var textures = assets.SelectMany(a => _referenceService.GetAssetLinks(a))
            .Where(x => x.AssetTypeInstance == _assetTypeProvider.Texture)
            .Select(x => x.DataRelativePath.Path)
            .Distinct()
            .ToArray();

        var firstTexture = textures.FirstOrDefault();
        var enableRegex = new CheckBox { Content = "Enable Regex", IsChecked = false };
        var fromTextBox = new TextBox { Text = firstTexture };
        var toTextBox = new TextBox { Text = firstTexture };
        var replacementCount = new TextBlock { Foreground = StandardBrushes.HighlightBrush };
        var replacement = new Card {
            Header = "Replace Textures",
            Description = "Replace parts of the texture paths. You can use regular expressions for more complex replacements.",
            Icon = FASymbol.Rename,
            Content = new StackPanel {
                Spacing = 5,
                Children = {
                    enableRegex,
                    fromTextBox,
                    toTextBox,
                }
            }
        };
        var regexErrorText = new TextBlock {
            Foreground = Brushes.IndianRed,
            IsVisible = false,
            TextWrapping = TextWrapping.Wrap,
        };
        var texturesListBox = new ItemsControl { ItemsSource = textures, [Grid.ColumnProperty] = 0 };
        var texturesAfterListBox = new ItemsControl { ItemsSource = textures, [Grid.ColumnProperty] = 1 };

        var previewObservable = Observable
            .CombineLatest(
                fromTextBox.GetObservable(TextBox.TextProperty).Select(x => x ?? string.Empty),
                toTextBox.GetObservable(TextBox.TextProperty).Select(x => x ?? string.Empty),
                enableRegex.GetObservable(ToggleButton.IsCheckedProperty).Select(x => x is true),
                (fromValue, toValue, isRegex) => new { FromValue = fromValue, ToValue = toValue, IsRegex = isRegex })
            .Publish()
            .RefCount();

        using var previewSubscription = previewObservable.Subscribe(values => {
            if (values.IsRegex) {
                try {
                    texturesAfterListBox.ItemsSource = textures.Select(path => Regex.Replace(path, values.FromValue, values.ToValue)).ToArray();
                    regexErrorText.IsVisible = false;
                    regexErrorText.Text = string.Empty;
                } catch (ArgumentException ex) {
                    texturesAfterListBox.ItemsSource = textures;
                    regexErrorText.Text = ex.Message;
                    regexErrorText.IsVisible = true;
                }
            } else {
                texturesAfterListBox.ItemsSource = textures.Select(path => path.Replace(values.FromValue, values.ToValue, StringComparison.OrdinalIgnoreCase)).ToArray();

                regexErrorText.IsVisible = false;
                regexErrorText.Text = string.Empty;
            }

            if (texturesAfterListBox.ItemsSource is IEnumerable itemsSource) {
                var count = itemsSource
                    .OfType<string>()
                    .Index()
                    .Count(x => x.Item != textures.ElementAt(x.Index));

                replacementCount.Text = $"{count} textures will be replaced";
            } else {
                replacementCount.Text = string.Empty;
            }
        });

        var content = new DockPanel {
            Children = {
                new StackPanel {
                    [DockPanel.DockProperty] = Dock.Top,
                    Children = {
                        replacement,
                        regexErrorText,
                        replacementCount,
                    }
                },
                new ScrollViewer {
                    Height = 440,
                    Content = new Grid {
                        ColumnDefinitions = new ColumnDefinitions("*,*"),
                        Children = { texturesListBox, texturesAfterListBox }
                    }
                }
            }
        };

        var target = assets is [{ Name: var name }] ? name : $"{assets.Length} assets";
        var remapDialog = CreateAssetDialog($"Remap textures in {target}", content);
        if (await remapDialog.ShowAsync(true) is FATaskDialogStandardResult.OK) {
            foreach (var fileLink in assets) {
                _modelModificationService.RemapLinks(
                    fileLink,
                    path => {
                        if (enableRegex.IsChecked is true) {
                            return Regex.Replace(path, fromTextBox.Text ?? string.Empty, toTextBox.Text ?? string.Empty);
                        }

                        return path.Replace(fromTextBox.Text ?? string.Empty, toTextBox.Text ?? string.Empty, StringComparison.OrdinalIgnoreCase);
                    });
            }
        }
    }

    [ReactiveCommand]
    private async Task AddFolder(SelectedListContext context) {
        if (context.SelectedAssets is not [{ DataSourceLink: DataSourceDirectoryLink dir }]) return;

        await AddFolder(dir);
    }

    public async Task AddFolder(DataSourceDirectoryLink dir) {
        var textBox = new TextBox { Text = string.Empty, PlaceholderText = "New folder" };
        var relativePath = dir.DataRelativePath.Path;
        var folderDialog = CreateAssetDialog($"Add new Folder at {relativePath}", textBox);

        if (await folderDialog.ShowAsync(true) is FATaskDialogStandardResult.OK) {
            var newFolder = dir.FileSystem.Path.Combine(dir.FullPath, textBox.Text);
            dir.FileSystem.Directory.CreateDirectory(newFolder);
        }
    }

    [ReactiveCommand]
    private async Task CopyAssetPath(SelectedListContext context) {
        if (context is not { SelectedAssets: [{ DataSourceLink: {} dataSourceLink }] }) return;

        await CopyAssetPath(dataSourceLink);
    }

    public async Task CopyAssetPath(IDataSourceLink dataSourceLink) {
        var clipboard = TopLevel.GetTopLevel(_mainWindow)?.Clipboard;
        if (clipboard is not null) {
            await clipboard.SetTextAsync(dataSourceLink.DataRelativePath.Path);
        }
    }

    [ReactiveCommand]
    private async Task CreateStaticRecord(SelectedListContext context) {
        foreach (var asset in context.SelectedAssets.Where(c => c.ReferencedAsset?.AssetLink.AssetTypeInstance == _assetTypeProvider.Model)) {
            var staticRecord = _recordController.CreateRecord<Static, IStaticGetter>();
            _recordController.RegisterUpdate(staticRecord,
                () => {
                    staticRecord.Model ??= new Model();
                    staticRecord.Model.File = asset.DataSourceLink.DataRelativePath;
                    staticRecord.EditorID = _recordPrefixService.ApplyPrefix(asset.DataSourceLink.NameWithoutExtension);
                });
        }
    }

    private FATaskDialog CreateAssetDialog(IEnumerable<IDataSourceLink> footerAssets, string header, Control? content = null) {
        var dialog = CreateAssetDialog(header, content);

        var fileLinks = footerAssets.ToArray();
        if (fileLinks.Length > 1) {
            dialog.FooterVisibility = FATaskDialogFooterVisibility.Auto;
            dialog.Footer = new ItemsControl {
                ItemsSource = fileLinks,
                ItemTemplate = new FuncDataTemplate<IDataSourceLink>((asset, _) => new TextBlock { Text = asset.DataRelativePath.Path }),
            };
        }

        return dialog;
    }

    private FATaskDialog CreateAssetDialog(string header, Control? content = null) {
        return _taskDialogProvider.CreateTaskDialog(
            header,
            content,
            assetDialog => {
                assetDialog.Classes.Add("AssetDialog");
                assetDialog.Styles.Add(new Style(x => x.OfType<FATaskDialog>().Class("AssetDialog").Template().OfType<Border>().Name("ContentRoot")) {
                        Setters = {
                            new Setter(Layoutable.MinWidthProperty, 1000.0),
                        },
                    }
                );

                if (!_referenceService.IsLoadingCurrently) return;

                assetDialog.IconSource = new FASymbolIconSource { Symbol = FASymbol.ReportHacked };
                assetDialog.SubHeader += "Warning: This change might break something - Not all references have been loaded yet.";
            });
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
