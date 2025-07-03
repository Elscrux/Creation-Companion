using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace LeveledList.Services.Record.List.ExtraColumns;

public static class FeaturesExtraColumn {
    public static FuncDataTemplate<IReferencedRecord> GetCellTemplate(Func<IMajorRecordGetter, object?> selector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            Name = "CellTextBlock",
            [!TextBlock.TextProperty] = record.WhenAnyValue(x => x.Record)
                .Select(r => selector(r)?.ToString())
                .ToBinding()
        });
    }
}
