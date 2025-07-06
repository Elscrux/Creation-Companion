using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using FluentAvalonia.UI.Controls;
using LeveledList.Model.List;
using LeveledList.Model.Tier;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace LeveledList.Services.Record.Actions;

public sealed class TierRecordActionProvider : IRecordActionsProvider {
    private readonly ILeveledListRecordTypeProvider _leveledListRecordTypeProvider;
    private readonly ITierController _tierController;
    private readonly IList<RecordAction> _actions;
    private readonly ReactiveCommand<(RecordListContext Context, TierIdentifier Tier), Unit> _addToTierCommand;

    public TierRecordActionProvider(
        ILeveledListRecordTypeProvider leveledListRecordTypeProvider,
        ITierController tierController) {
        _leveledListRecordTypeProvider = leveledListRecordTypeProvider;
        _tierController = tierController;

        _addToTierCommand = ReactiveCommand.Create<(RecordListContext Context, TierIdentifier Tier)>(x => {
            foreach (var record in x.Context.SelectedRecords) {
                _tierController.SetRecordTier(record.Record, x.Tier);
            }
        });

        _actions = [
            new RecordAction(
                context => context.SelectedRecords.All(record => record.Record is IItemGetter) && GetMenuItems(context).Any(),
                0,
                RecordActionGroup.Misc,
                null,
                context => new MenuItem {
                    Header = "Add to Tier",
                    Icon = new SymbolIcon { Symbol = Symbol.Add },
                    ItemsSource = GetMenuItems(context),
                }),
            new RecordAction(
                context => context.SelectedRecords.All(record => record.Record is IItemGetter) && GetMenuItems(context).Any(),
                0,
                RecordActionGroup.Misc,
                null,
                context => new MenuItem {
                    Header = "Remove from Tier",
                    Icon = new SymbolIcon { Symbol = Symbol.Remove },
                    Command = ReactiveCommand.Create(() => {
                        foreach (var record in context.SelectedRecords) {
                            _tierController.RemoveRecordTier(record.Record);
                        }
                    })
                }),
        ];
    }

    private IEnumerable<MenuItem> GetMenuItems(RecordListContext context) {
        if (context.SelectedRecords.Count == 0) return [];

        var listRecordType = _leveledListRecordTypeProvider.GetListRecordType(context.SelectedRecords[0].Record);
        if (listRecordType is null || context.SelectedRecords.Any(r => _leveledListRecordTypeProvider.GetListRecordType(r.Record) != listRecordType)) return [];

        var tiers = _tierController.GetTiers(listRecordType.Value);
        var tierGroup = new TierGroup(string.Empty, tiers, ListDefinition.TierGroupSeparator);
        return MenuItems(tierGroup, string.Empty);

        IEnumerable<MenuItem> MenuItems(TierGroup group, string prefix) {
            foreach (var subGroup in group.GetGroups()) {
                yield return new MenuItem {
                    Header = subGroup.GroupIdentifier,
                    ItemsSource = MenuItems(subGroup, JoinPrefix(subGroup.GroupIdentifier)),
                };
            }

            foreach (var item in group.GetItems()) {
                yield return new MenuItem {
                    Header = item,
                    Command = _addToTierCommand,
                    CommandParameter = (Context: context, Tier: JoinPrefix(item)),
                };
            }

            string JoinPrefix(string subPrefix) {
                return string.IsNullOrEmpty(prefix)
                    ? subPrefix
                    : prefix + ListDefinition.TierGroupSeparator + subPrefix;
            }
        }
    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
