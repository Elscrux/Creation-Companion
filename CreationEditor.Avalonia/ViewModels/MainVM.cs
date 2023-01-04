using System.Reactive;
using Autofac;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Avalonia.Services.Docking;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Mod;
using CreationEditor.Avalonia.Views.Record;
using CreationEditor.Environment;
using CreationEditor.Notification;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels;

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
        
        // todo add default docking manager views (record browser on the left in side panel, log at the bottom in side panel)
        
        OpenSelectMods = ReactiveCommand.Create<Window>(window => {
            var modSelectionWindow = new ModSelectionWindow(modSelectionVM);
            modSelectionWindow.ShowDialog(window);
        });

        Save = ReactiveCommand.Create(() => {});
        
        OpenLog = ReactiveCommand.Create(() => {
            // DockingManagerService.AddControl(
            //     new LogView(componentContext.Resolve<ILogVM>()),
            //     new DockConfig {
            //         DockInfo = new DockInfo {
            //             Header = "Log", 
            //         },
            //         Dock = Dock.Bottom,
            //         DockMode = DockMode.Side,
            //     });
        });
        
        OpenRecordBrowser = ReactiveCommand.Create(() => {
            DockingManagerService.AddControl(
                new RecordBrowser(componentContext.Resolve<IRecordBrowserVM>()),
                new DockConfig {
                    DockInfo = new DockInfo {
                        Header = "Record Browser", 
                    },
                    Dock = Dock.Left,
                    DockMode = DockMode.Layout,
                    Size = new GridLength(3, GridUnitType.Star),
                });
        });
        
        editorEnvironment.ActiveModChanged
            .Subscribe(activeMod => {
                WindowTitle = $"{BaseWindowTitle} - {activeMod}";
            });

        notifier.Subscribe(this);
    }
}