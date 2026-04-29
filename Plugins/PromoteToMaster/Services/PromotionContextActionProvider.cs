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
using ReactiveUI.SourceGenerators;
namespace PromoteToMaster.Services;

public sealed partial class PromotionContextActionProvider : IContextActionsProvider {
    private readonly Func<IReadOnlyList<IReferencedRecord>, PromoteToMasterVM> _promoteToMasterVMFactory;
    private readonly MainWindow _mainWindow;
    private readonly IList<ContextAction> _actions;
    public PromotionContextActionProvider(
        ILinkCacheProvider linkCacheProvider,
        IMenuItemProvider menuItemProvider,
        Func<IReadOnlyList<IReferencedRecord>, PromoteToMasterVM> promoteToMasterVMFactory,
        MainWindow mainWindow) {
        _promoteToMasterVMFactory = promoteToMasterVMFactory;
        _mainWindow = mainWindow;

        _actions = [
            new ContextAction(
                // Only visible if any selected record has a master that is not a vanilla master
                context => {
                    if (context.SelectedAssets.Count > 0) return false;

                    return context.SelectedRecords
                        .Any(record => {
                            var modKey = record.ReferencedRecord.FormKey.ModKey;
                            return linkCacheProvider
                                .LinkCache.GetMod(modKey)
                                .MasterReferences.Any(master => !SkyrimDefinitions.SkyrimModKeys.Contains(master.Master));
                        });
                },
                0,
                ContextActionGroup.Modification,
                PromoteCommand,
                context => menuItemProvider.Custom(
                    PromoteCommand,
                    "Promote to Master",
                    context,
                    Symbol.Up),
                IsPrimary: true),
        ];
    }

    [ReactiveCommand]
    private void Promote(SelectedListContext context) {
        var referenceWindow = new PromotionWindow {
            DataContext = _promoteToMasterVMFactory(context.SelectedRecords.Select(x => x.ReferencedRecord).ToArray()),
        };

        referenceWindow.Show(_mainWindow);
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
