using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Services.Docking;

public interface IDockingManagerService {
    DockingManagerVM Root { get; }

    /// <summary>
    /// Dock a control in the docking manager
    /// </summary>
    /// <param name="control">Control to dock</param>
    /// <param name="config">Config for the new dock</param>
    void AddControl(Control control, DockConfig config);

    /// <summary>
    /// Remove a control from the docking manager
    /// </summary>
    /// <param name="control">Control to remove</param>
    void RemoveControl(Control control);

    /// <summary>
    /// Focus a control in the docking manager
    /// </summary>
    /// <param name="control">Control to focus</param>
    void Focus(Control control);

    /// <summary>
    /// Emits when a new dock is opened
    /// </summary>
    IObservable<IDockedItem> Opened { get; }
    /// <summary>
    /// Emits when a dock was closed
    /// </summary>
    IObservable<IDockedItem> Closed { get; }

    void SaveLayout();
    void LoadLayout();
}
