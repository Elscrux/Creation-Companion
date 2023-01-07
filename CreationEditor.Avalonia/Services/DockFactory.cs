using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.ViewModels.Logging;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Logging;
using CreationEditor.Avalonia.Views.Record;
namespace CreationEditor.Avalonia.Services;

public sealed class DockFactory : IDockFactory {
    private readonly IComponentContext _componentContext;
    private readonly IDockingManagerService _dockingManagerService;
    
    public DockFactory(
        IComponentContext componentContext,
        IDockingManagerService dockingManagerService) {
        _componentContext = componentContext;
        _dockingManagerService = dockingManagerService;
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
                    },
                    Dock = Dock.Left,
                    DockMode = DockMode.Layout,
                    Size = new GridLength(3, GridUnitType.Star),
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
