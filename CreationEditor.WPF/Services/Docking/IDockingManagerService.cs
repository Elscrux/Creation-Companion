using Avalonia.Controls;
using Dock.Model.Core;
namespace CreationEditor.WPF.Services.Docking;

public interface IDockingManagerService {
    public DockPanel DockingManager { get; }

    public IDock? ActiveDocument { get; }

    public void AddControl<TControl>(TControl control,
        string title,
        Avalonia.Controls.Dock? dock = null)
        where TControl : UserControl;

    public void RemoveControl(UserControl control);

    public void SetActiveControl(UserControl control);

    public IObservable<UserControl> Closed { get; }

    public void SaveLayout();
    public void LoadLayout();
}