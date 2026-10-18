using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Avalonia.ViewModels.Integrations;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Notification;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Integrations;
using CreationEditor.Avalonia.Views.Setting;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Windowing;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels;

public sealed partial class MainVM : ViewModel {
    private readonly Func<ISettingsVM> _settingsVMFactory;
    private readonly Func<IntegrationsVM> _integrationsVMFactory;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IDockFactory _dockFactory;
    private readonly IDockableRegistry _dockableRegistry;
    private readonly IMainWindowProvider _mainWindowProvider;
    private readonly IFAApplicationSplashScreen _splashScreen;
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
        "Go to Folder in Asset Browser",
        "Create Record",
        "Create Record in New Tab",
        "Create Record in New Window",
    };
    [Reactive] public partial IReadOnlyList<MenuItemVM> MenuItems { get; set; }

    public MainVM(
        Func<ISettingsVM> settingsVMFactory,
        Func<IntegrationsVM> integrationsVMFactory,
        INotificationVM notificationVM,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM,
        IDataSourceSelectionVM dataSourceSelectionVM,
        IDockingManagerService dockingManagerService,
        IDockFactory dockFactory,
        IDockableRegistry dockableRegistry,
        IMainWindowProvider mainWindowProvider,
        IPluginService? pluginService,
        IModSaveService modSaveService,
        IFAApplicationSplashScreen splashScreen,
        IFileSystem fileSystem) {
        _settingsVMFactory = settingsVMFactory;
        _integrationsVMFactory = integrationsVMFactory;
        _editorEnvironment = editorEnvironment;
        _dockFactory = dockFactory;
        _dockableRegistry = dockableRegistry;
        _mainWindowProvider = mainWindowProvider;
        _modSaveService = modSaveService;
        _fileSystem = fileSystem;
        _splashScreen = splashScreen;
        NotificationVM = notificationVM;
        BusyService = busyService;
        ModSelectionVM = modSelectionVM;
        DataSourceSelectionVM = dataSourceSelectionVM;
        DockingManagerService = dockingManagerService;
        PluginService = pluginService;
        mainWindowProvider.MainWindowChanged
            .Subscribe(OnMainWindowChanged)
            .DisposeWith(this);
        if (mainWindowProvider.Current is {} currentWindow) OnMainWindowChanged(currentWindow);

        PluginService?.PluginsRegistered
            .ObserveOnGui()
            .Subscribe(_ => MenuItems = BuildMenuItems())
            .DisposeWith(this);

        PluginService?.PluginsUnregistered
            .ObserveOnGui()
            .Subscribe(_ => MenuItems = BuildMenuItems())
            .DisposeWith(this);

        WindowTitleObs = _editorEnvironment.ActiveModChanged
            .Select(activeMod => $"{BaseWindowTitle} - {activeMod}")
            .StartWith(BaseWindowTitle);

        MenuItems = BuildMenuItems();
    }

    private void OnMainWindowChanged(MainWindow window) {
        // The splash screen content view is reused across UI restarts; detach it from the
        // previous window's visual tree before assigning it to the new one
        if (_splashScreen.SplashScreenContent is Control content
         && content.GetVisualParent() is ContentPresenter presenter) {
            presenter.Content = null;
        }

        window.SplashScreen = _splashScreen;
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
        if (_mainWindowProvider.Current is not {} mainWindow) return;

        var settingsVM = _settingsVMFactory();
        var settingsWindow = new SettingsWindow(settingsVM);
        settingsWindow.ShowDialog(mainWindow);
    }

    [ReactiveCommand]
    private void OpenIntegrations() {
        if (_mainWindowProvider.Current is not {} mainWindow) return;

        var integrationsVM = _integrationsVMFactory();
        var integrationsWindow = new IntegrationsWindow(integrationsVM);
        integrationsWindow.ShowDialog(mainWindow);
    }

    [ReactiveCommand]
    private void Save() {
        Parallel.ForEach(_editorEnvironment.MutableMods, _modSaveService.SaveMod);
    }

    [ReactiveCommand]
    private Task OpenDockable(IDockableDefinition dockable) => _dockFactory.Open(dockable);

    private IReadOnlyList<MenuItemVM> BuildMenuItems() {
        var root = new MenuNode(string.Empty);
        foreach (var definition in _dockableRegistry.Definitions) {
            // Categories are root based: the first segment becomes a top level menu item, deeper segments become nested submenus
            var node = root;
            foreach (var segment in definition.Category.Split('/')) {
                node = node.GetOrAddChild(segment);
            }
            node.Definitions.Add(definition);
        }

        return [.. root.Children.Select(BuildNode)];

        MenuItemVM BuildNode(MenuNode node) {
            var children = node.Definitions.Select(CreateItem)
                .Concat(node.Children.Select(BuildNode))
                .ToList();
            return new MenuItemVM(node.Header, children);
        }

        MenuItemVM CreateItem(IDockableDefinition definition) {
            return new MenuItemVM(
                definition.Header,
                [],
                definition.Icon,
                definition.HotKey,
                ReactiveCommand.Create(() => _dockFactory.OpenFireAndForget(definition)));
        }
    }

    private sealed class MenuNode(string header) {
        public string Header { get; } = header;
        public List<MenuNode> Children { get; } = [];
        public List<IDockableDefinition> Definitions { get; } = [];

        public MenuNode GetOrAddChild(string header) {
            var child = Children.Find(node => node.Header == header);
            if (child is not null) return child;

            child = new MenuNode(header);
            Children.Add(child);
            return child;
        }
    }

    public bool IsEnvironmentUninitialized() => _editorEnvironment.ActiveMod.ModKey.IsNull;
}
