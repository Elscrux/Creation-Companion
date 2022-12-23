using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using DockingManagerVM = CreationEditor.Avalonia.ViewModels.Docking.DockingManagerVM;
namespace CreationEditor.Avalonia.Services.Docking;

public interface IDockingManagerService {
    public DockingManagerVM Root { get; }

    public void AddControl(Control control, DockConfig config);

    public void RemoveControl(Control control);

    public void SetActiveControl(Control control);

    public IObservable<Control> Closed { get; }

    public void SaveLayout();
    public void LoadLayout();
}
