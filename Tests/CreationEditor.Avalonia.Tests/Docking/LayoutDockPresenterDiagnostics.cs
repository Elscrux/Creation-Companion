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

public sealed class LayoutDockPresenterDiagnostics {
    private static IDockedItem CreateItem(string header) => new DockedItemVM(new Grid(), new DockInfo { Header = header });

    [AvaloniaFact]
    public void PresenterTakesHostedControlAfterLayout() {
        var root = new DockingManagerVM();
        var window = new Window {
            Width = 1000,
            Height = 800,
            Content = new DockingManager { DataContext = root },
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var item = CreateItem("A");
        root.Layout.Add(item, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        // The DockedControl's content presenter must have taken the hosted control as child
        var parent = item.Control.GetVisualParent();
        Assert.IsType<ContentPresenter>(parent);
    }

    [AvaloniaFact]
    public void HostedControlParentAfterRemove() {
        var root = new DockingManagerVM();
        var window = new Window {
            Width = 1000,
            Height = 800,
            Content = new DockingManager { DataContext = root },
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var item = CreateItem("A");
        root.Layout.Add(item, DockConfig.Default);
        Dispatcher.UIThread.RunJobs();

        Assert.NotNull(item.Control.GetVisualParent());

        root.Layout.Remove(item);
        Dispatcher.UIThread.RunJobs();

        // Is the hosted control released by the old presenter on remove, or still held?
        var parentAfterRemove = item.Control.GetVisualParent();
        Assert.Null(parentAfterRemove);
    }
}
