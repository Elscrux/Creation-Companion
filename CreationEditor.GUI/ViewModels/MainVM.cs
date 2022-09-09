using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows;
using Autofac;
using CreationEditor.GUI.Services;
using CreationEditor.GUI.ViewModels.Mod;
using CreationEditor.GUI.ViewModels.Record;
using CreationEditor.GUI.Views.Controls.Record;
using CreationEditor.GUI.Views.Windows;
using CreationEditor.Services.Environment;
using Elscrux.Notification;
using Elscrux.WPF.Extensions;
using Elscrux.WPF.Models;
using Elscrux.WPF.ViewModels;
using Elscrux.WPF.Views.Logging;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.ViewModels;

public class MainVM : ViewModel, INotificationReceiver {
    public const string BaseWindowTitle = "Creation Editor";

    private readonly ILifetimeScope _lifetimeScope;
    private readonly INotifier _notifier;
    private readonly IEditorEnvironment _editorEnvironment;

    public IBusyService BusyService { get; }
    
    [Reactive] public bool IsLoading { get; set; }
    
    [Reactive] public string WindowTitle { get; set; } = BaseWindowTitle;

    public ObservableCollection<NotificationItem> LoadingItems { get; } = new();

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

        this.WhenAnyValue(x => x.BusyService.IsBusy)
            .BindTo(this, x => x.IsLoading);
        
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
    
    public void ReceiveNotify(Guid id, string message) {
        var item = LoadingItems.FirstOrDefault(item => item.ID == id);
        if (item != null) {
            item.LoadText = message;
        } else {
            Application.Current.Dispatcher.Invoke(() => LoadingItems.Add(new NotificationItem(id, message, 0)));
        }
    }

    public void ReceiveProgress(Guid id, float progress) {
        var item = LoadingItems.FirstOrDefault(item => item.ID == id);
        if (item != null) {
            item.LoadProgress = progress;
        }
    }
    
    public void ReceiveStop(Guid id) {
        Application.Current.Dispatcher.Invoke(() => LoadingItems.RemoveWhere(x => x.ID == id));
    }
}