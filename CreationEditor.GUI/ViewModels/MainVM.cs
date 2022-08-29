using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;
using CreationEditor.GUI.Logging;
using CreationEditor.GUI.Models;
using CreationEditor.GUI.ViewModels.Mod;
using CreationEditor.GUI.Views.Controls.Record;
using CreationEditor.GUI.Views.Windows;
using Mutagen.Bethesda;
using MutagenLibrary.Notification;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI.ViewModels;

public class MainVM : ViewModel, INotificationReceiver {
    public const string BaseWindowTitle = "Creation Editor";
    public static readonly GameRelease GameRelease = GameRelease.SkyrimSE;

    public static readonly MainVM Instance = new();
    
    [Reactive] public string WindowTitle { get; set; } = BaseWindowTitle;

    [Reactive] public bool IsLoading { get; set; }
    public readonly Notifier Notifier = new  Notifier();
    public ObservableCollection<LoadingItem> LoadingItems { get; } = new();


    private readonly ModSelectionVM _modSelectionVM;

    public ReactiveCommand<Window, Unit> OpenSelectMods { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<DockingManager, LogView> OpenLog { get; }
    public ReactiveCommand<DockingManager, RecordBrowser> OpenRecordBrowser { get; }

    private MainVM() {
        _modSelectionVM = new ModSelectionVM(GameRelease, Notifier);
        
        OpenSelectMods = ReactiveCommand.Create((Window baseWindow) => {
            var modSelectionWindow = new ModSelectionWindow(_modSelectionVM) { Owner = baseWindow };
            modSelectionWindow.Show();
        });

        Save = ReactiveCommand.Create(() => {});
        
        OpenLog = ReactiveCommand.Create((DockingManager dockingManager) => dockingManager.AddControl<LogView>("Log", DockSide.Bottom, DockState.Dock));
        
        Editor.ActiveModChanged += EditorOnActiveModChanged;

        Notifier.Subscribe(this);
    }
    
    private void EditorOnActiveModChanged(object? sender, EventArgs e) {
        WindowTitle = _modSelectionVM.ActiveMod == null ? BaseWindowTitle : $"{BaseWindowTitle} - {_modSelectionVM.ActiveMod.Value}";
    }
    
    public void ReceiveNotify(string message, int level = 0) {
        while (LoadingItems.Count > level) Application.Current.Dispatcher.Invoke(() => LoadingItems.RemoveAt(level));

        Application.Current.Dispatcher.Invoke(() => LoadingItems.Add(new LoadingItem(message, 0)));
    }

    public void ReceiveProgress(float progress, int level = 0) {
        if (LoadingItems.Count > level) {
            LoadingItems[level].LoadProgress = progress;
        }
    }
}