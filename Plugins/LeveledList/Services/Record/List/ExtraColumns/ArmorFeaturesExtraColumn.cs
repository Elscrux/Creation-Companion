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

public sealed class ArmorFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _armorTypeSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.ArmorType).Selector;

    private readonly Func<IMajorRecordGetter, object?> _armorSlotSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.ArmorSlot).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Armor Type",
                    CellTemplate = CellTemplate(_armorTypeSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _armorTypeSelector(x.Record)),
                },
                150),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Armor Slot",
                    CellTemplate = CellTemplate(_armorSlotSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _armorSlotSelector(x.Record)),
                },
                150),
        ];
    }

    private static FuncDataTemplate<IReferencedRecord> CellTemplate(Func<IMajorRecordGetter, object?> selector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            [!TextBlock.TextProperty] = record.WhenAnyValue(x => x.Record)
                .Select(r => selector(r)?.ToString())
                .ToBinding()
        });
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IArmorGetter));
}

public sealed class WeaponFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _weaponTypeSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.WeaponType).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Weapon Type",
                    CellTemplate = CellTemplate(_weaponTypeSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _weaponTypeSelector(x.Record)),
                },
                150),
        ];
    }

    private static FuncDataTemplate<IReferencedRecord> CellTemplate(Func<IMajorRecordGetter, object?> selector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            [!TextBlock.TextProperty] = record.WhenAnyValue(x => x.Record)
                .Select(r => selector(r)?.ToString())
                .ToBinding()
        });
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IWeaponGetter));
}

public sealed class EnchantableFeaturesExtraColumn(IFeatureProvider featureProvider) : IAutoAttachingExtraColumns {
    private readonly Func<IMajorRecordGetter, object?> _schoolOfMagicSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.SchoolOfMagic).Selector;

    private readonly Func<IMajorRecordGetter, object?> _enchantmentSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.Enchantment).Selector;

    private readonly Func<IMajorRecordGetter, object?> _magicLevelSelector =
        featureProvider.GetFeatureWildcard(FeatureProvider.MagicLevel).Selector;

    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "School of Magic",
                    CellTemplate = CellTemplate(_schoolOfMagicSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _schoolOfMagicSelector(x.Record)),
                },
                150),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Enchantment",
                    CellTemplate = CellTemplate(_enchantmentSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _enchantmentSelector(x.Record)),
                },
                150),
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Magic Level",
                    CellTemplate = CellTemplate(_magicLevelSelector),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(x => _magicLevelSelector(x.Record)),
                },
                150),
        ];
    }

    private static FuncDataTemplate<IReferencedRecord> CellTemplate(Func<IMajorRecordGetter, object?> selector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            [!TextBlock.TextProperty] = record.WhenAnyValue(x => x.Record)
                .Select(r => selector(r)?.ToString())
                .ToBinding()
        });
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IEnchantableGetter));
}
