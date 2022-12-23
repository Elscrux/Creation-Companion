using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Docking;

public class DockingManagerService : ReactiveObject, IDockingManagerService {
    public DockingManagerVM Root { get; } = new();

    public IObservable<Control> Closed => Root.Closed;

    public void AddControl(Control control, DockConfig config) {
        var dockedItem = new DockedItem(control, Root, config);
        
        switch (config.DockType) {
            case DockType.Layout:
                Root.AddDockedControl(LayoutDockVM.Create(dockedItem), config);
                break;
            case DockType.Document:
                Root.AddToChildDocument(dockedItem, config);
                break;
        }
    }

    public void RemoveControl(Control control) {
        Root.RemoveDockedControl(control);
    }

    public void SetActiveControl(Control control) {
        Root.Focus(control);
        control.Focus();
    }

    public void SaveLayout() {}

    public void LoadLayout() {}
}