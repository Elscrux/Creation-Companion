using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Layout;
using CreationEditor.Avalonia.Constants;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class Tab : UserControl, IDockPreview {
    public static readonly StyledProperty<IDockedItem> DockedItemProperty
        = AvaloniaProperty.Register<Tab, IDockedItem>(nameof(DockedItem));

    public IDockedItem DockedItem {
        get => GetValue(DockedItemProperty);
        set => SetValue(DockedItemProperty, value);
    }

    public static readonly StyledProperty<TabbedDockVM?> DockContainerProperty
        = AvaloniaProperty.Register<Tab, TabbedDockVM?>(nameof(DockContainer));

    public TabbedDockVM? DockContainer {
        get => GetValue(DockContainerProperty);
        set => SetValue(DockContainerProperty, value);
    }

    public Tab() {
        InitializeComponent();
    }

    public void ShowPreview(Dock dock) {
        var horizontalAlignment = dock switch {
            Dock.Left => HorizontalAlignment.Left,
            Dock.Bottom => HorizontalAlignment.Left,
            Dock.Right => HorizontalAlignment.Right,
            Dock.Top => HorizontalAlignment.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(dock), dock, null),
        };

        AdornerLayer.SetAdorner(
            this,
            new Rectangle {
                Width = 20,
                HorizontalAlignment = horizontalAlignment,
                Fill = StandardBrushes.ValidBrush,
                IsHitTestVisible = false,
                Opacity = 0.5,
            });
    }

    private async void OnTabPointerReleased(object? sender, PointerReleasedEventArgs e) {
        if (sender is not Control control) return;

        var currentPoint = e.GetCurrentPoint(control);
        if (currentPoint.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased) {
            await DockedItem.Close.Execute();
        } else {
            if (DockContainer is not null) {
                await DockContainer.Activate.Execute(DockedItem);
            }
        }
    }
}
