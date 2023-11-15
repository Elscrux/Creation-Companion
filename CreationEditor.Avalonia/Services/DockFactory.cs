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
using CreationEditor.Avalonia.ViewModels.Scripting;
using CreationEditor.Avalonia.Views.Asset.Browser;
using CreationEditor.Avalonia.Views.Logging;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Avalonia.Views.Scripting;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Avalonia.Services;

public sealed class DockFactory(
        Func<ILogVM> logVMFactory,
        Func<IRecordBrowserVM> recordBrowserVMFactory,
        Func<string, IAssetBrowserVM> assetBrowserVMFactory,
        Func<IScriptVM> scriptVMFactory,
        IDataDirectoryProvider dataDirectoryProvider,
        IFileSystem fileSystem,
        IViewportFactory viewportFactory,
        IDockingManagerService dockingManagerService,
        ICellBrowserFactory cellBrowserFactory)
    : IDockFactory {
    private bool _viewportCreated;

    public void Open(DockElement dockElement, DockMode? dockMode = null, Dock? dock = null, object? parameter = null) {
        Control control;
        DockConfig dockConfig;

        switch (dockElement) {
            case DockElement.Log:
                var logVM = logVMFactory();
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
                var recordBrowserVM = recordBrowserVMFactory();
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
                control = cellBrowserFactory.GetBrowser();
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
                var folder = parameter as string ?? dataDirectoryProvider.Path;
                var assetBrowserVM = assetBrowserVMFactory(folder);
                control = new AssetBrowser(assetBrowserVM);
                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = $"Asset Browser{(parameter is string str ? $" - {fileSystem.Path.GetFileName(str)}" : string.Empty)}",
                        Size = 400,
                    },
                    Dock = Dock.Left,
                    DockMode = DockMode.Side,
                };
                break;
            case DockElement.ScriptEditor:
                var vm = scriptVMFactory();
                control = new ScriptEditor(vm);
                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Script Editor",
                        Size = 300,
                    },
                    Dock = Dock.Left,
                    DockMode = DockMode.Document,
                };
                break;
            case DockElement.Viewport:
                if (_viewportCreated && !viewportFactory.IsMultiInstanceCapable) return;

                control = viewportFactory.CreateViewport();
                _viewportCreated = true;

                dockConfig = new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Viewport",
                        CanClose = viewportFactory.IsMultiInstanceCapable
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

        dockingManagerService.AddControl(control, dockConfig);
    }
}
