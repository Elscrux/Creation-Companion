namespace CreationEditor.Avalonia.Models;

public enum DockElement {
    /// <summary>
    /// Log.
    /// No parameter required.
    /// </summary>
    Log,
    /// <summary>
    /// Browse records.
    /// No parameter required.
    /// </summary>
    RecordBrowser,
    /// <summary>
    /// Browse cells.
    /// No parameter required.
    /// </summary>
    CellBrowser,
    /// <summary>
    /// Browse assets.
    /// Optional parameter is an absolute directory path to determine the base path for browsing assets.
    /// </summary>
    AssetBrowser,
    /// <summary>
    /// Viewing and modifying the content of cells.
    /// </summary>
    Viewport
}
