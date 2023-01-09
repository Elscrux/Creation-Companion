using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Views.Docking; 

public partial class DocumentDock : ReactiveUserControl<DocumentDockVM>, IDockPreview {
    public DocumentDock() {
        InitializeComponent();
    }
    
    public DocumentDock(DocumentDockVM vm) : this() {
        DataContext = vm;
    }
    
    private void TabView_OnTabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args) {
        if (args.Item is IDockedItem dockedItem) {
            args.Data.SetData(nameof(DockDragData), new DockDragData { Item = dockedItem });
        }
    }
}

