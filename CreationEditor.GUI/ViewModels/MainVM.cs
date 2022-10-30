using System.Reactive;
using System.Windows;
using Autofac;
using CreationEditor.Environment;
using CreationEditor.GUI.Services;
using CreationEditor.GUI.Services.Docking;
using CreationEditor.GUI.ViewModels.Docking;
using CreationEditor.GUI.ViewModels.Mod;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.GUI.Views.Mod;
using CreationEditor.GUI.Views.Record;
using Elscrux.Notification;
using Elscrux.WPF.ViewModels;
using Elscrux.WPF.Views.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using AvalonDock.Layout;
using AvalonDock.Themes;
using ControlzEx.Theming;
using Theme = AvalonDock.Themes.Theme;
namespace CreationEditor.GUI.ViewModels;

public class MainVM : NotifiedVM {
    public const string BaseWindowTitle = "Creation Editor";

    private readonly ILifetimeScope _lifetimeScope;
    private readonly INotifier _notifier;
    private readonly IEditorEnvironment _editorEnvironment;

    public IBusyService BusyService { get; }
    
    [Reactive] public string WindowTitle { get; set; } = BaseWindowTitle;

    private readonly ModSelectionVM _modSelectionVM;
    public IDockingManagerService DockingManagerService { get; }

    public ReactiveCommand<Window, Unit> OpenSelectMods { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    
    public ReactiveCommand<Unit, Unit> OpenLog { get; }
    public ReactiveCommand<Unit, Unit> OpenRecordBrowser { get; }
    public Theme Theme { get; set; }

    public MainVM(
        ILifetimeScope lifetimeScope,
        INotifier notifier,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM,
        IDockingManagerService dockingManagerService) {
        _lifetimeScope = lifetimeScope;
        _notifier = notifier;
        BusyService = busyService;
        _editorEnvironment = editorEnvironment;
        _modSelectionVM = modSelectionVM;
        DockingManagerService = dockingManagerService;
        
        OpenSelectMods = ReactiveCommand.Create((Window baseWindow) => {
            var modSelectionWindow = new ModSelectionWindow(_modSelectionVM) { Owner = baseWindow };
            modSelectionWindow.Show();
        });

        Save = ReactiveCommand.Create(() => {});
        
        OpenLog = ReactiveCommand.Create(() => {
            DockingManagerService.AddAnchoredControl(
                new LogView(_lifetimeScope.Resolve<ILogVM>()),
                "Log",
                new DockingStatus { AnchorSide = AnchorSide.Bottom, Height = 200 });
        });
        
        OpenRecordBrowser = ReactiveCommand.Create(() => {
            DockingManagerService.AddAnchoredControl(
                new RecordBrowser(_lifetimeScope.Resolve<IRecordBrowserVM>()),
                "Record Browser",
                new DockingStatus { Width = 600 });
        });
        
        _editorEnvironment.ActiveModChanged += EditorOnActiveModChanged;

        _notifier.Subscribe(this);
        
        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
        ThemeManager.Current.SyncTheme();
        Theme = ThemeManager.Current.DetectTheme()?.BaseColorScheme switch {
            "Dark" => new Vs2013DarkTheme(),
            _ => new Vs2013LightTheme(),
        };
    }
    
    private void EditorOnActiveModChanged(object? sender, EventArgs e) {
        WindowTitle = _modSelectionVM.ActiveMod == null ? BaseWindowTitle : $"{BaseWindowTitle} - {_modSelectionVM.ActiveMod.Value}";
    }
}