namespace CreationEditor.Avalonia.Models.Docking;

/// <summary>
/// Serializable memento of the complete docking layout, used to persist the layout as part of the editor state.
/// </summary>
public sealed record DockLayoutMemento {
    public DockTabGroupMemento? Top { get; set; }
    public DockTabGroupMemento? Bottom { get; set; }
    public DockTabGroupMemento? Left { get; set; }
    public DockTabGroupMemento? Right { get; set; }
    public DockGridMemento? Layout { get; set; }
    public IList<FloatingDockMemento> FloatingDocks { get; set; } = [];
}

/// <summary>
/// A group of tabs (document dock or side dock).
/// </summary>
public sealed record DockTabGroupMemento {
    public DockMode Mode { get; set; }
    public IList<DockItemMemento> Items { get; set; } = [];
    public int ActiveIndex { get; set; } = -1;
}

/// <summary>
/// A single docked item, identified by its dockable type so it can be recreated on restore.
/// </summary>
public sealed record DockItemMemento {
    public string? Type { get; set; }
    public string? Header { get; set; }
    public double? Size { get; set; }
}

/// <summary>
/// A dock grid: row and column definitions with their sizes and the cells in between.
/// </summary>
public sealed record DockGridMemento {
    public IList<string> Rows { get; set; } = [];
    public IList<string> Columns { get; set; } = [];
    public IList<DockCellMemento> Cells { get; set; } = [];
}

/// <summary>
/// A cell of a dock grid. Content is either a single docked item, a tab group or a nested grid.
/// </summary>
public sealed record DockCellMemento {
    public int Row { get; set; }
    public int Column { get; set; }
    public int RowSpan { get; set; } = 1;
    public int ColumnSpan { get; set; } = 1;
    public DockItemMemento? Item { get; set; }
    public DockTabGroupMemento? Tabs { get; set; }
    public DockGridMemento? Grid { get; set; }
}

/// <summary>
/// A floating dock window with its position and size.
/// </summary>
public sealed record FloatingDockMemento {
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public DockTabGroupMemento Content { get; set; } = new();
}
