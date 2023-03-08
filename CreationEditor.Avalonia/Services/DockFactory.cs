using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.ViewModels.Logging;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Logging;
using CreationEditor.Avalonia.Views.Record;
namespace CreationEditor.Avalonia.Services;

public sealed class DockFactory : IDockFactory {
    private bool _viewportCreated;

    private readonly IComponentContext _componentContext;
    private readonly IViewportFactory _viewportFactory;
    private readonly IDockingManagerService _dockingManagerService;
    private readonly ICellBrowserFactory _cellBrowserFactory;

    public DockFactory(
        IComponentContext componentContext,
        IViewportFactory viewportFactory,
        IDockingManagerService dockingManagerService,
        ICellBrowserFactory cellBrowserFactory) {
        _componentContext = componentContext;
        _viewportFactory = viewportFactory;
        _dockingManagerService = dockingManagerService;
        _cellBrowserFactory = cellBrowserFactory;
    }

    public void Open(DockElement dockElement, DockMode? dockMode = null, Dock? dock = null) {
        Control control;
        DockConfig dockConfig;

        switch (dockElement) {
            case DockElement.Log:
                control = new LogView(_componentContext.Resolve<ILogVM>());
                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Log",
                        Size = 150,
                    },
                    Dock = Dock.Bottom,
                    DockMode = DockMode.Side,
                };
                break;
            case DockElement.RecordBrowser:
                control = new RecordBrowser(_componentContext.Resolve<IRecordBrowserVM>());
                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Record Browser",
                        Size = 500,
                    },
                    Dock = Dock.Left,
                    DockMode = DockMode.Side,
                    GridSize = new GridLength(3, GridUnitType.Star),
                };
                break;
            case DockElement.CellBrowser:
                control = _cellBrowserFactory.GetBrowser();
                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Cell Browser",
                        Size = 400,
                    },
                    Dock = Dock.Left,
                    DockMode = DockMode.Side,
                };
                break;
            case DockElement.Viewport:
                if (_viewportCreated && !_viewportFactory.IsMultiInstanceCapable) return;

                control = _viewportFactory.CreateViewport();
                _viewportCreated = true;

                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Viewport",
                        CanClose = _viewportFactory.IsMultiInstanceCapable
                    },
                    Dock = Dock.Right,
                    DockMode = DockMode.Layout,
                    GridSize = new GridLength(3, GridUnitType.Star),
                };
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dockElement), dockElement, null);
        }

        if (dockMode != null) dockConfig = dockConfig with { DockMode = dockMode.Value };
        if (dock != null) dockConfig = dockConfig with { Dock = dock.Value };

        _dockingManagerService.AddControl(control, dockConfig);
    }
}
