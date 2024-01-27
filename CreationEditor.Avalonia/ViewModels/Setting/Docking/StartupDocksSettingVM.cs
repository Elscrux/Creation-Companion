using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Models.Settings.Docking;
using CreationEditor.Avalonia.Services;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Settings;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting.Docking;

public sealed class StartupDocksSettingVM : ViewModel, ISetting, ILifecycleTask {
    public static readonly IEnumerable<DockElement> StartupDockTypes = Enum.GetValues<DockElement>();
    public static readonly IEnumerable<DockMode> DockModeTypes = Enum.GetValues<DockMode>();
    public static readonly IEnumerable<Dock> DockTypes = Enum.GetValues<Dock>();

    private readonly IDockFactory _dockFactory;

    public string Name => "Startup Docks";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = [];

    public ReactiveCommand<Unit, Unit> AddStartupDock { get; }
    public ReactiveCommand<IList, Unit> RemoveStartupDock { get; }

    public StartupDocksSetting Settings { get; }
    public ISettingModel Model => Settings;

    public StartupDocksSettingVM(
        IDockFactory dockFactory,
        ISettingImporter<StartupDocksSetting> settingImporter) {
        _dockFactory = dockFactory;

        Settings = settingImporter.Import(this) ?? StartupDocksSetting.Default;

        AddStartupDock = ReactiveCommand.Create(() => Settings.Docks.Add(new StartupDock()));

        RemoveStartupDock = ReactiveCommand.Create<IList>(removeDocks => {
            foreach (var removeDock in removeDocks.OfType<StartupDock>().ToList()) {
                Settings.Docks.Remove(removeDock);
            }
        });
    }

    private void Start(StartupDock startupDock) {
        _dockFactory.OpenFireAndForget(startupDock.DockElement, startupDock.DockMode, startupDock.Dock);
    }

    public void PreStartup() {}

    public void PostStartupAsync(CancellationToken token) => Dispatcher.UIThread.Post(() => {
        foreach (var startupDock in Settings.Docks) {
            Start(startupDock);
        }
    });

    public void OnExit() {}

    public void Apply() {
        // Nothing to do on this runtime
    }
}
