using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;
using Noggog;
using ReactiveUI;
using ISelectable = Noggog.ISelectable;
namespace CreationEditor.WPF;

public class DataGridSelectionBehavior : Behavior<DataGrid> {
    public static readonly StyledProperty<bool?> AllCheckedProperty = AvaloniaProperty.Register<DataGrid, bool?>(nameof(AllChecked), false);
    public static readonly StyledProperty<Func<ISelectable, bool>> SelectionGuardProperty = AvaloniaProperty.Register<DataGrid, Func<ISelectable, bool>>(nameof(SelectionGuard), (_ => true));

    private bool _isProcessing;

    public string? EnabledMapping { get; init; }

    public bool AddColumn { get; init; } = true;
    public bool AddContextFlyout { get; init; } = true;
    public bool AddKeyBind { get; init; } = true;

    public Key ToggleSelectionKeyBinding { get; init; } = Key.Space;

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
        if (AddContextFlyout) AddSelectionMenu();
        if (AddKeyBind) AddKeyBindings();
    }
    
    private void AddSelectionColumn() {
        const double columnWidth = 25;
        AssociatedObject?.Columns.Insert(0, new DataGridTemplateColumn {
            HeaderTemplate = new FuncDataTemplate<ISelectable>((_, _) => {
                var checkBox = new CheckBox {
                    [!ToggleButton.IsCheckedProperty] = new Binding(nameof(AllChecked)),
                    MinWidth = columnWidth,
                    DataContext = this,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                
                checkBox.AddHandler(ToggleButton.CheckedEvent, (_, _) => SelectAllItems());
                checkBox.AddHandler(ToggleButton.UncheckedEvent, (_, _) => SelectAllItems(false));
                
                return checkBox;
            }),
            CellTemplate = new FuncDataTemplate<ISelectable>(((_, _) => {
                var checkBox = new CheckBox {
                    [!ToggleButton.IsCheckedProperty] = new Binding(nameof(ISelectable.IsSelected)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    MinWidth = columnWidth,
                    Classes = new Classes("CenteredBorder"),
                    Styles = { new Style(x => x.OfType<CheckBox>().Class("CenteredBorder").Child().OfType<Border>()) {
                        Setters = { new Setter(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center) }
                    } }
                };
                
                checkBox.AddHandler(ToggleButton.CheckedEvent, (_, _) => UpdateAllChecked(true));
                checkBox.AddHandler(ToggleButton.UncheckedEvent, (_, _) => UpdateAllChecked(false));
                
                if (EnabledMapping != null) checkBox.Bind(InputElement.IsEnabledProperty, new Binding(EnabledMapping));
                
                return checkBox;
            })),
            CanUserResize = false,
            CanUserSort = false,
            CanUserReorder = false,
            IsReadOnly = true,
            Width = new DataGridLength(columnWidth)
        });
    }

    private void AddSelectionMenu() {
        if (AssociatedObject == null) return;

        AssociatedObject.ContextFlyout ??= new MenuFlyout();
        if (AssociatedObject.ContextFlyout is not MenuFlyout { Items: AvaloniaList<object> menuList }) return;
        
        menuList.InsertRange(0, new TemplatedControl[] {
            new MenuItem {
                Header = "Select All",
                Command = ReactiveCommand.Create(() => SelectDynamic())
            },
            new MenuItem {
                Header = "Invert",
                Command = ReactiveCommand.Create(InvertAll),
            },
            new Separator(),
        });
    }

    private void AddKeyBindings() {
        AssociatedObject?.KeyBindings.Add(new KeyBinding {
            Command = ReactiveCommand.Create(ToggleSelection),
            Gesture = new KeyGesture(ToggleSelectionKeyBinding),
        });
    }

    private void TryProcess(Action action) {
        if (_isProcessing) return;

        _isProcessing = true;
        action.Invoke();
        _isProcessing = false;
    }

    private void UpdateAllChecked(bool? lastWasChecked = null) {
        if (AssociatedObject == null) return;
        
        TryProcess(() => {
            var totalCount = 0;
            var selectedCount = 0;
            foreach (var selectable in AssociatedObject.Items.Cast<ISelectable>()) {
                totalCount++;
                if (selectable.IsSelected) selectedCount++;
            }

            // The checked/unchecked event comes before the IsSelected binding is updated, so we need to cheat
            // When we know that something was set to checked, we add one selected count and the other way around for unchecked
            // We don't do anything when the bindings aren't affected
            selectedCount += lastWasChecked switch {
                true => 1,
                false => -1,
                null => 0
            };

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
            foreach (var selectable in AssociatedObject.SelectedItems.Cast<ISelectable>()) {
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
            // Only select records in selection if multiple are selected
            SelectSelectedItems(newState);
        } else {
            // Otherwise select all records
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
