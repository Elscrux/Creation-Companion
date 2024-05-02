using Avalonia.Controls.PanAndZoom;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
namespace CreationEditor.Avalonia.Behavior.ZoomBorder;

public sealed class LimitPanAndZoomToSize : Behavior<global::Avalonia.Controls.PanAndZoom.ZoomBorder> {
    protected override void OnAttachedToVisualTree() {
        if (AssociatedObject is null) return;

        AssociatedObject.ZoomChanged += ZoomChanged;
        AssociatedObject.MinZoomX = 1;
        AssociatedObject.MinZoomY = 1;
        AssociatedObject.MaxOffsetX = 0;
        AssociatedObject.MaxOffsetY = 0;
    }

    protected override void OnDetachedFromVisualTree() {
        if (AssociatedObject is null) return;

        AssociatedObject.ZoomChanged -= ZoomChanged;
        AssociatedObject.MinZoomX = double.NegativeInfinity;
        AssociatedObject.MinZoomY = double.NegativeInfinity;
        AssociatedObject.MaxOffsetX = double.PositiveInfinity;
        AssociatedObject.MaxOffsetY = double.PositiveInfinity;
    }

    private void ZoomChanged(object sender, ZoomChangedEventArgs e) {
        if (AssociatedObject is null) return;

        AssociatedObject.MinOffsetX = AssociatedObject.Bounds.Width * (1 - e.ZoomX);
        AssociatedObject.MinOffsetY = AssociatedObject.Bounds.Height * (1 - e.ZoomY);
        
        // Snap to offset bounds
        var isOffsetXLessThanMin = AssociatedObject.OffsetX < AssociatedObject.MinOffsetX;
        var isOffsetYLessThanMin = AssociatedObject.OffsetY < AssociatedObject.MinOffsetY;

        if (isOffsetXLessThanMin || isOffsetYLessThanMin) {
            var newOffsetX = isOffsetXLessThanMin ? AssociatedObject.MinOffsetX : AssociatedObject.OffsetX;
            var newOffsetY = isOffsetYLessThanMin ? AssociatedObject.MinOffsetY : AssociatedObject.OffsetY;

            Dispatcher.UIThread.Post(() => AssociatedObject.ContinuePanTo(newOffsetX, newOffsetY));
        }
    }
}
