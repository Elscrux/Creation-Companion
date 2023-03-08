using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Services.Docking;

public interface IDockingManagerService {
    public DockingManagerVM Root { get; }

    public void AddControl(Control control, DockConfig config);

    public void RemoveControl(Control control);

    public void Focus(Control control);

    public IObservable<IDockedItem> Opened { get; }
    public IObservable<IDockedItem> Closed { get; }

    public void SaveLayout();
    public void LoadLayout();
}
