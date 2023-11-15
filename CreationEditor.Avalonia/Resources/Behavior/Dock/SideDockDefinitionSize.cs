using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.ViewModels.Docking;
using ReactiveUI;
namespace CreationEditor.Avalonia.Behavior;

public sealed class SideDockDefinitionSize : Behavior<DefinitionBase>, IDisposable {
    private readonly CompositeDisposable _disposables = new();

    public static readonly StyledProperty<SideDockVM?> SideDockProperty = AvaloniaProperty.Register<Control, SideDockVM?>(nameof(SideDock));

    public double ActiveTabSize { get; set; } = 300;
    public double ActiveTabMinSize { get; set; } = 50;
    public double NoActiveTabSize { get; set; } = 20;

    public SideDockVM? SideDock {
        get => GetValue(SideDockProperty);
        set => SetValue(SideDockProperty, value);
    }

    public bool UpdateSize { get; set; }

    protected override void OnAttached() {
        this.WhenAnyValue(x => x.SideDock,
                x => x.AssociatedObject)
            .Subscribe(_ => SideDockChanged())
            .DisposeWith(_disposables);
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        _disposables.Clear();
    }

    public void Dispose() => _disposables.Dispose();

    private void SideDockChanged() {
        if (SideDock is null || AssociatedObject is null) return;

        AssociatedObject.GetObservable(GetSizeProperty())
            .Subscribe(size => {
                if (UpdateSize && SideDock.ActiveTab is not null && size.GridUnitType == GridUnitType.Pixel) {
                    SideDock.ActiveTab.Size = size.Value;
                }
            })
            .DisposeWith(_disposables);

        SideDock.WhenAnyValue(
                x => x.InEditMode,
                x => x.ActiveTab,
                x => x.Tabs.Count,
                (inEditMode, activeTab, tabCount) => (InEditMode: inEditMode, ActiveTab: activeTab, TabCount: tabCount))
            .Subscribe(x => {
                if (x is { InEditMode: true, ActiveTab: not null }) return;

                var activeTabSize = UpdateSize && x.ActiveTab?.Size is not null ? x.ActiveTab.Size.Value : ActiveTabSize;
                var size = new GridLength(ReturnIf(SideDock, activeTabSize, NoActiveTabSize), GridUnitType.Pixel);
                AssociatedObject.SetValue(GetSizeProperty(), size);

                var minSize = ReturnIf(SideDock, ActiveTabMinSize, NoActiveTabSize);
                AssociatedObject.SetValue(GetMinSizeProperty(), minSize);
            })
            .DisposeWith(_disposables);
    }

    private static double ReturnIf(SideDockVM? vm, double tabActive, double noTabActive) {
        if (vm is null) return 0;

        if (!vm.Children.Any()) {
            return vm.InEditMode ? noTabActive : 0;
        }

        return vm.ActiveTab is not null ? tabActive : noTabActive;
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
