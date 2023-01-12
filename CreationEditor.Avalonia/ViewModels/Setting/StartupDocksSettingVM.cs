using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Models.Settings;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Startup;
using CreationEditor.Services.Settings;
using Newtonsoft.Json;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public sealed record StartupDocksSetting(List<StartupDock> Docks);

public sealed class StartupDocksSettingVM : ViewModel, ISetting, ILifecycleTask {
    public static readonly IEnumerable<DockElement> StartupDockTypes = Enum.GetValues<DockElement>();
    public static readonly IEnumerable<DockMode> DockModeTypes = Enum.GetValues<DockMode>();
    public static readonly IEnumerable<Dock> DockTypes = Enum.GetValues<Dock>();

    private readonly IDockFactory _dockFactory;

    public string Name => "Startup Docks";
    public Type? Parent => null;
    public List<ISetting> Children { get; } = new();

    public ReactiveCommand<Unit, Unit> AddStartupDock { get; }
    public ReactiveCommand<IList, Unit> RemoveStartupDock { get; }
    
    [JsonProperty]
    public ObservableCollection<StartupDock> Docks { get; } = new();

    public StartupDocksSettingVM(
        IDockFactory dockFactory,
        ISettingImporter<StartupDocksSetting> settingImporter) {
        _dockFactory = dockFactory;

        AddStartupDock = ReactiveCommand.Create(() => Docks.Add(new StartupDock()));
        
        RemoveStartupDock = ReactiveCommand.Create<IList>(removeDocks => {
            foreach (var removeDock in removeDocks.OfType<StartupDock>().ToList()) {
                Docks.Remove(removeDock);
            }
        });

        var startupDocksSettingVM = settingImporter.Import(this);
        if (startupDocksSettingVM != null) {
            Docks = new ObservableCollection<StartupDock>(startupDocksSettingVM.Docks);
        }
    }

    private void Start(StartupDock startupDock) {
        _dockFactory.Open(startupDock.DockElement, startupDock.DockMode, startupDock.Dock);
    }
    
    public void OnStartup() {
        foreach (var startupDock in Docks) {
            Start(startupDock);
        }
    }
    
    public void OnExit() {}
}
