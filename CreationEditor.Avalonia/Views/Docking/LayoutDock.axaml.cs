using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class LayoutDock : ReactiveUserControl<LayoutDockVM>, IDockPreview {
    public double PreviewOutlineThickness { get; set; } = 10;

    public LayoutDock() {
        InitializeComponent();
    }

    public LayoutDock(DockContainerVM vm) : this() {
        DataContext = vm;
    }

    public void ShowPreview(Dock dock) {
        var border = new Border {
            IsHitTestVisible = false,
            Opacity = 0.5,
            BorderBrush = (this as IDockPreview).Brush,
            BorderThickness = new Thickness(
                dock == Dock.Left ? PreviewOutlineThickness : 0,
                dock == Dock.Top ? PreviewOutlineThickness : 0,
                dock == Dock.Right ? PreviewOutlineThickness : 0,
                dock == Dock.Bottom ? PreviewOutlineThickness : 0
            ),
        };

        AdornerLayer.SetAdorner(this, border);
    }
}
