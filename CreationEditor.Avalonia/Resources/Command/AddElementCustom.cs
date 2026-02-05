using Avalonia;
namespace CreationEditor.Avalonia.Command;

public sealed class AddElementCustom<T> : ListCommand<T> {
    public static readonly StyledProperty<Func<T>> FactoryProperty
        = AvaloniaProperty.Register<AvaloniaObject, Func<T>>(nameof(List));

    public Func<T> Factory {
        get => GetValue(FactoryProperty);
        set => SetValue(FactoryProperty, value);
    }

    public override void Execute(object? parameter) {
        List?.Add(Factory.Invoke());
    }
}
