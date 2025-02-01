using CreationEditor.Avalonia.Models.Record;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Selectables;

public sealed partial class SelectableRecordDiff : ReactiveObject, IReactiveSelectable {
    [Reactive] public partial bool IsSelected { get; set; }
    public required RecordDiff RecordDiff { get; init; }
}
