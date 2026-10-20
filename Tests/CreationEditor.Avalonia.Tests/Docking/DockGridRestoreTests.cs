using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Docking;
using Xunit;
namespace CreationEditor.Avalonia.Tests.Docking;

/// <summary>
/// Verifies that restoring a dock grid from its memento re-creates the grid splitters
/// between the restored cells, so panes stay resizable and removal keeps working.
/// </summary>
public sealed class DockGridRestoreTests {
    [AvaloniaFact]
    public void Restore_TwoStackedCells_AddsSplitterBetweenThem() {
        var grid = DockLayoutPersistenceTests.CreateGrid(new DockGridMemento {
            Rows = ["1*", "5", "1*"],
            Columns = ["1*"],
            Cells = [
                new DockCellMemento { Row = 0, Column = 0 },
                new DockCellMemento { Row = 2, Column = 0 },
            ],
        });

        var splitter = grid.Children.OfType<GridSplitter>().Single();
        Assert.Equal(1, Grid.GetRow(splitter));
        Assert.Equal(0, Grid.GetColumn(splitter));
        Assert.Equal(1, Grid.GetColumnSpan(splitter));
    }
    [AvaloniaFact]
    public void Restore_NestedLayout_RestoresSplittersWithCorrectSpans() {
        // Layout shape: a full-width cell on top, two cells below separated by a vertical splitter
        var grid = DockLayoutPersistenceTests.CreateGrid(new DockGridMemento {
            Rows = ["653*", "5", "539*"],
            Columns = ["1388*", "5", "662*"],
            Cells = [
                new DockCellMemento { Row = 0, Column = 0, ColumnSpan = 3 },
                new DockCellMemento { Row = 2, Column = 0 },
                new DockCellMemento { Row = 2, Column = 2 },
            ],
        });

        var splitters = grid.Children.OfType<GridSplitter>().ToList();
        Assert.Equal(2, splitters.Count);

        // Horizontal splitter between the rows spans all columns
        var horizontal = splitters.Single(x => Grid.GetRow(x) == 1);
        Assert.Equal(0, Grid.GetColumn(horizontal));
        Assert.Equal(3, Grid.GetColumnSpan(horizontal));

        // Vertical splitter between the bottom cells only spans the bottom row
        var vertical = splitters.Single(x => Grid.GetColumn(x) == 1);
        Assert.Equal(2, Grid.GetRow(vertical));
        Assert.Equal(1, Grid.GetRowSpan(vertical));
    }
    [AvaloniaFact]
    public void Remove_AfterRestore_RemovesAdjacentSplitter() {
        var grid = DockLayoutPersistenceTests.CreateGrid(new DockGridMemento {
            Rows = ["1*", "5", "1*"],
            Columns = ["1*"],
            Cells = [
                new DockCellMemento { Row = 0, Column = 0 },
                new DockCellMemento { Row = 2, Column = 0 },
            ],
        });

        var cell = grid.Children.First(x => x is not GridSplitter && Grid.GetRow(x) == 2);
        Assert.True(grid.Remove(cell));
        Dispatcher.UIThread.RunJobs();

        Assert.Empty(grid.Children.OfType<GridSplitter>());
        Assert.Equal(1, grid.Children.Count);
        Assert.Equal(1, grid.RowDefinitions.Count);
    }
}
