using Avalonia.Controls;
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

    public void Focus(Control control) {
        if (Root.TryGetDock(control, out var dockedItem)) {
            dockedItem.DockParent.Focus(dockedItem);
            control.Focus();
        }
    }

    public void SaveLayout() {}

    public void LoadLayout() {}
}