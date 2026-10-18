using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Services.Docking;

public interface IDockLayoutPersistence {
    /// <summary>
    /// Creates a serializable memento of the current docking layout.
    /// </summary>
    DockLayoutMemento Save();

    /// <summary>
    /// Restores the docking layout from the given memento.
    /// </summary>
    Task Restore(DockLayoutMemento memento);
}
