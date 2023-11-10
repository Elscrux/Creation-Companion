using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public sealed class DockGrid : Grid {
    private double MinLayoutSize { get; set; } = 50;
    private double GridSplitterSize { get; set; } = 5;

    public DockGrid(IDockObject dataContext) {
        DataContext = dataContext;
        Name = nameof(DockGrid);
        RowDefinitions = new RowDefinitions { new(GridLength.Star) { MinHeight = MinLayoutSize } };
        ColumnDefinitions = new ColumnDefinitions { new(GridLength.Star) { MinWidth = MinLayoutSize } };
    }

    protected override Size ArrangeOverride(Size arrangeSize) {
        if (DataContext is DockingManagerVM dockingManagerVM) {
            dockingManagerVM.UpdateSize();
        }

        return base.ArrangeOverride(arrangeSize);
    }

    public void Add(Control control, DockConfig config) {
        // If there is no content in the layout grid, add something and return
        if (Children.Count == 0) {
            ColumnDefinitions[0].Width = config.GridSize;
            RowDefinitions[0].Height = config.GridSize;
            Children.Add(control);
            return;
        }

        var rowSpan = RowDefinitions.Count;
        var columnSpan = ColumnDefinitions.Count;
        var gridSplitter = new GridSplitter { Background = Brushes.Transparent };
        var gridSplitterSize = new GridLength(GridSplitterSize);

        switch (config.Dock) {
            case Dock.Top:
                // Increment row index
                foreach (var child in Children.Cast<Control>()) {
                    SetRow(child, GetRow(child) + 2);
                }

                // Add grid splitter
                RowDefinitions.Insert(0, new RowDefinition(gridSplitterSize));
                Children.Add(gridSplitter);
                SetRow(gridSplitter, 1);
                SetColumnSpan(gridSplitter, columnSpan);

                // Add new docking manager for control
                RowDefinitions.Insert(0, new RowDefinition(GridLength.Star) { MinHeight = MinLayoutSize, Height = config.GridSize });
                Children.Add(control);
                SetRow(control, 0);
                SetColumnSpan(control, columnSpan);

                break;
            case Dock.Bottom:
                // Add grid splitter
                RowDefinitions.Add(new RowDefinition(gridSplitterSize));
                Children.Add(gridSplitter);
                SetRow(gridSplitter, RowDefinitions.Count - 1);
                SetColumnSpan(gridSplitter, columnSpan);

                // Add new docking manager for control
                RowDefinitions.Add(new RowDefinition(GridLength.Star) { MinHeight = MinLayoutSize, Height = config.GridSize });
                Children.Add(control);
                SetRow(control, RowDefinitions.Count - 1);
                SetColumnSpan(control, columnSpan);

                break;
            case Dock.Left:
                // Increment column index
                foreach (var child in Children.Cast<Control>()) {
                    SetColumn(child, GetColumn(child) + 2);
                }

                // Add grid splitter
                ColumnDefinitions.Insert(0, new ColumnDefinition(gridSplitterSize));
                Children.Add(gridSplitter);
                SetColumn(gridSplitter, 1);
                SetRowSpan(gridSplitter, rowSpan);

                // Add new docking manager for control
                ColumnDefinitions.Insert(0, new ColumnDefinition(GridLength.Star) { MinWidth = MinLayoutSize, Width = config.GridSize });
                Children.Add(control);
                SetColumn(control, 0);
                SetRowSpan(control, rowSpan);

                break;
            case Dock.Right:
                // Add grid splitter
                ColumnDefinitions.Add(new ColumnDefinition(gridSplitterSize));
                Children.Add(gridSplitter);
                SetColumn(gridSplitter, ColumnDefinitions.Count - 1);
                SetRowSpan(gridSplitter, rowSpan);

                // Add new docking manager for control
                ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star) { MinWidth = MinLayoutSize, Width = config.GridSize });
                Children.Add(control);
                SetColumn(control, ColumnDefinitions.Count - 1);
                SetRowSpan(control, rowSpan);

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(config));
        }

    }

    public bool Remove(Control control) {
        var controlIndex = Children.IndexOf(control);
        if (controlIndex < 0) return false;

        // We need to remove the dock from the layout
        // If this was the only entry, we're done
        if (Children.Count == 1) {
            Children.RemoveAt(0);
            return true;
        }

        // Otherwise adjust remaining controls accordingly
        // Get control status
        var controlRow = GetRow(control);
        var controlColumn = GetColumn(control);

        var controlRowSpan = GetRowSpan(control);
        var controlColumnSpan = GetColumnSpan(control);

        // Remove control
        Children.RemoveAt(controlIndex);
        if (controlIndex == 0) {
            // The grid splitter will always be here for the first control
            if (Children.Count > 0) Children.RemoveAt(0);
        } else {
            // The grid splitter will always be here for other controls
            Children.RemoveAt(controlIndex - 1);
        }

        bool changeRow;
        if (controlRowSpan > 1) {
            // If we span over multiple rows, remove the column we're in 
            changeRow = false;
        } else if (controlColumnSpan > 1) {
            // If we span over multiple columns, remove the row we're in 
            changeRow = true;
        } else {
            // If our size is 1x1, check if there are any other children in the grid that are fully
            // subsumed by our control by row or column - don't delete the row or column in that case
            var removeRow = true;
            var removeColumn = true;

            foreach (var child in Children.OfType<Control>()) {
                var currentRow = GetRow(child);
                var currentRowSpan = GetRowSpan(child);

                var currentColumn = GetColumn(child);
                var currentColumnSpan = GetColumnSpan(child);

                if (currentRow >= controlRow
                 && currentRow + currentRowSpan <= controlRow + controlRowSpan) {
                    removeRow = false;
                }

                if (currentColumn >= controlColumn
                 && currentColumn + currentColumnSpan <= controlColumn + controlColumnSpan) {
                    removeColumn = false;
                }
            }

            if (removeRow && removeColumn) {
                throw new Exception("Both row, and column can be deleted. This state isn't allowed by definition");
            } else if (removeRow) {
                changeRow = true;
            } else if (removeColumn) {
                changeRow = false;
            } else {
                throw new Exception("Both row, and column can't be deleted. This state isn't allowed by definition");
            }
        }

        if (changeRow) {
            // Adjust remaining control positions
            foreach (var child in Children) {
                if (child is null) continue;

                var currentRow = GetRow(child);
                var currentRowSpan = GetRowSpan(child);

                if (currentRow + currentRowSpan - 1 < controlRow) continue;

                // We'll be affected by the row changes
                if (currentRow <= controlRow) {
                    // We're inside the row that is removed - decrease size
                    if (currentRowSpan > 2) {
                        SetRowSpan(child, currentRowSpan - 2);
                    } else {
                        throw new Exception($"To small to decrease span {child.GetType().Name}");
                    }
                } else {
                    // We're outside the row, but the row removal will affect us - change row
                    SetRow(child, currentRow - 2);
                }
            }

            // Remove rows
            if (RowDefinitions.Count > 1) {
                RowDefinitions.RemoveAt(controlRow);
                RowDefinitions.RemoveAt(RowDefinitions.Count > controlRow ? controlRow : RowDefinitions.Count - 1);
            }
        } else {
            // Adjust remaining control positions
            foreach (var child in Children) {
                if (child is null) continue;

                var currentColumn = GetColumn(child);
                var currentColumnSpan = GetColumnSpan(child);

                if (currentColumn + currentColumnSpan - 1 < controlColumn) continue;

                // We'll be affected by the column changes
                if (currentColumn <= controlColumn) {
                    // We're inside the column that is removed - decrease size
                    if (currentColumnSpan > 2) {
                        SetColumnSpan(child, currentColumnSpan - 2);
                    } else {
                        throw new Exception($"To small to decrease span {child.GetType().Name}");
                    }
                } else {
                    // We're outside the column, but the column removal will affect us - change column
                    SetColumn(child, currentColumn - 2);
                }
            }

            // Remove columns
            if (ColumnDefinitions.Count > 1) {
                ColumnDefinitions.RemoveAt(controlColumn);
                ColumnDefinitions.RemoveAt(ColumnDefinitions.Count > controlColumn ? controlColumn : ColumnDefinitions.Count - 1);
            }
        }

        // Clean up unnecessary rows and columns
        if (Children.Count <= 1) {
            if (ColumnDefinitions.Count > 1) {
                ColumnDefinitions.RemoveAt(controlColumn);
                ColumnDefinitions.RemoveAt(ColumnDefinitions.Count > controlColumn ? controlColumn : ColumnDefinitions.Count - 1);
            } else if (RowDefinitions.Count > 1) {
                RowDefinitions.RemoveAt(controlRow);
                RowDefinitions.RemoveAt(RowDefinitions.Count > controlRow ? controlRow : RowDefinitions.Count - 1);
            }
        }

        return true;
    }

    public Size AdjustSize(Grid? parentGrid = null) {
        // Recursively get and adjust sizes of all children
        var columnSizes = new double[ColumnDefinitions.Count];
        var rowSizes = new double[RowDefinitions.Count];
        foreach (var child in Children.OfType<Control>()) {
            var childColumn = GetColumn(child);
            var childRow = GetRow(child);

            var childSize = child switch {
                GridSplitter => new Size(GridSplitterSize, GridSplitterSize),
                DockGrid dockGrid => dockGrid.AdjustSize(this),
                _ => new Size(ColumnDefinitions[childColumn].MinWidth, RowDefinitions[childRow].MinHeight)
            };

            if (columnSizes[childColumn] < childSize.Width) {
                columnSizes[childColumn] = childSize.Width;
            }

            if (rowSizes[childRow] < childSize.Height) {
                rowSizes[childRow] = childSize.Height;
            }
        }

        // Calculate own size
        var size = new Size(columnSizes.Sum(), rowSizes.Sum());

        if (parentGrid is not null) {
            var row = GetRow(this);
            var column = GetColumn(this);

            parentGrid.RowDefinitions[row].MinHeight = size.Height;
            parentGrid.ColumnDefinitions[column].MinWidth = size.Width;
        }

        return size;
    }
}
