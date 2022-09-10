using System.Reactive;
using System.Windows;
using Autofac;
using CreationEditor.Environment;
using CreationEditor.GUI.Services;
using CreationEditor.GUI.ViewModels.Mod;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.GUI.Views.Mod;
using CreationEditor.GUI.Views.Record;
using Elscrux.Notification;
using Elscrux.WPF.Extensions;
using Elscrux.WPF.ViewModels;
using Elscrux.WPF.Views.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.ViewModels;

public class MainVM : NotifiedVM {
    public const string BaseWindowTitle = "Creation Editor";

    private readonly ILifetimeScope _lifetimeScope;
    private readonly INotifier _notifier;
    private readonly IEditorEnvironment _editorEnvironment;

    public IBusyService BusyService { get; }
    
    [Reactive] public string WindowTitle { get; set; } = BaseWindowTitle;

    private readonly ModSelectionVM _modSelectionVM;

    public ReactiveCommand<Window, Unit> OpenSelectMods { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<DockingManager, LogView> OpenLog { get; }
    public ReactiveCommand<DockingManager, RecordBrowser> OpenRecordBrowser { get; }

    public MainVM(
        ILifetimeScope lifetimeScope,
        INotifier notifier,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM) {
        _lifetimeScope = lifetimeScope;
        _notifier = notifier;
        BusyService = busyService;
        _editorEnvironment = editorEnvironment;
        _modSelectionVM = modSelectionVM;
        
        OpenSelectMods = ReactiveCommand.Create((Window baseWindow) => {
            var modSelectionWindow = new ModSelectionWindow(_modSelectionVM) { Owner = baseWindow };
            modSelectionWindow.Show();
        });

        Save = ReactiveCommand.Create(() => {});
        
        OpenLog = ReactiveCommand.Create((DockingManager dockingManager) => {
            using var scope = _lifetimeScope.BeginLifetimeScope();
            var recordBrowser = new LogView(scope.Resolve<ILogVM>());
            return dockingManager.AddControl(recordBrowser, "Log", DockSide.Bottom, DockState.Dock);
        });
        
        OpenRecordBrowser = ReactiveCommand.Create((DockingManager dockingManager) => {
            using var scope = _lifetimeScope.BeginLifetimeScope();
            var recordBrowser = new RecordBrowser(scope.Resolve<IRecordBrowserVM>());
            return dockingManager.AddControl(recordBrowser, "Record Browser", DockSide.Left, DockState.Dock, 500);
        });
        
        _editorEnvironment.ActiveModChanged += EditorOnActiveModChanged;

        _notifier.Subscribe(this);
    }
    
    private void EditorOnActiveModChanged(object? sender, EventArgs e) {
        WindowTitle = _modSelectionVM.ActiveMod == null ? BaseWindowTitle : $"{BaseWindowTitle} - {_modSelectionVM.ActiveMod.Value}";
    }
}