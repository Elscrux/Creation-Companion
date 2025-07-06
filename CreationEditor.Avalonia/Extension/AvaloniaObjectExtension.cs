using Avalonia;
using Avalonia.Data;
namespace CreationEditor.Avalonia;

public static class AvaloniaObjectExtension {
    public static void Bind<T>(this AvaloniaObject avaloniaObject, AvaloniaProperty<T> property, object? bindTo) {
        if (bindTo is IBinding binding) { 
            avaloniaObject.Bind(property, binding);
        } else if (bindTo is IObservable<object> observable) {
            avaloniaObject.Bind(property, observable);
        } else {
            avaloniaObject.SetValue(property, bindTo);
        }
    }
}
