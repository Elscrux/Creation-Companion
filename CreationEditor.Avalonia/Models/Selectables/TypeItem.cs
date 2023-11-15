using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public sealed class TypeItem(Type type, bool isSelected = true) : ReactiveObject, IReactiveSelectable {
    public Type Type { get; } = type;
    [Reactive] public bool IsSelected { get; set; } = isSelected;
}
