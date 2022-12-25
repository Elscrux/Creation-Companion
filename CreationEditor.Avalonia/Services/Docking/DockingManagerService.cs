using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Docking;

public class DockingManagerService : ReactiveObject, IDockingManagerService {
    public DockingManagerVM Root { get; } = new();

    public IObservable<IDockedItem> Closed => Root.Closed;

    public void AddControl(Control control, DockConfig config) {
        var dockedItem = new DockedItem(control, Root, config);
        
        Root.Add(dockedItem, config);
    }

    public void RemoveControl(Control control) {
        if (Root.TryGetDock(control, out var dockedItem)) {
            Root.Remove(dockedItem);
        }
    }

    public void Focus(Control control) {
        if (Root.TryGetDock(control, out var dockedItem)) {
            Root.Focus(dockedItem);
            control.Focus();
        }
    }

    public void SaveLayout() {}

    public void LoadLayout() {}
}