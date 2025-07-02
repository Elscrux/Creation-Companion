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

public sealed class WeaponFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _weaponTypeSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.WeaponType).Selector;

    private readonly Func<IMajorRecordGetter, object?> _magicLevelSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.MagicLevel).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Weapon Type",
                    CellTemplate = CellTemplate(_weaponTypeSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _weaponTypeSelector(x.Record)),
                },
                147),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Magic Level",
                    CellTemplate = CellTemplate(_magicLevelSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _magicLevelSelector(x.Record)),
                },
                140),
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

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IWeaponGetter));
}
