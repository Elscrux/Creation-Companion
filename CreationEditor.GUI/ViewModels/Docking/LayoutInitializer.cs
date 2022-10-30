using System.Windows;
using AvalonDock.Layout;
namespace CreationEditor.GUI.ViewModels.Docking;

public class LayoutInitializer : ILayoutUpdateStrategy {
    public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer? destinationContainer) {
        if (destinationContainer?.FindParent<LayoutFloatingWindow>() != null) return false;
        
        if (anchorableToShow.Content is PaneVM paneVM) {
            var pane = layout
                .Descendents()
                .OfType<LayoutAnchorSide>()
                .FirstOrDefault(d => d.Side == paneVM.DockingStatus.AnchorSide);

            if (pane != null) {
                if (!pane.Children.Any()) pane.Children.Add(new LayoutAnchorGroup());

                pane.Children.First().Children.Add(anchorableToShow);
                return true;
            }
        }

        return false;
    }

    public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown) {
        if (anchorableShown.Content is not PaneVM paneVM) return;

        if (paneVM.DockingStatus.IsDocked == anchorableShown.IsAutoHidden) {
            anchorableShown.ToggleAutoHide();
        }

        if (paneVM.DockingStatus.IsDocked) {
            var anchorablePane = anchorableShown.FindParent<LayoutAnchorablePane>();
            if (anchorablePane != null) {
                if (paneVM.DockingStatus.Height > 0) anchorablePane.DockHeight = new GridLength(paneVM.DockingStatus.Height);
                if (paneVM.DockingStatus.Width > 0) anchorablePane.DockWidth = new GridLength(paneVM.DockingStatus.Width);
            }
        } else {
            if (paneVM.DockingStatus.Height > 0) anchorableShown.AutoHideHeight = paneVM.DockingStatus.Height;
            if (paneVM.DockingStatus.Width > 0) anchorableShown.AutoHideWidth = paneVM.DockingStatus.Width;
        }
    }

    
    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer) {
        var pane = layout
            .Descendents()
            .OfType<LayoutDocumentPane>()
            .FirstOrDefault();

        if (pane != null) {
            pane.Children.Add(anchorableToShow);
            return true;
        }

        return false;
    }

    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown) {}
}