using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public sealed class GetFactionRankDataTemplate : ICustomConditionValueDataTemplate {
    private static readonly FuncDataTemplate<IRankGetter> RankDataTemplate = new((r, _) => new TextBlock {
        Text = r.Title?.GetName(s => s?.String) ?? r.Number.ToString() ?? string.Empty,
    });

    public bool Match(Condition.Function function) => function == Condition.Function.GetFactionRank;

    public IObservable<Control?> Build(
        ILinkCache linkCache,
        EditableCondition editableCondition,
        IConditionDataGetter data,
        IObservable<Unit>? conditionDataChanged) {
        if (conditionDataChanged is null) return Observable.Empty<Control?>();
        if (data is not IGetFactionRankConditionDataGetter getFactionRank) return Observable.Empty<Control?>();

        return conditionDataChanged
            .Select<Unit, Control>(_ => {
                var faction = getFactionRank.Faction.Link.TryResolve(linkCache);
                if (faction is null || faction.Ranks.Count == 0) {
                    return new TextBlock {
                        Text = "No ranks available!",
                    };
                }

                return new ComboBox {
                    DataContext = editableCondition,
                    ItemsSource = faction.Ranks,
                    ItemTemplate = RankDataTemplate,
                    [!SelectingItemsControl.SelectedItemProperty] = new Binding(nameof(EditableCondition.FloatValue)) {
                        Converter = new ExtendedFuncValueConverter<float, IRankGetter?, object?>(
                            (value, _) => {
                                var uInt32 = Convert.ToUInt32(value);
                                return faction.Ranks.FirstOrDefault(rank => rank.Number == uInt32);
                            },
                            (rank, _) => rank is not null ? Convert.ToSingle(rank.Number) : 0
                        ),
                        Mode = BindingMode.TwoWay,
                    },
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
            });
    }
}
