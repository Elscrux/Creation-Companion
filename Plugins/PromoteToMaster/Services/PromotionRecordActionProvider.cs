using CreationEditor;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Skyrim.Definitions;
using FluentAvalonia.UI.Controls;
using PromoteToMaster.ViewModels;
using PromoteToMaster.Views;
using ReactiveUI;
namespace PromoteToMaster.Services;

public sealed class PromotionRecordActionProvider : IRecordActionsProvider {
    private readonly IList<RecordAction> _actions;
    public PromotionRecordActionProvider(
        ILinkCacheProvider linkCacheProvider,
        IMenuItemProvider menuItemProvider,
        Func<IReadOnlyList<IReferencedRecord>, PromoteToMasterVM> promoteToMasterVMFactory,
        MainWindow mainWindow) {

        var promoteCommand = ReactiveCommand.Create<RecordListContext>(context => {
            var referenceWindow = new PromotionWindow {
                DataContext = promoteToMasterVMFactory(context.SelectedRecords)
            };

            referenceWindow.Show(mainWindow);
        });

        _actions = [
            new RecordAction(
                // Only visible if any selected record has a master that is not a vanilla master
                context => {
                    return context.SelectedRecords
                        .Any(record => {
                            var modKey = record.FormKey.ModKey;
                            return linkCacheProvider
                                .LinkCache.GetMod(modKey)
                                .MasterReferences.Any(master => !SkyrimDefinitions.SkyrimModKeys.Contains(master.Master));
                        });
                },
                0,
                RecordActionGroup.Modification,
                promoteCommand,
                context => menuItemProvider.Custom(
                    promoteCommand,
                    "Promote to Master",
                    context,
                    Symbol.Up),
                IsPrimary: true)
        ];
    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
