using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using CreationEditor.Avalonia.Views.Docking;
using Xunit;
namespace CreationEditor.Avalonia.Tests.Docking;

public sealed class LayoutDockConversionTests {
    private static IDockedItem CreateItem(string header, string? dockableType = null) => new DockedItemVM(new Grid(),
        new DockInfo {
            Header = header,
            DockableType = dockableType,
        });

    [AvaloniaFact]
    public void ConvertToDocumentDock_CreatesDocumentDockWithBothTabs() {
        var root = new DockingManagerVM();
        var existingItem = CreateItem("Existing");
        var draggedItem = CreateItem("Dragged");

        root.Layout.Add(existingItem, DockConfig.Default);
        var dockedControl = root.Layout.LayoutGrid.Children.OfType<DockedControl>().Single();

        root.Layout.ConvertToDocumentDock(dockedControl, [draggedItem]);

        var documentDock = root.Layout.LayoutGrid.Children
            .OfType<DocumentDock>()
            .Select(dock => dock.DataContext)
            .OfType<DocumentDockVM>()
            .Single();

        // The existing item must be the first tab, the dragged item the second
        Assert.Equal(2, documentDock.Tabs.Count);
        Assert.Same(existingItem, documentDock.Tabs[0]);
        Assert.Same(draggedItem, documentDock.Tabs[1]);
        Assert.Same(draggedItem, documentDock.ActiveTab);
    }

    [AvaloniaFact]
    public void ConvertToDocumentDock_KeepsGridPosition() {
        var root = new DockingManagerVM();
        var existingItem = CreateItem("Existing");
        var draggedItem = CreateItem("Dragged");

        root.Layout.Add(existingItem, DockConfig.Default);
        var dockedControl = root.Layout.LayoutGrid.Children.OfType<DockedControl>().Single();
        var (row, column, rowSpan, columnSpan) = (
            Grid.GetRow(dockedControl),
            Grid.GetColumn(dockedControl),
            Grid.GetRowSpan(dockedControl),
            Grid.GetColumnSpan(dockedControl));

        root.Layout.ConvertToDocumentDock(dockedControl, [draggedItem]);

        var convertedGrid = root.Layout.LayoutGrid.Children
            .OfType<DocumentDock>()
            .Single(dock => dock.DataContext is DocumentDockVM);

        Assert.Equal(row, Grid.GetRow(convertedGrid));
        Assert.Equal(column, Grid.GetColumn(convertedGrid));
        Assert.Equal(rowSpan, Grid.GetRowSpan(convertedGrid));
        Assert.Equal(columnSpan, Grid.GetColumnSpan(convertedGrid));

        // The old layout docked control is gone
        Assert.DoesNotContain(dockedControl, root.Layout.LayoutGrid.Children);
    }

    [AvaloniaFact]
    public void DocumentDroppedOntoLayoutArea_CreatesDocumentDock() {
        var root = new DockingManagerVM();
        var item = CreateItem("Document");

        // Simulates a drop of a document mode item onto the layout area
        root.Layout.Add(item, new DockConfig { DockMode = DockMode.Document });

        var documentDock = root.Layout.LayoutGrid.Children
            .OfType<DocumentDock>()
            .Select(dock => dock.DataContext)
            .OfType<DocumentDockVM>()
            .Single();
        Assert.Same(item, documentDock.Tabs.Single());
    }

    [AvaloniaFact]
    public void SecondDocumentDroppedOntoLayoutArea_JoinsExistingDocumentDock() {
        var root = new DockingManagerVM();
        var itemA = CreateItem("DocumentA");
        var itemB = CreateItem("DocumentB");

        root.Layout.Add(itemA, new DockConfig { DockMode = DockMode.Document });
        root.Layout.Add(itemB, new DockConfig { DockMode = DockMode.Document });

        var documentDock = root.Layout.LayoutGrid.Children
            .OfType<DocumentDock>()
            .Select(dock => dock.DataContext)
            .OfType<DocumentDockVM>()
            .Single();
        Assert.Equal(2, documentDock.Tabs.Count);
        Assert.Same(itemA, documentDock.Tabs[0]);
        Assert.Same(itemB, documentDock.Tabs[1]);
    }
}
