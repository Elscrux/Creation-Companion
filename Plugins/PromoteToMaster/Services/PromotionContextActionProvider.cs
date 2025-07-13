using CreationEditor;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Definitions;
using FluentAvalonia.UI.Controls;
using PromoteToMaster.ViewModels;
using PromoteToMaster.Views;
using ReactiveUI;
namespace PromoteToMaster.Services;

public sealed class PromotionContextActionProvider : IContextActionsProvider {
    private readonly IList<ContextAction> _actions;
    public PromotionContextActionProvider(
        ILinkCacheProvider linkCacheProvider,
        IMenuItemProvider menuItemProvider,
        Func<IReadOnlyList<IReferencedRecord>, PromoteToMasterVM> promoteToMasterVMFactory,
        MainWindow mainWindow) {

        var promoteCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            var referenceWindow = new PromotionWindow {
                DataContext = promoteToMasterVMFactory(context.SelectedRecords),
            };

            referenceWindow.Show(mainWindow);
        });

        _actions = [
            new ContextAction(
                // Only visible if any selected record has a master that is not a vanilla master
                context => {
                    if (context.SelectedAssets.Count > 0) return false;

                    return context.SelectedRecords
                        .Any(record => {
                            var modKey = record.FormKey.ModKey;
                            return linkCacheProvider
                                .LinkCache.GetMod(modKey)
                                .MasterReferences.Any(master => !SkyrimDefinitions.SkyrimModKeys.Contains(master.Master));
                        });
                },
                0,
                ContextActionGroup.Modification,
                promoteCommand,
                context => menuItemProvider.Custom(
                    promoteCommand,
                    "Promote to Master",
                    context,
                    Symbol.Up),
                IsPrimary: true),
        ];
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
