using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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
namespace CreationEditor.Avalonia.Behavior;

public sealed class DataGridSelectionBehavior : Behavior<DataGrid>, IDisposable {
    public static readonly StyledProperty<bool?> AllCheckedProperty
        = AvaloniaProperty.Register<DataGrid, bool?>(nameof(AllChecked), false);

    public static readonly StyledProperty<Func<IReactiveSelectable, bool>> SelectionGuardProperty
        = AvaloniaProperty.Register<DataGrid, Func<IReactiveSelectable, bool>>(nameof(SelectionGuard), _ => true);

    public static readonly StyledProperty<bool> MultiSelectProperty
        = AvaloniaProperty.Register<DataGridSelectionBehavior, bool>(nameof(MultiSelect), true);

    private bool _attached;
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

    public Func<IReactiveSelectable, bool> SelectionGuard {
        get => GetValue(SelectionGuardProperty);
        set => SetValue(SelectionGuardProperty, value);
    }

    public bool MultiSelect {
        get => GetValue(MultiSelectProperty);
        set => SetValue(MultiSelectProperty, value);
    }

    private readonly CompositeDisposable _attachedDisposable = new();
    private IDisposable? _currentCollectionChanged;

    public void Dispose() => _attachedDisposable?.Dispose();

    protected override void OnAttached() {
        base.OnAttached();
        
        if (_attached) return;

        _attached = true;

        if (AddColumn) AddSelectionColumn();
        if (AddContextFlyout) AddSelectionMenu();
        if (AddKeyBind) AddKeyBindings();

        if (AssociatedObject != null) AssociatedObject.LayoutUpdated += (_, _) => UpdateAllChecked();
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
                    .DisposeWith(_attachedDisposable);
            }
        }
    }

    public void OnSelectionToggled(IChangeSet<IReactiveSelectable> changeSet) {
        if (AssociatedObject?.ItemsSource == null) return;

        UpdateAllChecked();

        if (!MultiSelect) {
            if (changeSet.Count == 0) return;

            var selectableChange = changeSet.First();
            var selectable = selectableChange.Item.Current;
            if (selectable == null) return;

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

        _attachedDisposable?.Clear();
    }

    private void AddSelectionColumn() {
        const double columnWidth = 24;
        AssociatedObject?.Columns.Insert(0, new DataGridTemplateColumn {
            HeaderTemplate = new FuncDataTemplate<IReactiveSelectable>((_, _) => {
                var checkBox = new CheckBox {
                    [!ToggleButton.IsCheckedProperty] = new Binding(nameof(AllChecked)),
                    MinWidth = 20,
                    DataContext = this,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                checkBox.AddHandler(ToggleButton.IsCheckedChangedEvent, (_, e) => {
                    if (e.Source is ToggleButton { IsChecked: {} isChecked }) {
                        SelectAllItems(isChecked);
                    }
                });

                return checkBox;
            }),
            CellTemplate = new FuncDataTemplate<IReactiveSelectable>((_, _) => {
                var checkBox = new CheckBox {
                    [!ToggleButton.IsCheckedProperty] = new Binding(nameof(IReactiveSelectable.IsSelected)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    MinWidth = 20,
                    Classes = { "CenteredBorder" },
                    Styles = {
                        new Style(x => x.OfType<CheckBox>().Class("CenteredBorder").Child().OfType<Border>()) {
                            Setters = {
                                new Setter(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center),
                            }
                        },
                        new Style(x => x.OfType<CheckBox>().Class("CenteredBorder").Descendant().OfType<ContentPresenter>()) {
                            Setters = {
                                new Setter(Visual.IsVisibleProperty, false),
                            }
                        },
                    }
                };

                if (EnabledMapping != null) {
                    checkBox
                        .Bind(InputElement.IsEnabledProperty, new Binding(EnabledMapping))
                        .DisposeWith(_attachedDisposable);
                }

                return checkBox;
            }),
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
        if (AssociatedObject.ContextFlyout is not MenuFlyout { ItemsSource: AvaloniaList<object> menuList }) return;

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

    private void UpdateAllChecked() {
        if (AssociatedObject?.ItemsSource == null) return;

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
        if (AssociatedObject?.SelectedItems == null) return;

        TryProcess(() => {
            _isProcessing = true;
            foreach (var selectable in AssociatedObject.SelectedItems.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = newState && SelectionGuard.Invoke(selectable);
            }
        });

        UpdateAllChecked();
    }

    private void SelectAllItems(bool newState = true) {
        if (AssociatedObject?.ItemsSource == null) return;

        TryProcess(() => {
            foreach (var selectable in AssociatedObject.ItemsSource.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = newState && SelectionGuard.Invoke(selectable);
                AllChecked = newState;
            }
        });
    }

    private void SelectDynamic(bool newState = true) {
        if (AssociatedObject?.SelectedItems == null) return;

        if (AssociatedObject.SelectedItems.Count > 1) {
            // Only select records in selection if multiple are selected
            SelectSelectedItems(newState);
        } else {
            // Otherwise select all records
            SelectAllItems(newState);
        }
    }

    private void ToggleSelection() {
        if (AssociatedObject?.SelectedItems == null) return;

        TryProcess(() => {
            _isProcessing = true;
            var newStatus = !AssociatedObject.SelectedItems
                .Cast<IReactiveSelectable>()
                .All(selectable => selectable.IsSelected);

            AssociatedObject.SelectedItems
                .Cast<IReactiveSelectable>()
                .ForEach(selectable => selectable.IsSelected = newStatus && SelectionGuard.Invoke(selectable));
        });

        UpdateAllChecked();
    }

    private void InvertAll() {
        if (AssociatedObject?.ItemsSource == null) return;

        TryProcess(() => {
            foreach (var selectable in AssociatedObject.ItemsSource.Cast<IReactiveSelectable>()) {
                selectable.IsSelected = !selectable.IsSelected && SelectionGuard.Invoke(selectable);
            }
        });

        UpdateAllChecked();
    }
}
