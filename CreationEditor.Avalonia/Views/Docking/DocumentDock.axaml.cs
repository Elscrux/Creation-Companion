using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking; 

public partial class DocumentDock : ReactiveUserControl<DocumentDockVM> {
    public DocumentDock() {
        InitializeComponent();
    }
    
    public DocumentDock(DocumentDockVM vm) : this() {
        DataContext = vm;
    }
}

