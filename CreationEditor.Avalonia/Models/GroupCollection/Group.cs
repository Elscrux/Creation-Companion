using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.GroupCollection;

public class Group<T> : ReactiveObject {
    public Func<T, object> Selector { get; }
    [Reactive] public bool IsGrouped { get; set; }

    public Group(Func<T, object> selector, bool isGrouped) {
        Selector = selector;
        IsGrouped = isGrouped;
    }
}
