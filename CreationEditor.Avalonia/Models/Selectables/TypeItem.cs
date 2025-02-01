using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Selectables;

public sealed partial class TypeItem : ReactiveObject, IReactiveSelectable {
    [Reactive] public partial bool IsSelected { get; set; }
    public required Type Type { get; init; }
}
