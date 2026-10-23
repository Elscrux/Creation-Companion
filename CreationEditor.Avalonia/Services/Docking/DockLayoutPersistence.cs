using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using CreationEditor.Avalonia.Views.Docking;
using CreationEditor.Resources.Comparer;
using Noggog;
using Serilog;
namespace CreationEditor.Avalonia.Services.Docking;

/// <summary>
/// Saves and restores the docking layout from <see cref="DockLayoutMemento"/> mementos.
/// </summary>
public sealed class DockLayoutPersistence(
    ILogger logger,
    IDockingManagerService dockingManagerService,
    IDockableRegistry dockableRegistry) : IDockLayoutPersistence {
    public DockLayoutMemento Save() => dockingManagerService.Root.SaveLayout();

    public async Task Restore(DockLayoutMemento memento) {
        // Use a reference-equality dictionary to ensure that equal-valued memento items in different layout positions are treated as distinct dockables
        var items = new Dictionary<DockItemMemento, IDockedItem>(ReferenceEqualsComparer<DockItemMemento>.Default);
        foreach (var item in CollectItems(memento)) {
            if (item.Type is null) continue;

            var dockable = dockableRegistry.GetByType(item.Type);
            if (dockable is null) continue;

            var control = await Dispatcher.UIThread.InvokeAsync(() => dockable.CreateControl());
            if (control is null) continue;

            items[item] = new DockedItemVM(
                control,
                new DockInfo {
                    Header = item.Header ?? string.Empty,
                    IconSource = await Dispatcher.UIThread.InvokeAsync(() => IconConverters.ToIconSource(dockable.Icon)),
                    Size = item.Size,
                    DockableType = item.Type,
                });
        }

        await Dispatcher.UIThread.InvokeAsync(() => RestoreLayout(memento, items));
    }

    private async Task RestoreLayout(DockLayoutMemento memento, Dictionary<DockItemMemento, IDockedItem> items) {
        var root = dockingManagerService.Root;
        var wasReporting = root.IsReporting;
        root.IsReporting = false;
        try {
            RestoreSide(root.TopSide, memento.Top, items);
            RestoreSide(root.BottomSide, memento.Bottom, items);
            RestoreSide(root.LeftSide, memento.Left, items);
            RestoreSide(root.RightSide, memento.Right, items);

            await RestoreLayout(root.Layout, memento.Layout, CreateCellContent);

            foreach (var floating in memento.FloatingDocks) {
                var floatItems = floating.Content.Items
                    .Select(items.GetValueOrDefault)
                    .WhereNotNull()
                    .ToList();
                if (floatItems.Count == 0) continue;

                var floatingDock = root.Float(
                    floatItems,
                    new PixelPoint((int) floating.X, (int) floating.Y),
                    new Size(floating.Width, floating.Height));

                var activeIndex = floating.Content.ActiveIndex;
                if (activeIndex >= 0 && activeIndex < floatItems.Count) {
                    floatingDock.Content.Focus(floatItems[activeIndex]);
                }
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to restore docking layout: {Message}", e.Message);
        } finally {
            root.IsReporting = wasReporting;
        }
        return;

        async Task<Control?> CreateCellContent(DockCellMemento cell, DockContainerVM? owner) {
            // Single docked item
            if (cell.Item is not null && items.TryGetValue(cell.Item, out var item)) {
                item.DockParent = owner!;
                return new DockedControl(item);
            }

            // Tab group (document or side dock)
            if (cell.Tabs is not null) {
                var tabItems = cell.Tabs.Items
                    .Select(items.GetValueOrDefault)
                    .WhereNotNull()
                    .ToList();
                if (tabItems.Count == 0) return null;

                TabbedDockVM tabbedDock = cell.Tabs.Mode switch {
                    DockMode.Side => new SideDockVM(owner!),
                    _ => new DocumentDockVM(owner!),
                };
                foreach (var tabItem in tabItems) {
                    tabbedDock.Add(tabItem, DockConfig.Default);
                }

                var activeIndex = cell.Tabs.ActiveIndex;
                if (activeIndex >= 0 && activeIndex < tabItems.Count) {
                    tabbedDock.Focus(tabItems[activeIndex]);
                }

                return cell.Tabs.Mode switch {
                    DockMode.Side => new SideDock { DataContext = tabbedDock },
                    _ => new DocumentDock((DocumentDockVM) tabbedDock),
                };
            }

            // Nested layout grid
            if (cell.Grid is not null) {
                var layoutDock = new LayoutDockVM(owner);
                await RestoreLayout(layoutDock, cell.Grid, CreateCellContent);
                return layoutDock.LayoutGrid;
            }

            return null;
        }
    }

    public static async Task RestoreLayout(
        LayoutDockVM layoutDockVM,
        DockGridMemento? memento,
        Func<DockCellMemento, DockContainerVM?, Task<Control?>> createCellContent) {
        if (memento is null) return;

        await RestoreDockGrid(layoutDockVM.LayoutGrid, memento, createCellContent);
    }

    /// <summary>
    /// Restores a dock grid from its memento, recreating row and column definitions and cells.
    /// </summary>
    public static async Task RestoreDockGrid(DockGrid dockGrid, DockGridMemento memento, Func<DockCellMemento, DockContainerVM?, Task<Control?>> createCellContent) {
        dockGrid.Children.Clear();
        dockGrid.RowDefinitions = [.. memento.Rows.Select(x => new RowDefinition(ParseGridLength(x)))];
        dockGrid.ColumnDefinitions = [.. memento.Columns.Select(x => new ColumnDefinition(ParseGridLength(x)))];

        foreach (var cell in memento.Cells) {
            var content = await createCellContent(cell, dockGrid.DataContext as DockContainerVM);
            if (content is null) continue;

            dockGrid.Children.Add(content);
            Grid.SetRow(content, cell.Row);
            Grid.SetColumn(content, cell.Column);
            Grid.SetRowSpan(content, cell.RowSpan);
            Grid.SetColumnSpan(content, cell.ColumnSpan);
        }

        RestoreGridSplitters(dockGrid);
    }

    /// <summary>
    /// Re-adds the grid splitters between restored cells. The memento only stores content cells,
    /// so splitter positions are inferred from the fixed-size rows and columns between them and
    /// spanned over the cells adjacent to them.
    /// </summary>
    private static void RestoreGridSplitters(DockGrid dockGrid) {
        var cells = dockGrid.Children.ToList();
        if (cells.Count == 0) return;

        var splitters = CreateSplitters(dockGrid, cells);

        // Interleave cells and splitters so every cell except the first is directly preceded by
        // an adjacent splitter, matching the order in which the live layout adds them.
        // Removal relies on that order to find the splitter belonging to a removed cell.
        var firstCell = cells[0];
        var ordered = new List<Control> { firstCell };
        cells.RemoveAt(0);
        var isFirstPair = true;

        while (cells.Count > 0 && splitters.Count > 0) {
            var pair = (isFirstPair
                    ? splitters.Where(splitter => IsAdjacent(splitter, firstCell))
                    : splitters)
                .SelectMany(splitter => cells.Where(cell => IsAdjacent(splitter, cell)).Select(cell => (Splitter: (GridSplitter?) splitter, Cell: (Control?) cell)))
                .FirstOrDefault();
            if (pair.Splitter is null || pair.Cell is null) break;

            splitters.Remove(pair.Splitter);
            cells.Remove(pair.Cell);
            ordered.Add(pair.Splitter);
            ordered.Add(pair.Cell);
            isFirstPair = false;
        }

        ordered.AddRange(cells);
        ordered.AddRange(splitters);

        dockGrid.Children.Clear();
        foreach (var child in ordered) {
            dockGrid.Children.Add(child);
        }
    }

    private static List<GridSplitter> CreateSplitters(DockGrid dockGrid, List<Control> cells) {
        var cellPositions = cells
            .Select(child => (Row: Grid.GetRow(child), RowSpan: Grid.GetRowSpan(child), Column: Grid.GetColumn(child), ColumnSpan: Grid.GetColumnSpan(child)))
            .ToList();
        var splitters = new List<GridSplitter>();

        for (var row = 0; row < dockGrid.RowDefinitions.Count; row++) {
            if (!IsSplitterBand(dockGrid, dockGrid.RowDefinitions[row].Height)) continue;

            // The splitter spans the columns of the cells directly above and below it
            var adjacentColumns = cellPositions
                .Where(cell => cell.Row + cell.RowSpan == row || cell.Row == row + 1)
                .Select(cell => (Start: cell.Column, End: cell.Column + cell.ColumnSpan))
                .ToList();
            if (adjacentColumns.Count == 0) continue;

            var splitter = new GridSplitter { Background = Brushes.Transparent };
            Grid.SetRow(splitter, row);
            Grid.SetColumn(splitter, adjacentColumns.Min(x => x.Start));
            Grid.SetColumnSpan(splitter, adjacentColumns.Max(x => x.End) - adjacentColumns.Min(x => x.Start));
            splitters.Add(splitter);
        }

        for (var column = 0; column < dockGrid.ColumnDefinitions.Count; column++) {
            if (!IsSplitterBand(dockGrid, dockGrid.ColumnDefinitions[column].Width)) continue;

            // The splitter spans the rows of the cells directly left and right of it
            var adjacentRows = cellPositions
                .Where(cell => cell.Column + cell.ColumnSpan == column || cell.Column == column + 1)
                .Select(cell => (Start: cell.Row, End: cell.Row + cell.RowSpan))
                .ToList();
            if (adjacentRows.Count == 0) continue;

            var splitter = new GridSplitter { Background = Brushes.Transparent };
            Grid.SetColumn(splitter, column);
            Grid.SetRow(splitter, adjacentRows.Min(x => x.Start));
            Grid.SetRowSpan(splitter, adjacentRows.Max(x => x.End) - adjacentRows.Min(x => x.Start));
            splitters.Add(splitter);
        }

        return splitters;
    }

    private static bool IsAdjacent(GridSplitter splitter, Control cell) {
        var splitterRow = Grid.GetRow(splitter);
        var splitterRowSpan = Grid.GetRowSpan(splitter);
        var splitterColumn = Grid.GetColumn(splitter);
        var splitterColumnSpan = Grid.GetColumnSpan(splitter);
        var cellRow = Grid.GetRow(cell);
        var cellRowSpan = Grid.GetRowSpan(cell);
        var cellColumn = Grid.GetColumn(cell);
        var cellColumnSpan = Grid.GetColumnSpan(cell);

        return ((splitterRow == cellRow - 1 || splitterRow == cellRow + cellRowSpan)
             && Overlaps(splitterColumn, splitterColumnSpan, cellColumn, cellColumnSpan))
         || ((splitterColumn == cellColumn - 1 || splitterColumn == cellColumn + cellColumnSpan)
             && Overlaps(splitterRow, splitterRowSpan, cellRow, cellRowSpan));
    }

    private static bool Overlaps(int startA, int spanA, int startB, int spanB)
        => startA < startB + spanB && startB < startA + spanA;

    private static bool IsSplitterBand(DockGrid dockGrid, GridLength length) => length.IsAbsolute && length.Value < dockGrid.MinLayoutSize;

    private static GridLength ParseGridLength(string value) {
        if (string.IsNullOrEmpty(value)) return GridLength.Star;

        if (value.EndsWith('*')) {
            var number = value[..^1];
            return number.Length == 0
                ? GridLength.Star
                : new GridLength(double.Parse(number, CultureInfo.InvariantCulture), GridUnitType.Star);
        }

        return new GridLength(double.Parse(value, CultureInfo.InvariantCulture), GridUnitType.Pixel);
    }

    public static void RestoreSide(SideDockVM side, DockTabGroupMemento? memento, Dictionary<DockItemMemento, IDockedItem> items) {
        if (memento is null) return;

        var dockedItems = memento.Items
            .Select(items.GetValueOrDefault)
            .WhereNotNull()
            .ToList();

        foreach (var item in dockedItems) {
            // If an item of that kind is already present, skip it
            var foundItem = side.Tabs.FirstOrDefault(t => t.DockableType == item.DockableType);
            if (foundItem is not null) continue;

            side.Add(item, DockConfig.Default);
        }

        var activeIndex = memento.ActiveIndex;
        if (activeIndex >= 0 && activeIndex < dockedItems.Count) {
            side.Focus(dockedItems[activeIndex]);
        }
    }

    private static IEnumerable<DockItemMemento> CollectItems(DockLayoutMemento memento) {
        foreach (var group in new[] { memento.Top, memento.Bottom, memento.Left, memento.Right }) {
            if (group is null) continue;

            foreach (var item in group.Items) {
                yield return item;
            }
        }

        if (memento.Layout is not null) {
            foreach (var item in CollectGridItems(memento.Layout)) {
                yield return item;
            }
        }

        foreach (var floating in memento.FloatingDocks) {
            foreach (var item in floating.Content.Items) {
                yield return item;
            }
        }
    }

    private static IEnumerable<DockItemMemento> CollectGridItems(DockGridMemento grid) {
        foreach (var cell in grid.Cells) {
            if (cell.Item is not null) yield return cell.Item;

            if (cell.Tabs is not null) {
                foreach (var item in cell.Tabs.Items) {
                    yield return item;
                }
            }

            if (cell.Grid is not null) {
                foreach (var item in CollectGridItems(cell.Grid)) {
                    yield return item;
                }
            }
        }
    }
}
