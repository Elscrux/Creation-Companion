using System.Reactive;
using System.Reactive.Linq;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Services;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Notification;
using CreationEditor.Avalonia.ViewModels.Setting;
using CreationEditor.Avalonia.Views.Mod;
using CreationEditor.Avalonia.Views.Setting;
using CreationEditor.Services.Environment;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels;

public sealed class MainVM : ViewModel {
    private readonly ModSelectionVM _modSelectionVM;
    private const string BaseWindowTitle = "Creation Editor";

    public INotificationVM NotificationVM { get; }
    public IBusyService BusyService { get; }

    public IObservable<string> WindowTitleObs { get; }

    public IDockingManagerService DockingManagerService { get; }

    public ReactiveCommand<Unit, Unit> OpenSelectMods { get; }
    public ReactiveCommand<Window, Unit> OpenSettings { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }

    public ReactiveCommand<DockElement, Unit> OpenDockElement { get; }

    public MainVM(
        IComponentContext componentContext,
        INotificationVM notificationVM,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM,
        IDockingManagerService dockingManagerService,
        IDockFactory dockFactory) {
        NotificationVM = notificationVM;
        BusyService = busyService;
        DockingManagerService = dockingManagerService;

        OpenSelectMods = ReactiveCommand.Create(ShowModSelection);

        OpenSettings = ReactiveCommand.Create<Window>(window => {
            var settingsWindow = new SettingsWindow(componentContext.Resolve<ISettingsVM>());
            settingsWindow.ShowDialog(window);
        });

        Save = ReactiveCommand.Create(() => {});

        OpenDockElement = ReactiveCommand.Create<DockElement>(element => dockFactory.Open(element));

        WindowTitleObs = editorEnvironment.ActiveModChanged
            .Select(activeMod => $"{BaseWindowTitle} - {activeMod}")
            .StartWith(BaseWindowTitle);
    }

    public void ShowModSelection() {
        ModSelectionView.ShowAsContentDialog(_modSelectionVM);
    }
}
