using CreationEditor.Avalonia.Models.Record;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public sealed class SelectableRecordDiff(RecordDiff recordDiff) : ReactiveObject, IReactiveSelectable {
    [Reactive] public bool IsSelected { get; set; }
    public RecordDiff RecordDiff { get; } = recordDiff;
}
