using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.WPF.ViewModels.Docking;
using Dock.Avalonia.Controls;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
namespace CreationEditor.WPF.Services.Docking;

public class DefaultDockFactory : Factory {
    private readonly object _context;

    public IDock Main { get; set; } = null!;

    public DefaultDockFactory(object context) {
        _context = context;
    }

    public override IRootDock CreateLayout() {
        var root = CreateRootDock();

        Main = new ProportionalDock {
            Id = "MainLayout",
            Title = "MainLayout",
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>()
        };

        root.Id = "Root";
        root.Title = "Root";
        root.ActiveDockable = Main;
        root.DefaultDockable = Main;
        root.VisibleDockables = CreateList<IDockable>(Main);

        return root;
    }

    public override void InitLayout(IDockable layout) {
        ContextLocator = new Dictionary<string, Func<object>> {
            [nameof(IRootDock)] = () => _context,
            [nameof(IProportionalDock)] = () => _context,
            [nameof(IDocumentDock)] = () => _context,
            [nameof(IToolDock)] = () => _context,
            [nameof(IProportionalDockSplitter)] = () => _context,
            [nameof(IDockWindow)] = () => _context,
            [nameof(IDocument)] = () => _context,
            [nameof(ITool)] = () => _context,
            [nameof(ToolDockVM)] = () => _context,
            [nameof(DocumentDockVM)] = () => _context,
            ["MainLayout"] = () => _context,
            ["Main"] = () => _context,
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow>> {
            [nameof(IDockWindow)] = () => {
                var hostWindow = new HostWindow {
                    [!Window.TitleProperty] = new Binding("ActiveDockable.Title")
                };
                return hostWindow;
            }
        };

        DockableLocator = new Dictionary<string, Func<IDockable?>>();

        base.InitLayout(layout);

        SetActiveDockable(Main);
        SetFocusedDockable(Main, Main.VisibleDockables?.FirstOrDefault());
    }
}
