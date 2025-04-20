using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData;
using Noggog;
using ReactiveUI;
using Action = System.Action;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DataGridSelectionBehavior : Behavior<DataGrid>, IDisposable {
    public static readonly StyledProperty<bool?> AllCheckedProperty
        = AvaloniaProperty.Register<DataGrid, bool?>(nameof(AllChecked), false);

    public static readonly StyledProperty<Func<IReactiveSelectable, bool>> SelectionGuardProperty
        = AvaloniaProperty.Register<DataGrid, Func<IReactiveSelectable, bool>>(nameof(SelectionGuard), _ => true);

    public static readonly StyledProperty<IBinding?> ItemIsEnabledProperty
        = AvaloniaProperty.Register<DataGrid, IBinding?>(nameof(ItemIsEnabled));

    public static readonly StyledProperty<bool> MultiSelectProperty
        = AvaloniaProperty.Register<DataGridSelectionBehavior, bool>(nameof(MultiSelect), true);

    private bool _attached;
    private bool _isProcessing;

    public bool AddColumn { get; init; } = true;
    public bool AddContextFlyout { get; init; } = true;
    public bool AddKeyBinding { get; init; } = true;
    public bool AddDoubleTap { get; init; } = true;

    public bool ColumnEnabled { get; set; }
    public bool ContextFlyoutEnabled { get; set; }
    public bool KeyBindingEnabled { get; set; }

    public Key ToggleSelectionKeyBinding { get; init; } = Key.Space;

    public bool? AllChecked {
        get => GetValue(AllCheckedProperty);
        set => SetValue(AllCheckedProperty, value);
    }

    public Func<IReactiveSelectable, bool> SelectionGuard {
        get => GetValue(SelectionGuardProperty);
        set => SetValue(SelectionGuardProperty, value);
    }

    public IBinding? ItemIsEnabled {
        get => GetValue(ItemIsEnabledProperty);
        set => SetValue(ItemIsEnabledProperty, value);
    }

    public bool MultiSelect {
        get => GetValue(MultiSelectProperty);
        set => SetValue(MultiSelectProperty, value);
    }

    private readonly CompositeDisposable _attachedDisposable = new();
    private readonly CompositeDisposable _visualsAttachedDisposable = new();
    private IDisposable? _currentCollectionChanged;

    public void Dispose() {
        _visualsAttachedDisposable.Dispose();
        _attachedDisposable.Dispose();
    }

    protected override void OnAttached() {
        base.OnAttached();

        if (_attached) return;

        _attached = true;

        if (AddColumn) AddSelectionColumn();
        if (AddContextFlyout) AddSelectionMenu();
        if (AddKeyBinding) AddKeyBindings();
        if (AddDoubleTap) AddDoubleTapHandler();

        if (AssociatedObject is not null) AssociatedObject.LayoutUpdated += UpdateAllChecked;
    }

    protected override void OnDetaching() {
        if (ColumnEnabled) RemoveSelectionColumn();
        if (ContextFlyoutEnabled) RemoveSelectionMenu();
        if (KeyBindingEnabled) RemoveKeyBindings();
        if (AddDoubleTap) RemoveDoubleTapHandler();

        if (AssociatedObject is not null) AssociatedObject.LayoutUpdated -= UpdateAllChecked;

        _attachedDisposable.Clear();
    }

    protected override void OnAttachedToVisualTree() {
        base.OnAttachedToVisualTree();

        Register();

        if (AssociatedObject?.ItemsSource is INotifyCollectionChanged collectionChanged) {
            void Changed(object? o, NotifyCollectionChangedEventArgs e) => Register();
            collectionChanged.CollectionChanged -= Changed;
            collectionChanged.CollectionChanged += Changed;
        }

        void Register() {
            if (AssociatedObject?.ItemsSource is IEnumerable<IReactiveSelectable> reactiveSelectables) {
                _currentCollectionChanged?.Dispose();
                _currentCollectionChanged = reactiveSelectables
                    .ToObservable()
                    .ToObservableChangeSet()
                    .AutoRefresh(selectable => selectable.IsSelected)
                    .Subscribe(OnSelectionToggled)
                    .DisposeWith(_visualsAttachedDisposable);
            }
        }
    }

    public void OnSelectionToggled(IChangeSet<IReactiveSelectable> changeSet) {
        if (AssociatedObject?.ItemsSource is null) return;

        UpdateAllChecked();

        if (!MultiSelect) {
            if (changeSet.Count == 0) return;

            var selectableChange = changeSet.First();
            var selectable = selectableChange.Item.Current;
            if (selectable is null) return;

            if (!selectable.IsSelected) return;

            foreach (var modItem in AssociatedObject.ItemsSource
                .OfType<IReactiveSelectable>()
                .Where(m => m.IsSelected && m != selectable)) {
                modItem.IsSelected = false;
            }
        }
    }

    protected override void OnDetachedFromVisualTree() {
        base.OnDetachedFromVisualTree();

        _visualsAttachedDisposable.Clear();
    }

    private void AddSelectionColumn() {
        const double columnWidth = 24;
        AssociatedObject?.Columns.Insert(0,
            new DataGridTemplateColumn {
                HeaderTemplate = new FuncDataTemplate<IReactiveSelectable>((_, _) => {
                    var checkBox = new CheckBox {
                        [!Visual.IsVisibleProperty] = this.GetObservable(MultiSelectProperty).ToBinding(),
                        [!ToggleButton.IsCheckedProperty] = new Binding(nameof(AllChecked)),
                        MinWidth = 20,
                        DataContext = this,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };

                    checkBox.WhenAnyValue(x => x.IsChecked)
                        .Subscribe(isChecked => SelectAllItems(isChecked is true))
                        .DisposeWith(_attachedDisposable);

                    return checkBox;
                }),
                CellTemplate = new FuncDataTemplate<IReactiveSelectable>((_, _) => {
                    var checkBox = new CheckBox {
                        [!ToggleButton.IsCheckedProperty] = new Binding(nameof(IReactiveSelectable.IsSelected)),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        MinWidth = 20,
                        Classes = { "CenteredBorder", "CheckmarkOnly" },
                        Styles = {
                            new Style(x => x.OfType<CheckBox>().Class("CenteredBorder").Child().OfType<Border>()) {
                                Setters = {
                                    new Setter(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                                },
                            },
                        },
                    };

                    if (ItemIsEnabled is not null) {
                        checkBox[!InputElement.IsEnabledProperty] = ItemIsEnabled;
                    }

                    return checkBox;
                }),
                CanUserResize = false,
                CanUserSort = false,
                CanUserReorder = false,
                IsReadOnly = true,
                Width = new DataGridLength(columnWidth),
            });

        ColumnEnabled = true;
    }

    private void RemoveSelectionColumn() {
        AssociatedObject?.Columns?.RemoveAt(0);
        ColumnEnabled = false;
    }

    private void AddSelectionMenu() {
        if (AssociatedObject is null) return;

        AssociatedObject.ContextFlyout ??= new MenuFlyout();
        if (AssociatedObject.ContextFlyout is not MenuFlyout menuFlyout) return;

        if (menuFlyout.Items.Count > 0) menuFlyout.Items.Insert(0, new Separator());
        menuFlyout.Items.Insert(0,
            new MenuItem {
                [!Visual.IsVisibleProperty] = this.GetObservable(MultiSelectProperty).ToBinding(),
                Header = "Invert",
                Command = ReactiveCommand.Create(InvertAll),
            });
        menuFlyout.Items.Insert(0,
            new MenuItem {
                [!Visual.IsVisibleProperty] = this.GetObservable(MultiSelectProperty).ToBinding(),
                Header = "Select All",
                Command = ReactiveCommand.Create(() => SelectDynamic()),
            });

        ContextFlyoutEnabled = true;
    }

    private void RemoveSelectionMenu() {
        if (AssociatedObject?.ContextFlyout is not MenuFlyout menuFlyout) return;

        if (menuFlyout.Items.Count > 2) {
            menuFlyout.Items.RemoveAt(0);
            menuFlyout.Items.RemoveAt(0);
            menuFlyout.Items.RemoveAt(0);
        } else {
            menuFlyout.Items.Clear();
        }

        ContextFlyoutEnabled = false;
    }

    private void AddKeyBindings() {
        AssociatedObject?.KeyBindings.Insert(0,
            new KeyBinding {
                Command = ReactiveCommand.Create(ToggleSelection),
                Gesture = new KeyGesture(ToggleSelectionKeyBinding),
            });

        KeyBindingEnabled = true;
    }

    private void RemoveKeyBindings() {
        if (AssociatedObject is null) return;

        AssociatedObject.KeyBindings.RemoveAt(0);

        KeyBindingEnabled = false;
    }

    private void AddDoubleTapHandler() {
        AssociatedObject?.AddHandler(InputElement.DoubleTappedEvent, DoubleTappedEventHandler);
    }

    private void RemoveDoubleTapHandler() {
        AssociatedObject?.RemoveHandler(InputElement.DoubleTappedEvent, DoubleTappedEventHandler);
    }

    private void DoubleTappedEventHandler(object? sender, TappedEventArgs e) {
        if (AssociatedObject?.SelectedItems is null) return;

        SelectSelectedItems(!AssociatedObject.SelectedItems.OfType<IReactiveSelectable>().Any(s => s.IsSelected));
    }

    private void TryProcess(Action action) {
        if (_isProcessing) return;

        _isProcessing = true;
        action();
        _isProcessing = false;
    }

    private void UpdateAllChecked(object? o, EventArgs eventArgs) => UpdateAllChecked();

    private void UpdateAllChecked() {
        if (AssociatedObject?.ItemsSource is null) return;

        TryProcess(() => {
            var totalCount = 0;
            var selectedCount = 0;
            foreach (var selectable in AssociatedObject.ItemsSource.Cast<IReactiveSelectable>()) {
                totalCount++;
                if (selectable.IsSelected) selectedCount++;
            }

            if (totalCount == 0) {
                AllChecked = false;
            } else if (selectedCount == totalCount) {
                AllChecked = true;
            } else if (selectedCount > 0) {
                AllChecked = null;
            } else {
                AllChecked = false;
            }
        });
    }

    private void SelectSelectedItems(bool newState = true) {
        if (AssociatedObject?.SelectedItems is null) return;

        TryProcess(() => {
            _isProcessing = true;
            foreach (var selectable in AssociatedObject.SelectedItems.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = newState && SelectionGuard(selectable);
            }
        });

        UpdateAllChecked();
    }

    private void SelectAllItems(bool newState = true) {
        if (AssociatedObject?.ItemsSource is null) return;

        TryProcess(() => {
            foreach (var selectable in AssociatedObject.ItemsSource.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = newState && SelectionGuard(selectable);
                AllChecked = newState;
            }
        });
    }

    private void SelectDynamic(bool newState = true) {
        if (AssociatedObject?.SelectedItems is null) return;

        if (AssociatedObject.SelectedItems.Count > 1) {
            // Only select records in selection if multiple are selected
            SelectSelectedItems(newState);
        } else {
            // Otherwise select all records
            SelectAllItems(newState);
        }
    }

    private void ToggleSelection() {
        if (AssociatedObject?.SelectedItems is null) return;

        TryProcess(() => {
            _isProcessing = true;
            var newStatus = !AssociatedObject.SelectedItems
                .Cast<IReactiveSelectable>()
                .All(selectable => selectable.IsSelected);

            AssociatedObject.SelectedItems
                .Cast<IReactiveSelectable>()
                .ForEach(selectable => selectable.IsSelected = newStatus && SelectionGuard(selectable));
        });

        UpdateAllChecked();
    }

    private void InvertAll() {
        if (AssociatedObject?.ItemsSource is null) return;

        TryProcess(() => {
            foreach (var selectable in AssociatedObject.ItemsSource.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = !selectable.IsSelected && SelectionGuard(selectable);
            }
        });

        UpdateAllChecked();
    }
}
