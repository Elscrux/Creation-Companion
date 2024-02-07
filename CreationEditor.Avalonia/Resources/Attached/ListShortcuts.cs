using System.Collections;
using System.Reflection;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
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
using ReactiveUI;
namespace CreationEditor.Avalonia.Attached;

public sealed class ListShortcuts : AvaloniaObject {
    private const string RemoveColumnTag = "ListShortcuts_RemoveColumn";

    private static readonly MethodInfo TreeDataGridAddRemoveButtonMethodInfo = typeof(ListShortcuts).GetMethod(nameof(TreeDataGrid_AddRemoveButton), BindingFlags.Static | BindingFlags.NonPublic)!;

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

            switch (args.Sender) {
                case Control c:
                    ToggleAddButton(c, args);
                    break;
            }
        });

        RemoveProperty.Changed.Subscribe(args => {
            var selectedItems = args.Sender switch {
                DataGrid dataGrid => dataGrid.SelectedItems,
                ListBox listBox => listBox.SelectedItems,
                _ => null
            };

            HandleGesture(args, selectedItems, RemoveGesture, RemoveHeader, RemoveSymbol);

            switch (args.Sender) {
                case DataGrid dg:
                    DataGrid_AddRemoveButton(dg, args.NewValue.GetValueOrDefault());
                    break;
                case TreeDataGrid treeDataGrid:
                    IDisposable? disposable = null;

                    treeDataGrid.Unloaded -= OnTreeDataGridUnloaded;
                    treeDataGrid.Unloaded += OnTreeDataGridUnloaded;
                    treeDataGrid.Loaded -= OnTreeDataGridLoaded;
                    treeDataGrid.Loaded += OnTreeDataGridLoaded;
                    break;

                    void OnTreeDataGridLoaded(object? sender, RoutedEventArgs e) {
                        disposable?.Dispose();
                        disposable = treeDataGrid.WhenAnyValue(x => x.Source)
                            .NotNull()
                            .Subscribe(source => {
                                var t = source.GetType().GetGenericArguments()[0];

                                var genericAddRemoveButtonMethodInfo = TreeDataGridAddRemoveButtonMethodInfo.MakeGenericMethod(t);
                                genericAddRemoveButtonMethodInfo.Invoke(null, [source, args.NewValue.GetValueOrDefault()]);
                            });
                    }
                    void OnTreeDataGridUnloaded(object? sender, RoutedEventArgs e) {
                        disposable?.Dispose();
                        treeDataGrid.Unloaded -= OnTreeDataGridUnloaded;
                    }
            }
        });
    }

    private static void ToggleAddButton(Control c, AvaloniaPropertyChangedEventArgs<ICommand> args) {
        c.Loaded -= Function;
        c.Loaded += Function;

        void Function(object? sender, RoutedEventArgs routedEventArgs) {
            var addButton = c.FindDescendantOfType<Button>();
            if (addButton is null) return;

            addButton.IsVisible = true;
            addButton.Command = args.NewValue.GetValueOrDefault();
        }
    }

    private static FuncDataTemplate<object> RemoveButtonTemplate<TRowType>(ICommand? command) {
        return new FuncDataTemplate<object>((o, _) => new Button {
            [!Visual.IsVisibleProperty] = new Binding("IsPointerOver") {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) {
                    AncestorType = typeof(TRowType),
                }
            },
            Content = new SymbolIcon { Symbol = Symbol.Delete },
            Foreground = Brushes.Red,
            Classes = { "Transparent" },
            Command = command,
            HorizontalAlignment = HorizontalAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            CommandParameter = new ArrayList { o },
        });
    }

    private static void DataGrid_AddRemoveButton(DataGrid dg, ICommand? command) {
        var removeColumn = new DataGridTemplateColumn {
            Tag = RemoveColumnTag,
            CellTemplate = RemoveButtonTemplate<DataGridRow>(command),
            CanUserResize = false,
            CanUserSort = false,
            CanUserReorder = false,
            IsReadOnly = true,
        };

        dg.Loaded -= Function;
        dg.Loaded += Function;

        void Function(object? sender, RoutedEventArgs routedEventArgs) {
            if (dg.Columns.Any(c => ReferenceEquals(c.Tag, RemoveColumnTag))) return;

            dg.Columns.Add(removeColumn);
        }
    }

    private static void TreeDataGrid_AddRemoveButton<T>(ITreeDataGridSource treeDataGridSource, ICommand? command) {
        var templateColumn = new TemplateColumn<T>(
            string.Empty,
            RemoveButtonTemplate<TreeDataGridRow>(command),
            options: new TemplateColumnOptions<T> {
                CanUserResizeColumn = false,
                CanUserSortColumn = false,
            });

        if (treeDataGridSource.Columns.Any(c => ReferenceEquals(c.Tag, RemoveColumnTag))) return;

        if (treeDataGridSource.Columns is ColumnList<T> column) {
            column.Add(templateColumn);
        }
    }

    private static void HandleGesture(AvaloniaPropertyChangedEventArgs<ICommand> args, IList? selectedItems, KeyGesture gesture, string header, Symbol menuIcon) {
        if (args.Sender is not Control control) return;

        var newCommand = args.NewValue.GetValueOrDefault();

        // Add key bindings
        if (newCommand is not null) {
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
        if (flyout is not MenuFlyout { Items: {} list }) return;
        if (list.OfType<MenuItem>().Any(x => x.Command == newCommand)) return;

        if (newCommand is not null) {
            list.Add(new MenuItem {
                Icon = new SymbolIcon { Symbol = menuIcon },
                Header = header,
                Command = newCommand,
                CommandParameter = selectedItems
            });
        } else {
            for (var i = list.Count - 1; i >= 0; i--) {
                if (list[i] is MenuItem menuItem && ReferenceEquals(menuItem.Header, header)) {
                    list.RemoveAt(i);
                }
            }
        }
    }
}
