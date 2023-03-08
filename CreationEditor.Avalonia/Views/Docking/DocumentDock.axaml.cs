using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class DocumentDock : ReactiveUserControl<DocumentDockVM>, IDockPreview {
    private DockDragData? _dockDragData;

    public DocumentDock() {
        InitializeComponent();
    }

    public DocumentDock(DocumentDockVM vm) : this() {
        DataContext = vm;
    }

    private void TabView_OnTabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args) {
        if (args.Item is IDockedItem dockedItem) {
            _dockDragData = new DockDragData { Item = dockedItem };
            args.Data.SetData(nameof(DockDragData), _dockDragData);
            dockedItem.DockRoot.SetEditMode(true);
        }
    }
    private void TabView_OnTabDragCompleted(TabView sender, TabViewTabDragCompletedEventArgs args) {
        if (args.Item is IDockedItem dockedItem) {
            dockedItem.DockRoot.SetEditMode(false);

            _dockDragData?.Preview?.HidePreview();
            _dockDragData = null;
        }
    }
}
