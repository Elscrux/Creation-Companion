using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
namespace CreationEditor.Avalonia.Attached;

/// <summary>
/// Container class for attached properties. Must inherit from <see cref="AvaloniaObject"/>.
/// </summary>
public sealed class DoubleTappedProperty : AvaloniaObject {
    /// <summary>
    /// Identifies the <seealso cref="CommandProperty"/> avalonia attached property.
    /// </summary>
    /// <value>Provide an <see cref="ICommand"/> derived object or binding.</value>
    public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<DoubleTappedProperty, Interactive, ICommand>("Command");

    public static ICommand GetCommand(AvaloniaObject element) => element.GetValue(CommandProperty);
    public static void SetCommand(AvaloniaObject element, ICommand commandValue) => element.SetValue(CommandProperty, commandValue);

    /// <summary>
    /// Identifies the <seealso cref="CommandParameterProperty"/> avalonia attached property.
    /// Use this as the parameter for the <see cref="CommandProperty"/>.
    /// </summary>
    /// <value>Any value of type <see cref="object"/>.</value>
    public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<DoubleTappedProperty, Interactive, object>("CommandParameter");

    public static object GetCommandParameter(AvaloniaObject element) => element.GetValue(CommandParameterProperty);
    public static void SetCommandParameter(AvaloniaObject element, object parameter) => element.SetValue(CommandParameterProperty, parameter);

    static DoubleTappedProperty() {
        CommandProperty.Changed
            .Subscribe(args => HandleCommandChanged(args.Sender, args.NewValue.GetValueOrDefault<ICommand>()));
    }

    private static void HandleCommandChanged(IAvaloniaObject element, ICommand? commandValue) {
        if (element is Interactive interactElem) {
            if (commandValue != null) {
                interactElem.AddHandler(InputElement.DoubleTappedEvent, DoubleTappedEventHandler);
            } else {
                interactElem.RemoveHandler(InputElement.DoubleTappedEvent, DoubleTappedEventHandler);
            }
        }

        void DoubleTappedEventHandler(object? sender, TappedEventArgs tappedEventArgs) {
            var commandParameter = interactElem.GetValue(CommandParameterProperty);
            if (commandValue?.CanExecute(commandParameter) is true) {
                commandValue.Execute(commandParameter);
            }
        }
    }
}
