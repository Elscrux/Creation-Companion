using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Notification;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Setting;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Plugin;
using DynamicData.Binding;
using FluentAvalonia.UI.Windowing;
using Noggog;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels;

public sealed partial class MainVM : ViewModel {
    private readonly Func<ISettingsVM> _settingsVMFactory;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IDockFactory _dockFactory;
    private readonly MainWindow _mainWindow;
    private readonly IModSaveService _modSaveService;
    private readonly IFileSystem _fileSystem;
    private const string BaseWindowTitle = "Creation Companion";

    public INotificationVM NotificationVM { get; }
    public IBusyService BusyService { get; }
    public ModSelectionVM ModSelectionVM { get; }
    public IDataSourceSelectionVM DataSourceSelectionVM { get; }

    public IObservable<string> WindowTitleObs { get; }

    public IDockingManagerService DockingManagerService { get; }
    public IPluginService? PluginService { get; }
    public IObservableCollection<IMenuPluginDefinition> MenuBarPlugins { get; } = new ObservableCollectionExtended<IMenuPluginDefinition>();
    public IObservableCollection<string> Actions { get; } = new ObservableCollectionExtended<string> {
        "Save",
        "Save As",
        "Save All",
        "Close",
        "Close All",
        "Close All But This",
        "Exit",
        "Undo",
        "Redo",
        "Cut",
        "Copy",
        "Paste",
        "Delete",
        "Select All",
        "Find",
        "Find Next",
        "Find Previous",
        "Open Record",
        "Open Record in New Tab",
        "Open Record in New Window",
        "Create Record",
        "Create Record in New Tab",
        "Create Record in New Window",
    };

    public MainVM(
        Func<ISettingsVM> settingsVMFactory,
        INotificationVM notificationVM,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM,
        IDataSourceSelectionVM dataSourceSelectionVM,
        IDockingManagerService dockingManagerService,
        IDockFactory dockFactory,
        MainWindow mainWindow,
        IPluginService? pluginService,
        IModSaveService modSaveService,
        IApplicationSplashScreen splashScreen,
        IFileSystem fileSystem) {
        _settingsVMFactory = settingsVMFactory;
        _editorEnvironment = editorEnvironment;
        _dockFactory = dockFactory;
        _mainWindow = mainWindow;
        _modSaveService = modSaveService;
        _fileSystem = fileSystem;
        NotificationVM = notificationVM;
        BusyService = busyService;
        ModSelectionVM = modSelectionVM;
        DataSourceSelectionVM = dataSourceSelectionVM;
        DockingManagerService = dockingManagerService;
        PluginService = pluginService;
        _mainWindow.SplashScreen = splashScreen;

        PluginService?.PluginsRegistered
            .Subscribe(newPlugins => MenuBarPlugins.AddRange(newPlugins.OfType<IMenuPluginDefinition>()))
            .DisposeWith(this);

        WindowTitleObs = _editorEnvironment.ActiveModChanged
            .Select(activeMod => $"{BaseWindowTitle} - {activeMod}")
            .StartWith(BaseWindowTitle);
    }

    [ReactiveCommand]
    private void OpenPlugin(IMenuPluginDefinition plugin) {
        DockingManagerService.AddControl(plugin.GetControl(),
            new DockConfig {
                DockInfo = new DockInfo {
                    Header = plugin.Name
                },
                DockMode = plugin.DockMode,
                Dock = plugin.Dock,
            });
    }

    [ReactiveCommand]
    private void OpenGameFolder() {
        var gameFolder = _fileSystem.Directory.GetParent(_editorEnvironment.GameEnvironment.DataFolderPath);
        if (gameFolder is not null) {
            Process.Start(new ProcessStartInfo {
                FileName = gameFolder.FullName,
                UseShellExecute = true,
                Verb = "open",
            });
        }
    }

    [ReactiveCommand]
    private void OpenDataFolder() {
        Process.Start(new ProcessStartInfo {
            FileName = _editorEnvironment.GameEnvironment.DataFolderPath,
            UseShellExecute = true,
            Verb = "open",
        });
    }

    [ReactiveCommand]
    private void OpenSettings() {
        var settingsVM = _settingsVMFactory();
        var settingsWindow = new SettingsWindow(settingsVM);
        settingsWindow.ShowDialog(_mainWindow);
    }

    [ReactiveCommand]
    private void Save() {
        Parallel.ForEach(_editorEnvironment.MutableMods, _modSaveService.SaveMod);
    }

    [ReactiveCommand]
    private Task OpenDockElement(DockElement element) {
        return _dockFactory.Open(element);
    }

    public bool IsEnvironmentUninitialized() {
        return _editorEnvironment.ActiveMod.ModKey.IsNull;
    }
}
