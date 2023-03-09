using System.Collections;
using Avalonia;
namespace CreationEditor.Avalonia.Command;

public class RemoveElement<T> : ListCommand<T> where T : new() {
    public static readonly StyledProperty<IList<T>> RemoveItemsProperty
        = AvaloniaProperty.Register<RemoveElement<T>, IList<T>>(nameof(RemoveItems));

    public IList<T> RemoveItems {
        get => GetValue(RemoveItemsProperty);
        set => SetValue(RemoveItemsProperty, value);
    }

    public override void Execute(object? parameter) {
        if (List == null) return;
        if (parameter is not IList removeList) return;

        foreach (var t in removeList.OfType<T>().ToList()) {
            List.Remove(t);
        }
    }
}
