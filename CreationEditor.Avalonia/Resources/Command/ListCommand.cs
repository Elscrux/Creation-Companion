using System.Windows.Input;
using Avalonia;
namespace CreationEditor.Avalonia.Command;

public abstract class ListCommand<T> : AvaloniaObject, ICommand {
    public static readonly StyledProperty<IList<T>?> ListProperty
        = AvaloniaProperty.Register<ListCommand<T>, IList<T>?>(nameof(List));

    public IList<T>? List {
        get => GetValue(ListProperty);
        set => SetValue(ListProperty, value);
    }

    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter) => true;
    public abstract void Execute(object? parameter);
}
