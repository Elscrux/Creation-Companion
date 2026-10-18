using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Models.Docking;
namespace CreationEditor.Avalonia.Services.Docking;

/// <summary>
/// Definition of a dockable view.
/// Registered via DI and collected by the <see cref="IDockableRegistry"/>.
/// </summary>
public interface IDockableDefinition {
    /// <summary>
    /// Stable type of the dockable, used for layout persistence and settings.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Display header of the docked view.
    /// </summary>
    string Header { get; }

    /// <summary>
    /// Optional icon of the docked view.
    /// </summary>
    object? Icon { get; }

    /// <summary>
    /// Menu category of the dockable as a path, e.g. <c>View</c> for a direct entry in the View menu or <c>View/Tools</c> for a nested submenu.
    /// Segments outside <c>View</c> are nested under the View menu.
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Optional hotkey that opens the dockable, shown in the menu and global actions.
    /// </summary>
    KeyGesture? HotKey { get; }

    /// <summary>
    /// Preferred size of the docked view, if any.
    /// </summary>
    double? Size { get; }

    /// <summary>
    /// Whether the docked view can be closed by the user.
    /// </summary>
    bool CanClose { get; }

    /// <summary>
    /// Default dock mode used when the dockable is opened without an explicit mode.
    /// </summary>
    DockMode DockMode { get; }

    /// <summary>
    /// Default dock side used when the dockable is opened without an explicit side.
    /// </summary>
    Dock Dock { get; }

    /// <summary>
    /// Default grid size used when the dockable is docked into the layout.
    /// </summary>
    GridLength GridSize { get; }

    /// <summary>
    /// Create a new control instance of the dockable.
    /// Instance policies (e.g. single instance restrictions) are an implementation detail of the definition:
    /// return <see langword="null"/> if no instance may be created right now.
    /// </summary>
    /// <param name="parameter">Optional custom parameter, interpreted by the definition (e.g. a data source link).</param>
    /// <returns>The created control, or <see langword="null"/> if creation is not possible.</returns>
    Task<Control?> CreateControl(object? parameter = null);
}
