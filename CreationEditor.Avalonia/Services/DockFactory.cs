using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Docking;
using FluentAvalonia.UI.Controls;
using Noggog;
using Serilog;
namespace CreationEditor.Avalonia.Services;

public sealed class DockFactory(
    IDockingManagerService dockingManagerService,
    ILogger logger)
    : IDockFactory {
    public void OpenFireAndForget(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null) {
        Task.Run(async () => {
                var dockResult = await CreateDockConfig(dockable, dockMode, dock, parameter)
                 ?? throw new InvalidOperationException($"Failed to open dock {dockable.Type}");

                Dispatcher.UIThread.Post(() => dockingManagerService.AddControl(dockResult.Control, dockResult.Config));
            })
            .FireAndForget(e => logger.Here().Warning(e, "Couldn't open dock {Identifier}: {Message}", dockable.Type, e));
    }

    public Task Open(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null) {
        return Task.Run(async () => await CreateDock(dockable, dockMode, dock, parameter));
    }

    public Control? TryGetDock(IDockableDefinition dockable) {
        return dockingManagerService.Root.IterateAllChildren()
            .OfType<IDockedItem>()
            .FirstOrDefault(item => item.DockableType == dockable.Type)
            ?.Control;
    }

    public Task<Control> GetOrOpenDock(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null) {
        var openDock = TryGetDock(dockable);
        if (openDock is null) return CreateDock(dockable, dockMode, dock, parameter);

        Dispatcher.UIThread.Post(() => dockingManagerService.Focus(openDock));
        return Task.FromResult(openDock);
    }

    private readonly Lock _dockCreationLock = new();

    private async Task<Control> CreateDock(IDockableDefinition dockable, DockMode? dockMode = null, Dock? dock = null, object? parameter = null) {
        var dockResult = await CreateDockConfig(dockable, dockMode, dock, parameter)
         ?? throw new InvalidOperationException($"Failed to open dock {dockable.Type}");

        return await Dispatcher.UIThread.InvokeAsync(() => {
            // Check if the dock was already created by someone else while we were waiting for the UI thread
            lock (_dockCreationLock) {
                var openDock = TryGetDock(dockable);
                if (openDock is not null) {
                    dockingManagerService.Focus(openDock);
                    return openDock;
                }

                dockingManagerService.AddControl(dockResult.Control, dockResult.Config);
            }
            return dockResult.Control;
        });
    }

    private async Task<(Control Control, DockConfig Config)?> CreateDockConfig(IDockableDefinition dockable, DockMode? dockMode, Dock? dock, object? parameter) {
        // Controls must be created on the UI thread - make sure heavy work inside CreateControl is offloaded to a background thread
        var control = await Dispatcher.UIThread.InvokeAsync(() => dockable.CreateControl(parameter));
        if (control is null) return null;

        var config = new DockConfig {
            DockInfo = new DockInfo {
                Header = dockable.Header,
                IconSource = dockable.Icon switch {
                    null => null,
                    FASymbol symbol => new FASymbolIconSource { Symbol = symbol },
                    _ => new FAFontIconSource { Glyph = dockable.Icon.ToString() },
                },
                Size = dockable.Size,
                CanClose = dockable.CanClose,
                DockableType = dockable.Type,
            },
            DockMode = dockMode ?? dockable.DockMode,
            Dock = dock ?? dockable.Dock,
            GridSize = dockable.GridSize,
        };

        return (control, config);
    }
}
