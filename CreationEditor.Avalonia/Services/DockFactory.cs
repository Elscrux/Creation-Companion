using System.IO.Abstractions;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Viewport;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Logging;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Asset.Browser;
using CreationEditor.Avalonia.Views.Logging;
using CreationEditor.Avalonia.Views.Record;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Avalonia.Services;

public sealed class DockFactory : IDockFactory {
    private bool _viewportCreated;

    private readonly Func<ILogVM> _logVMFactory;
    private readonly Func<IRecordBrowserVM> _recordBrowserVMFactory;
    private readonly Func<string, IAssetBrowserVM> _assetBrowserVMFactory;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IFileSystem _fileSystem;
    private readonly IViewportFactory _viewportFactory;
    private readonly IDockingManagerService _dockingManagerService;
    private readonly ICellBrowserFactory _cellBrowserFactory;

    public DockFactory(
        Func<ILogVM> logVMFactory,
        Func<IRecordBrowserVM> recordBrowserVMFactory,
        Func<string, IAssetBrowserVM> assetBrowserVMFactory,
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem,
        IViewportFactory viewportFactory,
        IDockingManagerService dockingManagerService,
        ICellBrowserFactory cellBrowserFactory) {
        _logVMFactory = logVMFactory;
        _recordBrowserVMFactory = recordBrowserVMFactory;
        _assetBrowserVMFactory = assetBrowserVMFactory;
        _dataDirectoryProvider = dataDirectoryProvider;
        _fileSystem = fileSystem;
        _viewportFactory = viewportFactory;
        _dockingManagerService = dockingManagerService;
        _cellBrowserFactory = cellBrowserFactory;
    }

    public void Open(DockElement dockElement, DockMode? dockMode = null, Dock? dock = null, object? parameter = null) {
        Control control;
        DockConfig dockConfig;

        switch (dockElement) {
            case DockElement.Log:
                var logVM = _logVMFactory();
                control = new LogView(logVM);
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
                var recordBrowserVM = _recordBrowserVMFactory();
                control = new RecordBrowser(recordBrowserVM);
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
            case DockElement.AssetBrowser:
                var folder = parameter as string ?? _dataDirectoryProvider.Path;
                var assetBrowserVM = _assetBrowserVMFactory(folder);
                control = new AssetBrowser(assetBrowserVM);
                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = $"Asset Browser{(parameter is string str ? $" - {_fileSystem.Path.GetFileName(str)}" : string.Empty)}",
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

        if (dockMode is not null) dockConfig = dockConfig with { DockMode = dockMode.Value };
        if (dock is not null) dockConfig = dockConfig with { Dock = dock.Value };

        _dockingManagerService.AddControl(control, dockConfig);
    }
}
