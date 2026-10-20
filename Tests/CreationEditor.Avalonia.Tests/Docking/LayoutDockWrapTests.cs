using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using CreationEditor.Avalonia.Views.Docking;
using Xunit;
namespace CreationEditor.Avalonia.Tests.Docking;

/// <summary>
/// Reproduces the "already has a visual parent" crash when adding a docked item into a
/// layout dock that already contains another item (the wrap path).
/// The docking manager view is rooted in a real (headless) window so layout passes run
/// through the actual layout manager and content presenters take their children.
/// </summary>
public sealed class LayoutDockWrapTests {
    private static IDockedItem CreateItem(string header) => new DockedItemVM(new Grid(), new DockInfo { Header = header });

    private static Window CreateWindow(out DockingManagerVM root) {
        root = new DockingManagerVM();
        var window = new Window {
            Width = 1000,
            Height = 800,
            Content = new DockingManager { DataContext = root },
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    [AvaloniaFact]
    public void AddSecondItemToLayoutDock_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        // Drop a second item into the layout dock - wraps the existing control into a nested layout
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Right });
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void MoveItemBetweenLayoutDocks_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Right });
        Dispatcher.UIThread.RunJobs();

        // Simulate a drag of A onto the layout dock again (remove + re-add)
        root.Layout.Remove(itemA);
        root.Layout.Add(itemA, new DockConfig { Dock = Dock.Left });
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void ConvertToDocumentDock_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        // Center drop: convert the docked control into a document dock presenting the same view
        var dockedControl = root.Layout.LayoutGrid.GetVisualDescendants().OfType<DockedControl>().Single();
        root.Layout.ConvertToDocumentDock(dockedControl, [itemB]);
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void ConvertToDocumentDock_FromOtherLayoutDock_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        // Two layout docks: adding B wraps A into a nested layout dock
        root.Layout.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Right });
        Dispatcher.UIThread.RunJobs();

        // Drag A onto B as document (center drop) - the drop target is B's own (nested) layout dock
        root.Layout.Remove(itemA);
        var dockedControlB = root.Layout.LayoutGrid
            .GetVisualDescendants()
            .OfType<DockedControl>()
            .Single(x => x.DockedItem == itemB);
        var targetLayoutDock = (LayoutDockVM) dockedControlB.DockParent;
        targetLayoutDock.ConvertToDocumentDock(dockedControlB, [itemA]);
        Dispatcher.UIThread.RunJobs();

        // B's control must now be presented by the document dock, not the old DockedControl
        var documentDock = root.Layout.LayoutGrid
            .GetVisualDescendants()
            .OfType<DocumentDock>()
            .Single();
        var parent = Assert.IsType<ContentPresenter>(itemA.Control.GetVisualParent());
        Assert.Contains(parent, documentDock.GetVisualDescendants());
    }

    [AvaloniaFact]
    public void MoveItemIntoDocumentDock_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Right, DockMode = DockMode.Document });
        Dispatcher.UIThread.RunJobs();

        // Simulate dragging A as a tab into the document dock of B
        root.Layout.Remove(itemA);
        root.Layout.Add(itemA, new DockConfig { DockMode = DockMode.Document });
        Dispatcher.UIThread.RunJobs();
    }

    [AvaloniaFact]
    public void MoveSideDockItemIntoLayoutAsDocument_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        // Default startup layout: items docked in the side docks
        root.LeftSide.Add(itemA, DockConfig.Default);
        root.RightSide.Add(itemB, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        // Drag A from the left side dock into the layout as document (center drop)
        root.LeftSide.Remove(itemA);
        root.Layout.Add(itemA, new DockConfig { DockMode = DockMode.Document });
        Dispatcher.UIThread.RunJobs();

        // A's control must be presented exactly once - by the new document dock
        Assert.NotNull(itemA.Control.GetVisualParent());
    }

    [AvaloniaFact]
    public void MoveSideDockItemIntoLayoutEdge_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.LeftSide.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        // Drag A from the side dock onto the layout dock edge (split)
        root.LeftSide.Remove(itemA);
        root.Layout.Add(itemA, new DockConfig { Dock = Dock.Bottom });
        Dispatcher.UIThread.RunJobs();

        Assert.NotNull(itemA.Control.GetVisualParent());
    }

    [AvaloniaFact]
    public void MoveLayoutItemIntoSideDock_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        root.LeftSide.Add(itemB, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        // Drag A from the layout into the side dock
        root.Layout.Remove(itemA);
        root.LeftSide.Add(itemA, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        Assert.NotNull(itemA.Control.GetVisualParent());
    }

    [AvaloniaFact]
    public void WrappedItem_KeepsOriginalItemIdentity() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Right });
        Dispatcher.UIThread.RunJobs();

        // The wrap path must not wrap the wrapper: the docked control for A must wrap the
        // original item, not another DockedControl (wrapper chains break removal identity)
        var dockedControlA = root.Layout.LayoutGrid
            .GetVisualDescendants()
            .OfType<DockedControl>()
            .Single(x => x.DockedItem == itemA);
        Assert.Same(itemA, dockedControlA.DockedItem);
    }

    [AvaloniaFact]
    public void DragWrappedItemAsDocument_MeasureDoesNotThrow() {
        CreateWindow(out var root);
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        // Two layout docks via the wrap path
        root.Layout.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Right });
        Dispatcher.UIThread.RunJobs();

        // Drag A onto B as document: remove + add as document tab.
        // With a wrapper chain the removal silently fails and the item ends up presented
        // by both the old layout dock and the new document dock - crashing the layout pass.
        root.Layout.Remove(itemA);
        Assert.DoesNotContain(itemA, root.Layout.IterateAllChildren().OfType<IDockedItem>());
        root.Layout.Add(itemA, new DockConfig { DockMode = DockMode.Document });
        Dispatcher.UIThread.RunJobs();

        // A's control must be presented exactly once
        Assert.NotNull(itemA.Control.GetVisualParent());
    }
}
