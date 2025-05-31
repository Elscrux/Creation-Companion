using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Services.DataSource;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Asset.Browser;

public partial class AssetBrowser : ReactiveUserControl<IAssetBrowserVM> {
    public AssetBrowser() {
        InitializeComponent();
    }

    public AssetBrowser(IAssetBrowserVM assetBrowserVM) : this() {
        ViewModel = assetBrowserVM;
        this.WhenActivated(d => {
            AssetTree.ContextRequested += ContextRequestHandler;
            d.Add(Disposable.Create(() => AssetTree.ContextRequested -= ContextRequestHandler));
        });
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (ViewModel is null) return;

        var anyExtraSelected = ViewModel.WhenAnyValue(x => x.ShowBehaviors)
            .CombineLatest(
                ViewModel.WhenAnyValue(x => x.ShowBodyTextures),
                ViewModel.WhenAnyValue(x => x.ShowDeformedModels),
                ViewModel.WhenAnyValue(x => x.ShowInterfaces),
                ViewModel.WhenAnyValue(x => x.ShowSeq),
                ViewModel.WhenAnyValue(x => x.ShowTranslations),
                (x1, x2, x3, x4, x5, x6) => x1 || x2 || x3 || x4 || x5 || x6);

        ExtraModel[!BackgroundProperty] = anyExtraSelected
            .Select(anySelected => anySelected
                ? StandardBrushes.GetBrush("ToggleButtonBackgroundChecked")
                : StandardBrushes.GetBrush("ToggleButtonBackground"))
            .ToBinding();

        ExtraModel[!ForegroundProperty] = anyExtraSelected
            .Select(anySelected => anySelected
                ? Brushes.Black
                : Brushes.White)
            .ToBinding();

        TextureButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Texture, "Texture");
        ModelButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Model, "Model");
        ScriptSourcesButton.Content = GetControl(ViewModel.AssetTypeService.Provider.ScriptSource, "Script Source");
        ScriptsButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Script, "Script");
        SoundsButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Sound, "Sound");
        MusicButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Music, "Music");
        BehaviorButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Behavior, "Behavior");
        BodyTextureButton.Content = GetControl(ViewModel.AssetTypeService.Provider.BodyTexture, "Body Texture");
        DeformedModelButton.Content = GetControl(ViewModel.AssetTypeService.Provider.DeformedModel, "Deformed Model");
        SeqButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Seq, "Seq");
        InterfaceButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Interface, "Interface");
        TranslationButton.Content = GetControl(ViewModel.AssetTypeService.Provider.Translation, "Translation");

        Control GetControl(IAssetType assetType, string name) {
            var icon = ViewModel.AssetIconService.GetIcon(assetType);
            if (icon is FontIcon fontIcon) fontIcon.FontSize = 20;
            return new StackPanel {
                Height = 20,
                Orientation = Orientation.Horizontal,
                Spacing = 2,
                Children = {
                    icon,
                    new TextBlock { Text = name, VerticalAlignment = VerticalAlignment.Center },
                }
            };
        }
    }

    private void ContextRequestHandler(object? sender, ContextRequestedEventArgs e) {
        if (e.Source is not Control { DataContext: FileSystemLink asset } control) return;

        var contextFlyout = new MenuFlyout {
            ItemsSource = ViewModel?.GetContextMenuItems(asset),
        };

        contextFlyout.ShowAt(control, true);

        e.Handled = true;
    }

    private void AssetTree_OnRowDragStarted(object? sender, TreeDataGridRowDragStartedEventArgs e) {
        e.AllowedEffects = e.Models.ToArray().OfType<FileSystemLink>().Any(x => x.DataSource.IsReadOnly)
            ? DragDropEffects.None
            : DragDropEffects.Move;
    }

    private void AssetTree_OnRowDragOver(object? sender, TreeDataGridRowDragEventArgs e) {
        e.Inner.DragEffects = DragDropEffects.Move;

        if (e.TargetRow.Model is FileSystemLink { IsDirectory: true } directoryLink
         && e.Position is not (TreeDataGridRowDropPosition.After or TreeDataGridRowDropPosition.Before)) {
            if (e.Inner.Data.Get(DragInfo.DataFormat) is DragInfo dragInfo) {
                var assets = dragInfo.Indexes
                    .Select(indexPath => {
                        var rowIndex = dragInfo.Source.Rows.ModelIndexToRowIndex(indexPath);
                        var row = dragInfo.Source.Rows[rowIndex];
                        return row.Model;
                    })
                    .OfType<FileSystemLink>();

                if (assets.Any(asset => asset == directoryLink || asset.ParentDirectory == directoryLink)) {
                    e.Inner.DragEffects = DragDropEffects.None;
                }
            }
        } else {
            e.Inner.DragEffects = DragDropEffects.None;
        }
    }

    private void AssetTree_OnRowDrop(object? sender, TreeDataGridRowDragEventArgs e) {
        // Never allow the tree to drop things, changes are handled by file system watchers
        e.Inner.DragEffects = DragDropEffects.None;

        if (e.TargetRow.Model is not FileSystemLink { IsDirectory: true } directoryLink) return;
        if (e.Inner.Data.Get(DragInfo.DataFormat) is not DragInfo dragInfo) return;

        ViewModel?.Drop(directoryLink, dragInfo);
    }

    private async void AssetTree_OnKeyDown(object? sender, KeyEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        switch (e.Key) {
            // Focus search box
            case Key.F when (e.KeyModifiers & KeyModifiers.Control) != 0:
                SearchBox.Focus();
                break;
            // Open references
            case Key.R when (e.KeyModifiers & KeyModifiers.Control) != 0:
                if (AssetTree.RowSelection.SelectedItem is FileSystemLink item) {
                    if (!await ViewModel.OpenReferences.CanExecute) break;

                    await ViewModel.OpenReferences.Execute(item);
                }
                break;
            // Rename
            case Key.F2:
                if (AssetTree.RowSelection.SelectedItem is FileSystemLink fileSystemLink) {
                    if (!await ViewModel.Rename.CanExecute) break;

                    await ViewModel.Rename.Execute(fileSystemLink);
                }
                break;
            // Delete
            case Key.Delete:
                await ViewModel.Delete.Execute(AssetTree.RowSelection.SelectedItems.OfType<FileSystemLink?>().ToList());
                break;
        }
    }

    private async void AssetTree_OnDoubleTapped(object? sender, TappedEventArgs e) {
        if (AssetTree.RowSelection is null || ViewModel is null) return;

        // Ignore double taps on the chevron
        if (e.Source is Visual v && v.GetVisualChildren().Any(x => x.Name == "ChevronPath")) return;

        if (AssetTree.RowSelection.SelectedItem is FileSystemLink { IsFile: true }) {
            await ViewModel.Open.Execute(AssetTree.RowSelection.SelectedItems.OfType<FileSystemLink>().ToList());
        } else {
            if (e.Source is not Visual visual) return;

            var expanderCell = visual.FindAncestorOfType<TreeDataGridExpanderCell>();
            if (expanderCell is not null) {
                expanderCell.IsExpanded = !expanderCell.IsExpanded;
            }
        }
    }
}
