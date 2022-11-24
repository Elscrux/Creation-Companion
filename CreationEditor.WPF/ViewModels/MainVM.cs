using System.Reactive;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Environment;
using CreationEditor.Notification;
using CreationEditor.WPF.Services;
using CreationEditor.WPF.Services.Docking;
using CreationEditor.WPF.ViewModels.Mod;
using CreationEditor.WPF.ViewModels.Record;
using CreationEditor.WPF.Views.Mod;
using CreationEditor.WPF.Views.Record;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.ViewModels;

public class MainVM : NotifiedVM {
    private const string BaseWindowTitle = "Creation Editor";

    public IBusyService BusyService { get; }
    
    [Reactive] public string WindowTitle { get; set; } = BaseWindowTitle;

    public IDockingManagerService DockingManagerService { get; }

    public ReactiveCommand<Window, Unit> OpenSelectMods { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    
    public ReactiveCommand<Unit, Unit> OpenLog { get; }
    public ReactiveCommand<Unit, Unit> OpenRecordBrowser { get; }

    public MainVM(
        IComponentContext componentContext,
        INotifier notifier,
        IBusyService busyService,
        IEditorEnvironment editorEnvironment,
        ModSelectionVM modSelectionVM,
        IDockingManagerService dockingManagerService) {
        BusyService = busyService;
        DockingManagerService = dockingManagerService;
        
        OpenSelectMods = ReactiveCommand.Create<Window>(window => {
            var modSelectionWindow = new ModSelectionWindow(modSelectionVM);
            modSelectionWindow.ShowDialog(window);
        });

        Save = ReactiveCommand.Create(() => {});
        
        OpenLog = ReactiveCommand.Create(() => {
            // DockingManagerService.AddControl(
            //     new LogView(componentContext.Resolve<ILogVM>()),
            //     "Log",
            //     DockPosition.Bottom);
            //     // new DockingStatus { AnchorSide = AnchorSide.Bottom, Height = 200 });
        });
        
        OpenRecordBrowser = ReactiveCommand.Create(() => {
            DockingManagerService.AddControl(
                new RecordBrowser(componentContext.Resolve<IRecordBrowserVM>()),
                "Record Browser",
                Avalonia.Controls.Dock.Left);
            // new DockingStatus { Width = 600 });
        });
        
        editorEnvironment.ActiveModChanged
            .Subscribe(activeMod => {
                WindowTitle = $"{BaseWindowTitle} - {activeMod}";
            });

        notifier.Subscribe(this);

        // ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
        // ThemeManager.Current.SyncTheme();
        // Theme = ThemeManager.Current.DetectTheme()?.BaseColorScheme switch {
        //     "Dark" => new Vs2013DarkTheme(),
        //     _ => new Vs2013LightTheme(),
        // };
    }
}