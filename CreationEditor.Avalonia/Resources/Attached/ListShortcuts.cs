using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Noggog;
namespace CreationEditor.Avalonia.Attached;

public sealed class ListShortcuts : AvaloniaObject {
    public static readonly AttachedProperty<ICommand> AddProperty = AvaloniaProperty.RegisterAttached<ListShortcuts, Interactive, ICommand>("Add");
    public static readonly AttachedProperty<ICommand> RemoveProperty = AvaloniaProperty.RegisterAttached<ListShortcuts, Interactive, ICommand>("Remove");

    public static ICommand GetAdd(AvaloniaObject element) => element.GetValue(AddProperty);
    public static void SetAdd(AvaloniaObject element, ICommand addValue) => element.SetValue(AddProperty, addValue);

    public static ICommand GetRemove(AvaloniaObject element) => element.GetValue(RemoveProperty);
    public static void SetRemove(AvaloniaObject element, ICommand parameter) => element.SetValue(RemoveProperty, parameter);

    private static readonly KeyGesture AddGesture = KeyGesture.Parse("Add");
    private static readonly KeyGesture RemoveGesture = KeyGesture.Parse("Delete");

    private const string AddHeader = "Add";
    private const string RemoveHeader = "Remove";

    static ListShortcuts() {
        AddProperty.Changed.Subscribe(args => HandleGesture(args, AddGesture, AddHeader));

        RemoveProperty.Changed.Subscribe(args => HandleGesture(args, RemoveGesture, RemoveHeader));
    }

    private static void HandleGesture(AvaloniaPropertyChangedEventArgs<ICommand> args, KeyGesture gesture, string header) {
        if (args.Sender is not Control control) return;

        var newCommand = args.NewValue.GetValueOrDefault();

        // Add key bindings
        if (newCommand != null) {
            control.KeyBindings.Add(new KeyBinding {
                Command = newCommand,
                Gesture = gesture
            });
        } else {
            control.KeyBindings.RemoveWhere(keyBinding => ReferenceEquals(keyBinding.Gesture, gesture));
        }

        // Add context menu
        control.ContextFlyout ??= new MenuFlyout();
        if (control.ContextFlyout is MenuFlyout { Items: IAvaloniaList<object> list }) {
            if (newCommand != null) {
                list.Add(new MenuItem {
                    Header = header,
                    Command = newCommand
                });
            } else {
                list.RemoveWhere(item =>
                    item is MenuItem menuItem
                 && ReferenceEquals(menuItem.Header, header));
            }
        }
    }
}
