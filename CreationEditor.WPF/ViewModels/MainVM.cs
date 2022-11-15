using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Autofac;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Environment;
using CreationEditor.WPF.Models;
using CreationEditor.WPF.Services;
using CreationEditor.WPF.Services.Docking;
using CreationEditor.WPF.ViewModels.Mod;
using CreationEditor.WPF.Views.Mod;
using DynamicData.Binding;
using Elscrux.Notification;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.ViewModels;

public class NotifiedVM : ReactiveObject, INotificationReceiver {
    public ObservableCollection<NotificationItem> LoadingItems { get; } = new();

    private readonly ObservableAsPropertyHelper<bool> _anyLoading;
    public bool AnyLoading => _anyLoading.Value;

    private readonly ObservableAsPropertyHelper<NotificationItem?> _latestNotification;
    public NotificationItem? LatestNotification => _latestNotification.Value;

    public NotifiedVM() {
        var observableLoadingItems = LoadingItems.ToObservableChangeSet();
        
        _anyLoading = observableLoadingItems
            .Select(x => LoadingItems.Count > 0)
            .ToProperty(this, x => x.AnyLoading);
        
        _latestNotification = observableLoadingItems
            .Select(x => LoadingItems.LastOrDefault())
            .ToProperty(this, x => x.LatestNotification);
    }
    
    public void ReceiveNotify(Guid id, string message) {
        var item = LoadingItems.FirstOrDefault(item => item.ID == id);
        if (item != null) {
            item.LoadText = message;
        } else {
            Dispatcher.UIThread.Post(() => LoadingItems.Add(new NotificationItem(id, message, 0)));
        }
    }

    public void ReceiveProgress(Guid id, float progress) {
        var item = LoadingItems.FirstOrDefault(item => item.ID == id);
        if (item != null) {
            item.LoadProgress = progress;
        }
    }
    
    public void ReceiveStop(Guid id) {
        Dispatcher.UIThread.Post(() => LoadingItems.RemoveWhere(x => x.ID == id));
    }
}

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
        
        OpenSelectMods = ReactiveCommand.Create((Window window) => {
            var modSelectionWindow = new ModSelectionWindow(_modSelectionVM);
            modSelectionWindow.ShowDialog(window);
        });

        Save = ReactiveCommand.Create(() => {});
        
        OpenLog = ReactiveCommand.Create(() => {
            // DockingManagerService.AddAnchoredControl(
            //     new LogView(_lifetimeScope.Resolve<ILogVM>()),
            //     "Log",
            //     new DockingStatus { AnchorSide = AnchorSide.Bottom, Height = 200 });
        });
        
        OpenRecordBrowser = ReactiveCommand.Create(() => {
            // DockingManagerService.AddAnchoredControl(
            //     new RecordBrowser(_lifetimeScope.Resolve<IRecordBrowserVM>()),
            //     "Record Browser",
            //     new DockingStatus { Width = 600 });
        });
        
        _editorEnvironment.ActiveModChanged += EditorOnActiveModChanged;

        _notifier.Subscribe(this);
        
        // ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
        // ThemeManager.Current.SyncTheme();
        // Theme = ThemeManager.Current.DetectTheme()?.BaseColorScheme switch {
        //     "Dark" => new Vs2013DarkTheme(),
        //     _ => new Vs2013LightTheme(),
        // };
    }
    
    private void EditorOnActiveModChanged(object? sender, EventArgs e) {
        WindowTitle = _modSelectionVM.ActiveMod == null ? BaseWindowTitle : $"{BaseWindowTitle} - {_modSelectionVM.ActiveMod.Value}";
    }
}