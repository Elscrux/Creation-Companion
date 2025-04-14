using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Actions;

public class AssetRecordActionsProvider : IRecordActionsProvider {
    private readonly IList<RecordAction> _actions;

    public AssetRecordActionsProvider(
        IFileSystem fileSystem,
        IDockFactory dockFactory,
        IMenuItemProvider menuItemProvider) {

        var goToAsset = ReactiveCommand.CreateFromTask<RecordListContext>(async context => {
            if (context.SelectedRecords[0].Record is not IModeledGetter { Model.File.DataRelativePath: var dataRelativePath }) return;

            var assetBrowser = await dockFactory.GetOrOpenDock(DockElement.AssetBrowser);
            if (assetBrowser.DataContext is not IAssetBrowserVM assetBrowserVM) return;

            await assetBrowserVM.MoveTo.Execute(dataRelativePath);
        });

        _actions = [
            new RecordAction(
                context => context.SelectedRecords is [{ Record: IModeledGetter { Model.File.DataRelativePath: var rawFilePath } }]
                 && !string.IsNullOrWhiteSpace(rawFilePath.Path),
                50,
                RecordActionGroup.Linking,
                goToAsset,
                context => {
                    var dataRelativePath = (context.SelectedRecords[0].Record as IModeledGetter)!.Model!.File.DataRelativePath;

                    return menuItemProvider.Custom(
                        goToAsset,
                        $"Go to {fileSystem.Path.GetFileName(dataRelativePath.Path)}",
                        context,
                        Symbol.Go);
                }
            ),
        ];
    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
