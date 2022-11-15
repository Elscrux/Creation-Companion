using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Xaml.Interactivity;
using Noggog;
using ISelectable = Noggog.ISelectable;
namespace CreationEditor.WPF;

public class DataGridSelectionBehavior : Behavior<DataGrid> {
    public static readonly StyledProperty<bool> AllCheckedProperty = AvaloniaProperty.Register<DataGrid, bool>(nameof(AllChecked));
    public static readonly StyledProperty<Func<ISelectable, bool>> SelectionGuardProperty = AvaloniaProperty.Register<DataGrid, Func<ISelectable, bool>>(nameof(AllChecked), (_ => true));

    private bool _isProcessing;

    public string? EnabledMapping { get; set; }

    public bool AddColumn { get; set; } = true;
    public bool AddCommands { get; set; } = true;
    public bool AddKeyBind { get; set; } = true;

    public Key KeyBindKey { get; set; } = Key.Space;
    public string KeyBindEventName { get; set; } = "KeyUp";

    public bool? AllChecked {
        get => GetValue(AllCheckedProperty);
        set => SetValue(AllCheckedProperty, value);
    }

    public Func<ISelectable, bool> SelectionGuard {
        get => GetValue(SelectionGuardProperty);
        set => SetValue(SelectionGuardProperty, value);
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (AddColumn) AddSelectionColumn();
        if (AddCommands) AddSelectionMenu();
        if (AddKeyBind) AddKeyBindings();
    }
    
    private void AddSelectionColumn() {
        var cellCheckBox = new CheckBox();
        cellCheckBox.AddHandler(ToggleButton.CheckedEvent, UpdateAllChecked);
        cellCheckBox.AddHandler(ToggleButton.UncheckedEvent, UpdateAllChecked);
        cellCheckBox.SetValue(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        cellCheckBox.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        cellCheckBox.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(ISelectable.IsSelected)));
        if (EnabledMapping != null) cellCheckBox.Bind(InputElement.IsEnabledProperty, new Binding(EnabledMapping));
        
        var columnCheckBox = new CheckBox();
        columnCheckBox.AddHandler(ToggleButton.CheckedEvent, SelectAllItems);
        columnCheckBox.AddHandler(ToggleButton.UncheckedEvent, (_, _) => SelectAllItems(false));
        columnCheckBox.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(AllChecked)));
        
        // var column = new DataGridTemplateColumn {
        //     // HeaderTemplate = new DataTemplate { Content = columnCheckBox, DataType = typeof(CheckBox) },
        //     // CellTemplate = new DataTemplate { Content = cellCheckBox, DataType = typeof(CheckBox) },
        //     // HeaderTemplate = new DataTemplate(typeof(CheckBox)) { VisualTree = columnCheckBox },
        //     // CellTemplate = new DataTemplate(typeof(CheckBox)) { VisualTree = cellCheckBox },
        //     CanUserResize = false,
        //     CanUserSort = false,
        //     CanUserReorder = false,
        //     Width = new DataGridLength(40)
        // };

        AssociatedObject?.Columns.Insert(0, new DataGridTemplateColumn {
            Header = "test",
            CellTemplate = new DataTemplate { Content = cellCheckBox, DataType = typeof(CheckBox) },
            CanUserResize = false,
            CanUserSort = false,
            CanUserReorder = false,
            Width = new DataGridLength(250),
        });
    }

    private void AddSelectionMenu() {
        if (AssociatedObject == null) return;

        // AssociatedObject.ContextMenu ??= new ContextMenu();
        // if (AssociatedObject.ContextMenu.Items is not IList x) return;
        //
        // var selectAllMenu = new MenuItem {
        //     Header = "Select All",
        //     Command = ReactiveCommand.Create(() => SelectDynamic())
        // };
        //     
        // x.Insert(0, selectAllMenu);
        //
        // var invertMenu = new MenuItem {
        //     Header = "Invert",
        //     Command = ReactiveCommand.Create(InvertAll)
        // };
        // x.Insert(0, invertMenu);
    }

    private void AddKeyBindings() {
        if (AssociatedObject == null) return;
        
        // var eventTrigger = new EventTriggerBehavior {
        //     EventName = KeyBindEventName,
        //     Actions = {
        //         new InvokeCommandAction {
        //             Command = ReactiveCommand.Create((KeyEventArgs args) => {
        //                 if (args.Key == KeyBindKey) {
        //                     ToggleSelection();
        //                 }
        //             }),
        //             PassEventArgsToCommand = true
        //         }
        //     }
        // };

        // var behaviorCollection = Interaction.GetBehaviors(AssociatedObject);
        //
        // // behaviorCollection.Add(eventTrigger);
        // Interaction.SetBehaviors(AssociatedObject, behaviorCollection);
    }

    private void TryProcess(Action action) {
        if (_isProcessing) return;

        _isProcessing = true;
        action.Invoke();
        _isProcessing = false;
    }

    private void UpdateAllChecked() {
        if (AssociatedObject == null) return;
        
        TryProcess(() => {
            var totalCount = 0;
            var selectedCount = 0;
            foreach (var selectable in AssociatedObject.Items.Cast<ISelectable>()) {
                totalCount++;
                if (selectable.IsSelected) selectedCount++;
            }

            if (selectedCount == totalCount) {
                AllChecked = true;
            } else if (selectedCount > 0) {
                AllChecked = null;
            } else {
                AllChecked = false;
            }
        });
    }

    private void SelectSelectedItems(bool newState = true) {
        if (AssociatedObject == null) return;
        
        TryProcess(() => {
            _isProcessing = true;
            foreach (var selectedItem in AssociatedObject.SelectedItems) {
                var selectable = (ISelectable) selectedItem;
                selectable.IsSelected = newState && SelectionGuard.Invoke(selectable);
            }
        });

        UpdateAllChecked();
    }

    private void SelectAllItems(bool newState = true) {
        if (AssociatedObject == null) return;
        
        TryProcess(() => {
            foreach (var selectable in AssociatedObject.Items.Cast<ISelectable>()) {
                selectable.IsSelected = newState && SelectionGuard.Invoke(selectable);
                AllChecked = newState;
            }
        });
    }

    private void SelectDynamic(bool newState = true) {
        if (AssociatedObject == null) return;
        
        if (AssociatedObject.SelectedItems.Count > 1) {
            //Only select records in selection if multiple are selected
            SelectSelectedItems(newState);
        } else {
            //Otherwise select all records
            SelectAllItems(newState);
        }
    }

    private void ToggleSelection() {
        if (AssociatedObject == null) return;
        
        TryProcess(() => {
            _isProcessing = true;
            var newStatus = !AssociatedObject.SelectedItems
                .Cast<ISelectable>()
                .All(selectable => selectable.IsSelected);

            AssociatedObject.SelectedItems
                .Cast<ISelectable>()
                .ForEach(selectable => selectable.IsSelected = newStatus && SelectionGuard.Invoke(selectable));
        });

        UpdateAllChecked();
    }

    private void InvertAll() {
        if (AssociatedObject == null) return;
        
        TryProcess(() => {
            foreach (var selectable in AssociatedObject.Items.Cast<ISelectable>()) {
                selectable.IsSelected = !selectable.IsSelected && SelectionGuard.Invoke(selectable);
            }
        });

        UpdateAllChecked();
    }
}
