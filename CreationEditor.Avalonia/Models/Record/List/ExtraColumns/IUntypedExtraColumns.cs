using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public interface IUntypedExtraColumns {
    static FuncDataTemplate<IReferencedRecord> GetTextCellTemplate(Func<IMajorRecordGetter, object?> selector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            Name = "CellTextBlock",
            [!TextBlock.TextProperty] = record.WhenAnyValue(x => x.Record)
                .Select(r => selector(r)?.ToString())
                .ToBinding()
        });
    }

    static FuncDataTemplate<IReferencedRecord> GetTextCellTemplate(Func<IReferencedRecord, IBinding> bindingSelector) {
        return new FuncDataTemplate<IReferencedRecord>((record, _) => new TextBlock {
            Name = "CellTextBlock",
            [!TextBlock.TextProperty] = bindingSelector(record)
        });
    }

    IEnumerable<ExtraColumn> CreateColumns();
}
