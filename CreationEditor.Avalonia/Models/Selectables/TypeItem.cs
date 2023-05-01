using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public sealed class TypeItem : ReactiveObject, IReactiveSelectable {
    public Type Type { get; init; }
    [Reactive] public bool IsSelected { get; set; }

    public TypeItem(Type type, bool isSelected = true) {
        Type = type;
        IsSelected = isSelected;
    }
}
