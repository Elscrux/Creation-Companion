using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
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
            _ => throw new ArgumentOutOfRangeException(nameof(dock), dock, null)
        };

        AdornerLayer.SetAdorner(this, new Rectangle {
            Width = 20,
            HorizontalAlignment = horizontalAlignment,
            Fill = (this as IDockPreview).Brush,
            IsHitTestVisible = false,
            Opacity = 0.5
        });
    }
}
