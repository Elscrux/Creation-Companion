using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Selectables;

public partial class Selectable<T> : ReactiveObject, IReactiveSelectable {
    [Reactive] public partial bool IsSelected { get; set; }
    public required T Value { get; init; }
}
