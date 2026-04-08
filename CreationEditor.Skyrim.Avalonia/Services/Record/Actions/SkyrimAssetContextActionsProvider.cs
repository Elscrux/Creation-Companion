using System.IO.Abstractions;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Media;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Reference;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Reference;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Key = Avalonia.Input.Key;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public partial class SkyrimAssetContextActionsProvider : IContextActionsProvider, IAssetContextActionsProvider {
    private readonly IReferenceBrowserVMFactory _referenceBrowserVMFactory;
    private readonly MainWindow _mainWindow;
    private readonly ITaskDialogProvider _taskDialogProvider;
    private readonly IAssetController _assetController;
    private readonly IReferenceService _referenceService;
    private readonly IList<ContextAction> _actions;

    public SkyrimAssetContextActionsProvider(
        IReferenceBrowserVMFactory referenceBrowserVMFactory,
        IFileSystem fileSystem,
        MainWindow mainWindow,
        IDockFactory dockFactory,
        ITaskDialogProvider taskDialogProvider,
        IAssetController assetController,
        IReferenceService referenceService,
        IMenuItemProvider menuItemProvider) {
        _referenceBrowserVMFactory = referenceBrowserVMFactory;
        _mainWindow = mainWindow;
        _taskDialogProvider = taskDialogProvider;
        _assetController = assetController;
        _referenceService = referenceService;

        var goToAsset = ReactiveCommand.CreateFromTask<SelectedListContext>(async context => {
            if (context.SelectedRecords[0].ReferencedRecord.Record is not IModeledGetter { Model.File.DataRelativePath: var dataRelativePath }) return;

            var assetBrowser = await dockFactory.GetOrOpenDock(DockElement.AssetBrowser);
            if (assetBrowser.DataContext is not IAssetBrowserVM assetBrowserVM) return;

            await assetBrowserVM.MoveTo.Execute(dataRelativePath);
        });

        _actions = [
            new ContextAction(
                context => context is
                        { SelectedRecords: [{ ReferencedRecord.Record: IModeledGetter { Model.File.DataRelativePath: var rawFilePath } }], SelectedAssets.Count: 0 }
                 && !string.IsNullOrWhiteSpace(rawFilePath.Path),
                50,
                ContextActionGroup.Linking,
                goToAsset,
                context => {
                    var dataRelativePath = (context.SelectedRecords[0].ReferencedRecord.Record as IModeledGetter)!.Model!.File.DataRelativePath;

                    return menuItemProvider.Custom(
                        goToAsset,
                        $"Go to {fileSystem.Path.GetFileName(dataRelativePath.Path)}",
                        context,
                        Symbol.Go);
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
                    Symbol.NewFolder,
                    new KeyGesture(Key.N, KeyModifiers.Control | KeyModifiers.Shift))),
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
        ];
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
        if (await renameDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
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
        if (await moveDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
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

        if (await deleteDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
            foreach (var asset in deleteAssets) {
                _assetController.Delete(asset);
            }
        }
    }

    [ReactiveCommand]
    private async Task AddFolder(SelectedListContext context) {
        if (context.SelectedAssets is not [{ DataSourceLink: DataSourceDirectoryLink dir }]) return;

        await AddFolder(dir);
    }

    public async Task AddFolder(DataSourceDirectoryLink dir) {
        var textBox = new TextBox { Text = string.Empty, Watermark = "New folder" };
        var relativePath = dir.DataRelativePath.Path;
        var folderDialog = CreateAssetDialog($"Add new Folder at {relativePath}", textBox);

        if (await folderDialog.ShowAsync(true) is TaskDialogStandardResult.OK) {
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

    private TaskDialog CreateAssetDialog(string header, Control? content = null) {
        return _taskDialogProvider.CreateTaskDialog(
            header,
            content,
            assetDialog => {
                if (!_referenceService.IsLoadingCurrently) return;

                assetDialog.IconSource = new SymbolIconSource { Symbol = Symbol.ReportHacked };
                assetDialog.SubHeader += "Warning: This change might break something - Not all references have been loaded yet.";
            });
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
