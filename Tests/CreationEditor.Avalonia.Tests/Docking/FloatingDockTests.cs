using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using Xunit;
namespace CreationEditor.Avalonia.Tests.Docking;

public sealed class FloatingDockTests {
    private static IDockedItem CreateItem(string header, string? dockableType = null) => new DockedItemVM(new Grid(),
        new DockInfo {
            Header = header,
            DockableType = dockableType,
        });

    [AvaloniaFact]
    public void Float_MovesItemIntoFloatingDock() {
        var root = new DockingManagerVM();
        var item = CreateItem("Floating");

        root.Layout.Add(item, DockConfig.Default);
        root.Float(item, new PixelPoint(100, 100), new Size(400, 300));

        // Item was removed from the layout
        Assert.Empty(root.Layout.LayoutGrid.Children);

        // And added to a new floating dock
        var floatingDock = Assert.Single(root.FloatingDocks);
        Assert.Same(item, floatingDock.Content.Tabs.Single());
        Assert.Same(floatingDock.Content, item.DockParent);
    }

    [AvaloniaFact]
    public void Float_RemovesItemFromSideDock() {
        var root = new DockingManagerVM();
        var item = CreateItem("SideItem");

        root.TopSide.Add(item, DockConfig.Default);
        root.Float(item, new PixelPoint(100, 100), new Size(400, 300));

        Assert.Empty(root.TopSide.Tabs);
        var floatingDock = Assert.Single(root.FloatingDocks);
        Assert.Same(item, floatingDock.Content.Tabs.Single());
    }

    [AvaloniaFact]
    public void FloatGroup_MovesAllItemsInOrder() {
        var root = new DockingManagerVM();
        var itemA = CreateItem("A");
        var itemB = CreateItem("B");

        root.Layout.Add(itemA, DockConfig.Default);
        root.Layout.Add(itemB, new DockConfig { Dock = Dock.Bottom });

        root.Float([itemA, itemB], new PixelPoint(100, 100), new Size(400, 300));

        Assert.Empty(root.Layout.LayoutGrid.Children);
        var floatingDock = Assert.Single(root.FloatingDocks);
        Assert.Equal(2, floatingDock.Content.Tabs.Count);
        Assert.Same(itemA, floatingDock.Content.Tabs[0]);
        Assert.Same(itemB, floatingDock.Content.Tabs[1]);
    }

    [AvaloniaFact]
    public void CleanUp_ClosesEmptyFloatingDock() {
        var root = new DockingManagerVM();
        var item = CreateItem("Floating");

        root.Float(item, new PixelPoint(100, 100), new Size(400, 300));
        var floatingDock = Assert.Single(root.FloatingDocks);

        // Remove the item again - the floating dock has to close
        item.DockParent.Remove(item);

        Assert.Empty(root.FloatingDocks);
        Assert.Null(floatingDock.Window);
    }
}
