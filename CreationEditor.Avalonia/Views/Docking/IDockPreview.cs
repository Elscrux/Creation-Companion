using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Views.Docking;

public interface IDockPreview {
    public IBrush? Brush =>
        Application.Current is not null
     && Application.Current.TryFindResource("SystemAccentColor", out var obj)
     && obj is Color color
            ? new SolidColorBrush(color)
            : null;

    public void ShowPreview(Dock dock) {
        AdornerLayer.SetAdorner((Visual) this,
            new Rectangle {
                Fill = Brush,
                IsHitTestVisible = false,
                Opacity = 0.5,
            });
    }

    public void HidePreview() {
        AdornerLayer.SetAdorner((Visual) this, null);
    }
}
