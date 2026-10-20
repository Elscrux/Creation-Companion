using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using CreationEditor.Avalonia.Views.Docking;
using Newtonsoft.Json;
using Serilog;
using Xunit;
namespace CreationEditor.Avalonia.Tests.Docking;

public sealed class DockLayoutPersistenceTests {
    private static DockedItemVM CreateItem(string header, string? dockableType = null) => new(new Grid(),
        new DockInfo {
            Header = header,
            DockableType = dockableType,
        });

    private static readonly IDockableRegistry Registry = new TestDockableRegistry(
        new TestDockable("Log"),
        new TestDockable("RecordBrowser"),
        new TestDockable("CellBrowser"),
        new TestDockable("AssetBrowser"),
        new TestDockable("ScriptEditor"),
        new TestDockable("Viewport"));

    private static (DockLayoutPersistence Persistence, DockingManagerService Service) CreatePersistence() {
        var service = new DockingManagerService();
        return (new DockLayoutPersistence(Log.Logger, service, Registry), service);
    }

    [AvaloniaFact]
    public async Task SaveAndRestore_RoundTripsLayout() {
        // Arrange: build a layout with a side dock, a split layout and a floating dock
        var service = new DockingManagerService();
        var root = service.Root;

        var sideItem = CreateItem("Side", "Log");
        root.TopSide.Add(sideItem, DockConfig.Default);

        var layoutItem = CreateItem("Layout", "Viewport");
        root.Layout.Add(layoutItem, DockConfig.Default);

        var documentItemA = CreateItem("DocumentA", "ScriptEditor");
        var documentItemB = CreateItem("DocumentB", "AssetBrowser");
        root.Layout.Add(documentItemA, new DockConfig { Dock = Dock.Bottom, DockMode = DockMode.Document });
        root.Layout.Add(documentItemB, new DockConfig { DockMode = DockMode.Document });

        var floatingItem = CreateItem("Floating", "RecordBrowser");
        root.Float(floatingItem, new PixelPoint(250, 150), new Size(600, 400));

        var persistence = new DockLayoutPersistence(Log.Logger, service, Registry);
        var memento = persistence.Save();

        // Act: restore into a fresh docking manager
        var (restorePersistence, restoreService) = CreatePersistence();
        await restorePersistence.Restore(memento);
        var restoredMemento = restorePersistence.Save();

        // Assert: the restored layout serializes to the same memento
        var expected = JsonConvert.SerializeObject(memento);
        var actual = JsonConvert.SerializeObject(restoredMemento);
        Assert.Equal(expected, actual);

        // Spot checks on the restored layout (items are recreated, so compare identity fields)
        var restoredSideItem = restoreService.Root.TopSide.Tabs.Single();
        Assert.Equal(sideItem.Header, restoredSideItem.Header);
        Assert.Equal(sideItem.DockableType, restoredSideItem.DockableType);
        var restoredFloat = Assert.Single(restoreService.Root.FloatingDocks);
        Assert.Equal(new PixelPoint(250, 150), restoredFloat.Position);
        Assert.Equal(new Size(600, 400), restoredFloat.Size);
        var restoredFloatingItem = restoredFloat.Content.Tabs.Single();
        Assert.Equal(floatingItem.Header, restoredFloatingItem.Header);
        Assert.Equal(floatingItem.DockableType, restoredFloatingItem.DockableType);
    }

    [AvaloniaFact]
    public async Task Restore_SkipsItemsWithoutDockableType() {
        var (persistence, service) = CreatePersistence();

        var memento = new DockLayoutMemento {
            Top = new DockTabGroupMemento {
                Mode = DockMode.Side,
                Items = [new DockItemMemento { Header = "Unknown", Type = null }],
                ActiveIndex = 0,
            },
        };

        await persistence.Restore(memento);

        // Items without a dockable type can't be recreated - the side dock stays empty
        Assert.Empty(service.Root.TopSide.Tabs);
    }

    [AvaloniaFact]
    public async Task Restore_EqualValuedItemsInDifferentPositions_GetSeparateControls() {
        var (persistence, service) = CreatePersistence();

        // The same dockable can appear multiple times in a saved layout with equal values -
        // those are distinct dockables and must be restored with separate controls
        var memento = new DockLayoutMemento {
            Top = new DockTabGroupMemento {
                Mode = DockMode.Side,
                Items = [new DockItemMemento { Type = "Log", Header = "Log", Size = 150 }],
                ActiveIndex = 0,
            },
            Layout = new DockGridMemento {
                Rows = ["1*"],
                Columns = ["1*"],
                Cells = [
                    new DockCellMemento {
                        Item = new DockItemMemento { Type = "Log", Header = "Log", Size = 150 },
                    },
                ],
            },
        };

        await persistence.Restore(memento);

        var sideControl = service.Root.TopSide.Tabs.Single().Control;
        var layoutControl = service.Root.Layout.LayoutGrid.Children.OfType<DockedControl>().Single().Control;

        Assert.NotSame(sideControl, layoutControl);
    }
    public static DockGrid CreateGrid(DockGridMemento memento) {
        var grid = new DockGrid(new LayoutDockVM(null));
        var window = new Window {
            Width = 1000,
            Height = 800,
            Content = grid,
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        DockLayoutPersistence.RestoreDockGrid(grid, memento, (_, _) => Task.FromResult<Control?>(new Grid())).GetAwaiter().GetResult();
        Dispatcher.UIThread.RunJobs();
        return grid;
    }

    [AvaloniaFact]
    public void Restore_TwoStackedCells_AddsSplitterBetweenThem() {
        var grid = CreateGrid(new DockGridMemento {
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
        var grid = CreateGrid(new DockGridMemento {
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
        var grid = CreateGrid(new DockGridMemento {
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
