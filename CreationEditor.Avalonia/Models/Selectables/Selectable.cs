using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public class Selectable<T>(T value, bool isSelected = true) : ReactiveObject, IReactiveSelectable {
    [Reactive] public bool IsSelected { get; set; } = isSelected;
    public T Value { get; } = value;
}
