using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using Mutagen.Bethesda.Skyrim.Assets;
using NifPlugin.Views;
using ReactiveUI.SourceGenerators;
namespace NifPlugin.Services.Record.Actions;

public sealed partial class NifActionsProvider : IContextActionsProvider {
    private readonly NifVMFactory _nifVMFactory;
    private readonly IMainWindowProvider _mainWindowProvider;
    private readonly IList<ContextAction> _actions;

    public NifActionsProvider(
        NifVMFactory nifVMFactory,
        IMainWindowProvider mainWindowProvider,
        IMenuItemProvider menuItemProvider) {
        _nifVMFactory = nifVMFactory;
        _mainWindowProvider = mainWindowProvider;

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
        if (_mainWindowProvider.Current is not {} mainWindow) return;

        foreach (var asset in context.SelectedAssets) {
            var nifWindow = new NifWindow {
                DataContext = _nifVMFactory.Create(asset.DataSourceLink),
            };

            nifWindow.Show(mainWindow);
        }
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
