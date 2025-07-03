using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using FluentAvalonia.UI.Controls;
using LeveledList.Model.Tier;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace LeveledList.Services.Record.Actions;

public sealed class TierRecordActionProvider : IRecordActionsProvider {
    private readonly IRecordTypeProvider _recordTypeProvider;
    private readonly ITierController _tierController;
    private readonly IList<RecordAction> _actions;
    private readonly ReactiveCommand<(RecordListContext Context, TierIdentifier Tier), Unit> _addToTierCommand;

    public TierRecordActionProvider(
        IRecordDecorationController recordDecorationController,
        IRecordTypeProvider recordTypeProvider,
        ITierController tierController) {
        _recordTypeProvider = recordTypeProvider;
        _tierController = tierController;

        _addToTierCommand = ReactiveCommand.Create<(RecordListContext Context, TierIdentifier Tier)>(x => {
            foreach (var record in x.Context.SelectedRecords) {
                recordDecorationController.Update(record.Record.ToFormLinkInformation(), new Tier(x.Tier));
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

        var listRecordType = _recordTypeProvider.GetListRecordType(context.SelectedRecords[0].Record);
        if (listRecordType is null || context.SelectedRecords.Any(r => _recordTypeProvider.GetListRecordType(r.Record) != listRecordType)) return [];

        const char separator = '/';
        var tiers = _tierController.GetTiers(listRecordType.Value);
        var tierGroup = new TierGroup(string.Empty, tiers, separator);
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
                    : prefix + separator + subPrefix;
            }
        }
    }

    public IEnumerable<RecordAction> GetActions() => _actions;
}
