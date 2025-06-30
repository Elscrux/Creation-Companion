using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using FluentAvalonia.UI.Controls;
using LeveledList.Model;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace LeveledList.Services;

public sealed class TierRecordActionProvider : IRecordActionsProvider {
    private readonly ITierController _tierController;
    private readonly IList<RecordAction> _actions;
    private readonly ReactiveCommand<(RecordListContext Context, TierIdentifier Tier), Unit> _addToTierCommand;

    public TierRecordActionProvider(
        IRecordDecorationController recordDecorationController,
        ITierController tierController) {
        _tierController = tierController;

        _addToTierCommand = ReactiveCommand.Create<(RecordListContext Context, TierIdentifier Tier)>(x => {
            foreach (var record in x.Context.SelectedRecords) {
                recordDecorationController.Update(record, new Tier(x.Tier));
            }
        });

        _actions = [
            new RecordAction(
                context => context.SelectedRecords.All(record => record.Record is IItemGetter),
                0,
                RecordActionGroup.Misc,
                null,
                context => new MenuItem {
                    Header = "Add to Tier",
                    Icon = new SymbolIcon { Symbol = Symbol.Add },
                    ItemsSource = GetMenuItems(context),
                }),
        ];
    }

    private IEnumerable<MenuItem> GetMenuItems(RecordListContext context) {
        if (context.SelectedRecords.Count == 0) return [];

        var firstType = context.SelectedRecords[0].Type;
        if (context.SelectedRecords.Any(r => r.Type != firstType)) return [];

        var tiers = _tierController.GetTiers(firstType);
        var tierGroup = new TierGroup(string.Empty, tiers, '/');
        return MenuItems(tierGroup);

        IEnumerable<MenuItem> MenuItems(TierGroup group) {
            foreach (var subGroup in group.GetGroups()) {
                yield return new MenuItem {
                    Header = subGroup.GroupIdentifier,
                    ItemsSource = MenuItems(subGroup),
                };
            }

            foreach (var item in group.GetItems()) {
                yield return new MenuItem {
                    Header = item,
                    Command =  _addToTierCommand,
                    CommandParameter = (Context: context, Tier: item)
                };
            }
        }
    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
