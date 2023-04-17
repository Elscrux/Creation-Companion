using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using Noggog;
namespace CreationEditor.Avalonia.Attached;

public sealed class ListShortcuts : AvaloniaObject {
    public static readonly AttachedProperty<ICommand> AddProperty = AvaloniaProperty.RegisterAttached<ListShortcuts, Interactive, ICommand>("Add");
    public static readonly AttachedProperty<ICommand> RemoveProperty = AvaloniaProperty.RegisterAttached<ListShortcuts, Interactive, ICommand>("Remove");

    public static ICommand GetAdd(AvaloniaObject element) => element.GetValue(AddProperty);
    public static void SetAdd(AvaloniaObject element, ICommand addValue) => element.SetValue(AddProperty, addValue);

    public static ICommand GetRemove(AvaloniaObject element) => element.GetValue(RemoveProperty);
    public static void SetRemove(AvaloniaObject element, ICommand parameter) => element.SetValue(RemoveProperty, parameter);

    private static readonly KeyGesture AddGesture = new(Key.OemPlus);
    private static readonly KeyGesture RemoveGesture = new(Key.OemMinus);

    private const string AddHeader = "Add";
    private const string RemoveHeader = "Remove";

    private const Symbol AddSymbol = Symbol.Add;
    private const Symbol RemoveSymbol = Symbol.Remove;

    static ListShortcuts() {
        AddProperty.Changed.Subscribe(args => {
            HandleGesture(args, null, AddGesture, AddHeader, AddSymbol);

            if (args.Sender is DataGrid dg) {
                ToggleAddButton(dg, args);
            }
        });

        RemoveProperty.Changed.Subscribe(args => {
            var selectedItems = args.Sender switch {
                DataGrid dataGrid => dataGrid.SelectedItems,
                ListBox listBox => listBox.SelectedItems,
                _ => null
            };

            HandleGesture(args, selectedItems, RemoveGesture, RemoveHeader, RemoveSymbol);

            if (args.Sender is DataGrid dg) {
                AddRemoveButton(dg, args.NewValue.GetValueOrDefault());
            }
        });
    }

    private static void ToggleAddButton(DataGrid dg, AvaloniaPropertyChangedEventArgs<ICommand> args) {
        dg.Loaded -= Function;
        dg.Loaded += Function;

        void Function(object? sender, RoutedEventArgs routedEventArgs) {
            var addButton = dg.FindDescendantOfType<Button>();
            if (addButton == null) return;

            addButton.IsVisible = true;
            addButton.Command = args.NewValue.GetValueOrDefault();
        }
    }

    private static void AddRemoveButton(DataGrid dg, ICommand? command) {
        var removeButtonTemplate = new FuncDataTemplate<object>((o, _) => new Button {
            [!Visual.IsVisibleProperty] = new Binding("IsPointerOver") {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) {
                    AncestorType = typeof(DataGridRow),
                }
            },
            Content = new SymbolIcon { Symbol = Symbol.Delete },
            Foreground = Brushes.Red,
            Classes = new Classes("Transparent"),
            Command = command,
            HorizontalAlignment = HorizontalAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            CommandParameter = new ArrayList { o },
        });

        var removeColumn = new DataGridTemplateColumn {
            CellTemplate = removeButtonTemplate,
            CanUserResize = false,
            CanUserSort = false,
            CanUserReorder = false,
            IsReadOnly = true,
        };

        dg.Loaded -= Function;
        dg.Loaded += Function;

        void Function(object? sender, RoutedEventArgs routedEventArgs) {
            if (dg.Columns.Any(c =>
                c is DataGridTemplateColumn templateColumn
             && templateColumn.CellTemplate == removeButtonTemplate)) return;

            dg.Columns.Add(removeColumn);
        }
    }

    private static void HandleGesture(AvaloniaPropertyChangedEventArgs<ICommand> args, IList? selectedItems, KeyGesture gesture, string header, Symbol menuIcon) {
        if (args.Sender is not Control control) return;

        var newCommand = args.NewValue.GetValueOrDefault();

        // Add key bindings
        if (newCommand != null) {
            control.KeyBindings.Add(new KeyBinding {
                Command = newCommand,
                Gesture = gesture,
                CommandParameter = selectedItems!
            });
        } else {
            control.KeyBindings.RemoveWhere(keyBinding => ReferenceEquals(keyBinding.Gesture, gesture));
        }

        // Add context menu
        control.ContextFlyout ??= new MenuFlyout();
        AddCommand(selectedItems, header, control.ContextFlyout, newCommand, menuIcon);

        control.GetPropertyChangedObservable(Control.ContextFlyoutProperty)
            .Subscribe(_ => {
                control.ContextFlyout ??= new MenuFlyout();
                AddCommand(selectedItems, header, control.ContextFlyout, newCommand, menuIcon);
            });
    }

    private static void AddCommand(IEnumerable? selectedItems, string header, FlyoutBase? flyout, ICommand? newCommand, Symbol menuIcon) {
        if (flyout is not MenuFlyout { Items: IAvaloniaList<object> list }) return;
        if (list.OfType<MenuItem>().Any(x => x.Command == newCommand)) return;

        if (newCommand != null) {
            list.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = menuIcon },
                Header = header,
                Command = newCommand,
                CommandParameter = selectedItems
            });
        } else {
            list.RemoveWhere(item =>
                item is MenuItem menuItem
             && ReferenceEquals(menuItem.Header, header));
        }
    }
}
