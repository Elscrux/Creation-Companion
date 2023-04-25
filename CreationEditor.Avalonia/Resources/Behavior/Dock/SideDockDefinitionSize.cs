using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.ViewModels.Docking;
using ReactiveUI;
namespace CreationEditor.Avalonia.Behavior;

public sealed class SideDockDefinitionSize : Behavior<DefinitionBase> {
    public static readonly StyledProperty<SideDockVM?> SideDockProperty = AvaloniaProperty.Register<Control, SideDockVM?>(nameof(SideDock));

    public double ActiveTabSize { get; set; } = 300;
    public double ActiveTabMinSize { get; set; } = 50;
    public double NoActiveTabSize { get; set; } = 20;

    public SideDockVM? SideDock {
        get => GetValue(SideDockProperty);
        set => SetValue(SideDockProperty, value);
    }

    public bool UpdateSize { get; set; }

    private IDisposable? _attachedDisposable;

    protected override void OnAttached() {
        _attachedDisposable = this.WhenAnyValue(x => x.SideDock,
                x => x.AssociatedObject)
            .Subscribe(_ => SideDockChanged());
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        _attachedDisposable?.Dispose();
    }

    private void SideDockChanged() {
        if (SideDock == null || AssociatedObject == null) return;

        AssociatedObject.GetObservable(GetSizeProperty())
            .Subscribe(size => {
                if (UpdateSize && SideDock.ActiveTab != null && size.GridUnitType == GridUnitType.Pixel) {
                    SideDock.ActiveTab.Size = size.Value;
                }
            });

        SideDock.WhenAnyValue(x => x.InEditMode, x => x.ActiveTab, x => x.Tabs.Count)
            .Subscribe(x => {
                var (editMode, activeTab, tabCount) = x;
                if (SideDock.InEditMode && SideDock.ActiveTab != null) return;

                var activeTabSize = UpdateSize && activeTab?.Size != null ? activeTab.Size.Value : ActiveTabSize;
                var size = new GridLength(ReturnIf(SideDock, activeTabSize, NoActiveTabSize), GridUnitType.Pixel);
                AssociatedObject.SetValue(GetSizeProperty(), size);

                var minSize = ReturnIf(SideDock, ActiveTabMinSize, NoActiveTabSize);
                AssociatedObject.SetValue(GetMinSizeProperty(), minSize);
            });
    }

    private double ReturnIf(SideDockVM? vm, double tabActive, double noTabActive) {
        if (vm == null) return 0;

        if (!vm.Children.Any()) {
            return vm.InEditMode ? noTabActive : 0;
        }

        return vm.ActiveTab != null ? tabActive : noTabActive;
    }

    private StyledProperty<GridLength> GetSizeProperty() {
        return AssociatedObject switch {
            ColumnDefinition => ColumnDefinition.WidthProperty,
            RowDefinition => RowDefinition.HeightProperty,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private StyledProperty<double> GetMinSizeProperty() {
        return AssociatedObject switch {
            ColumnDefinition => ColumnDefinition.MinWidthProperty,
            RowDefinition => RowDefinition.MinHeightProperty,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
