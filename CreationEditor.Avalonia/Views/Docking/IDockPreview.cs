using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using CreationEditor.Avalonia.Constants;
namespace CreationEditor.Avalonia.Views.Docking;

public interface IDockPreview {
    void ShowPreview(Dock dock) {
        AdornerLayer.SetAdorner((Visual) this,
            new Rectangle {
                Fill = StandardBrushes.HighlightBrush,
                IsHitTestVisible = false,
                Opacity = 0.5,
            });
    }

    void HidePreview() {
        AdornerLayer.SetAdorner((Visual) this, null);
    }
}
