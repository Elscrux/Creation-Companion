using System.Diagnostics;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Notification;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Mod;
using CreationEditor.Avalonia.Views.Setting;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod.Save;
using CreationEditor.Services.Plugin;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels;

public sealed class MainVM : ViewModel {
    private readonly ModSelectionVM _modSelectionVM;
    private const string BaseWindowTitle = "Creation Companion";

    public INotificationVM NotificationVM { get; }
    public IBusyService BusyService { get; }

    public IObservable<string> WindowTitleObs { get; }

    public IDockingManagerService DockingManagerService { get; }
    public IPluginService? PluginService { get; }
    public IList<IMenuPluginDefinition>? MenuBarPlugins { get; }

    public ReactiveCommand<Unit, Unit> OpenSelectMods { get; }
    public ReactiveCommand<IMenuPluginDefinition, Unit> OpenPlugin { get; }
    public ReactiveCommand<Unit, Unit> OpenSettings { get; }

    public ReactiveCommand<Unit, Unit> OpenGameFolder { get; }
    public ReactiveCommand<Unit, Unit> OpenDataFolder { get; }

    public ReactiveCommand<Unit, Unit> Save { get; }

    public ReactiveCommand<DockElement, Unit> OpenDockElement { get; }

    public MainVM(
        IComponentContext componentContext,
        INotificationVM notificationVM,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM,
        IDockingManagerService dockingManagerService,
        IDockFactory dockFactory,
        MainWindow mainWindow,
        IPluginService? pluginService,
        IModSaveService modSaveService,
        IFileSystem fileSystem) {
        _modSelectionVM = modSelectionVM;
        NotificationVM = notificationVM;
        BusyService = busyService;
        DockingManagerService = dockingManagerService;
        PluginService = pluginService;
        MenuBarPlugins = PluginService?.Plugins.OfType<IMenuPluginDefinition>().ToList();

        OpenSelectMods = ReactiveCommand.Create(ShowModSelection);

        OpenPlugin = ReactiveCommand.Create<IMenuPluginDefinition>(plugin => {
            DockingManagerService.AddControl(
                plugin.GetControl(),
                new DockConfig {
                    DockInfo = new DockInfo { Header = plugin.Name, },
                    DockMode = DockMode.Side,
                    Dock = Dock.Top,
                });
        });

        OpenGameFolder = ReactiveCommand.Create(() => {
            var gameFolder = fileSystem.Directory.GetParent(editorEnvironment.GameEnvironment.DataFolderPath);
            if (gameFolder != null) {
                Process.Start(new ProcessStartInfo {
                    FileName = gameFolder.FullName,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        });

        OpenDataFolder = ReactiveCommand.Create(() => {
            Process.Start(new ProcessStartInfo {
                FileName = editorEnvironment.GameEnvironment.DataFolderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        });

        OpenSettings = ReactiveCommand.Create(() => {
            var settingsWindow = new SettingsWindow(componentContext.Resolve<ISettingsVM>());
            settingsWindow.ShowDialog(mainWindow);
        });

        Save = ReactiveCommand.Create(() => modSaveService.SaveMod(editorEnvironment.LinkCache, editorEnvironment.ActiveMod));

        OpenDockElement = ReactiveCommand.Create<DockElement>(element => dockFactory.Open(element));

        WindowTitleObs = editorEnvironment.ActiveModChanged
            .Select(activeMod => $"{BaseWindowTitle} - {activeMod}")
            .StartWith(BaseWindowTitle);
    }

    public void ShowModSelection() {
        ModSelectionView.ShowAsContentDialog(_modSelectionVM);
    }
}
