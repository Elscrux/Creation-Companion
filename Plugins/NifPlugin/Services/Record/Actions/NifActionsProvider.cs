using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Views;
using Mutagen.Bethesda.Skyrim.Assets;
using NifPlugin.Views;
using ReactiveUI.SourceGenerators;
namespace NifPlugin.Services.Record.Actions;

public sealed partial class NifActionsProvider : IContextActionsProvider {
    private readonly NifVMFactory _nifVMFactory;
    private readonly MainWindow _mainWindow;
    private readonly IList<ContextAction> _actions;

    public NifActionsProvider(
        NifVMFactory nifVMFactory,
        MainWindow mainWindow,
        IMenuItemProvider menuItemProvider) {
        _nifVMFactory = nifVMFactory;
        _mainWindow = mainWindow;

        _actions = [
            new ContextAction(context =>
                    context is { SelectedAssets: [{ ReferencedAsset.AssetLink.AssetTypeInstance: var assetType }] }
                 && assetType == SkyrimModelAssetType.Instance,
                50,
                ContextActionGroup.Modification,
                OpenEditMenuCommand,
                context => menuItemProvider.Edit(OpenEditMenuCommand, context, "Edit Nif")),
        ];
    }

    [ReactiveCommand]
    private void OpenEditMenu(SelectedListContext context) {
        foreach (var asset in context.SelectedAssets) {
            var nifWindow = new NifWindow {
                DataContext = _nifVMFactory.Create(asset.DataSourceLink),
            };

            nifWindow.Show(_mainWindow);
        }
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
