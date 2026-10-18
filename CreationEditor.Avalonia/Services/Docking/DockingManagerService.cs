using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Docking;

public sealed class DockingManagerService : ReactiveObject, IDockingManagerService {
    public DockingManagerVM Root { get; } = new();

    public IObservable<IDockedItem> Opened => Root.Opened;
    public IObservable<IDockedItem> Closed => Root.Closed;

    public void AddControl(Control control, DockConfig config) {
        var dockedItem = new DockedItemVM(control, config.DockInfo);

        Root.Add(dockedItem, config);
    }

    public void RemoveControl(Control control) {
        if (Root.TryGetDock(control, out var dockedItem)) {
            dockedItem.DockParent.Remove(dockedItem);
        }
    }

    public T? TryGetControl<T>() {
        return Root.IterateAllChildren()
            .OfType<IDockedItem>()
            .Select(x => x.Control)
            .OfType<T>()
            .FirstOrDefault();
    }

    public Control? TryGetControl(Func<Control, bool> filter) {
        return Root.IterateAllChildren()
            .OfType<IDockedItem>()
            .Select(x => x.Control)
            .FirstOrDefault(filter);
    }

    public void Focus(Control control) {
        if (Root.TryGetDock(control, out var dockedItem)) {
            dockedItem.DockParent.Focus(dockedItem);
            control.Focus();
        }
    }

    public void DetachControls() {
        // The central layout subtree is moved to the new window as a whole, keeping its view instances and bindings intact
        DetachFromVisualParent(Root.Layout.LayoutGrid);

        // Side docked items are re-wrapped by the fresh side dock views of the new window,
        // so their controls are detached from the old wrappers individually
        foreach (var side in new[] { Root.TopSide, Root.BottomSide, Root.LeftSide, Root.RightSide }) {
            foreach (var tab in side.Tabs) {
                DetachFromVisualParent(tab.Control);
            }
        }
    }

    private static void DetachFromVisualParent(Control control) {
        switch (control.GetVisualParent()) {
            case ContentPresenter presenter:
                presenter.Content = null;
                break;
            case Panel panel:
                panel.Children.Remove(control);
                break;
        }
    }
}
