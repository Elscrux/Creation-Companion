using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class TierExtraColumn(IRecordDecorationController recordDecorationController) : IAutoAttachingExtraColumns {
    public IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Tier",
                    CellTemplate = new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
                        [!TextBlock.TextProperty] = recordDecorationController.GetObservable<Tier>(record.Record)
                            .Select(x => x.ToString())
                            .ToBinding(),
                    }),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(record =>
                        recordDecorationController.Get<Tier>(record.Record)?.TierIdentifier)
                },
                160),
        ];
    }

    public bool CanAttachTo(Type type) => type.IsAssignableTo(typeof(IItemGetter));
}
