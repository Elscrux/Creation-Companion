using Avalonia;
using Avalonia.Data;
namespace CreationEditor.Avalonia;

public static class AvaloniaObjectExtension {
    extension(AvaloniaObject avaloniaObject) {
        public void Bind<T>(AvaloniaProperty<T> property, object? bindTo) {
            if (bindTo is BindingBase binding) { 
                avaloniaObject.Bind(property, binding);
            } else if (bindTo is IObservable<object> observable) {
                avaloniaObject.Bind(property, observable);
            } else {
                avaloniaObject.SetValue(property, bindTo);
            }
        }
    }
}
