using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Services.Mutagen.References.Record;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class EnchantableFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _schoolOfMagicSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.SchoolOfMagic).Selector;

    private readonly Func<IMajorRecordGetter, object?> _enchantmentSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.Enchantment).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Enchantment",
                    CellTemplate = CellTemplate(_enchantmentSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _enchantmentSelector(x.Record)),
                },
                142),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "School of Magic",
                    CellTemplate = CellTemplate(_schoolOfMagicSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _schoolOfMagicSelector(x.Record)),
                },
                141),
        ];
    }

    private static FuncDataTemplate<IReferencedRecord> CellTemplate(Func<IMajorRecordGetter, object?> selector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            Name = "CellTextBlock",
            [!TextBlock.TextProperty] = record.WhenAnyValue(x => x.Record)
                .Select(r => selector(r)?.ToString())
                .ToBinding()
        });
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IEnchantableGetter));
}
