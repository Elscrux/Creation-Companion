using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.Decorations;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services.Record.List.ExtraColumns;

public sealed class TierExtraColumn(IRecordDecorationController recordDecorationController) : ExtraColumns<IItemGetter> {
    public override IEnumerable<ExtraColumn> CreateColumns() {
        return [
            new ExtraColumn(
                new DataGridTemplateColumn {
                    Header = "Tier",
                    CellTemplate = new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
                        Name = "CellTextBlock",
                        [!TextBlock.TextProperty] = recordDecorationController.GetObservable<Tier>(record.Record.ToFormLinkInformation())
                            .Select(x => x.ToString())
                            .ToBinding(),
                    }),
                    CanUserSort = true,
                    CustomSortComparer = ReferencedRecordComparers.SelectorComparer(record =>
                        recordDecorationController.Get<Tier>(record.Record.ToFormLinkInformation())?.TierIdentifier)
                },
                149),
        ];
    }
}
