using Avalonia.ReactiveUI;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Browser;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Browser; 

public partial class CellBrowser : ReactiveUserControl<ICellBrowserVM> {
    public CellBrowser() {
        InitializeComponent();
    }
    
    public CellBrowser(ICellBrowserVM cellBrowserVM) : this() {
        DataContext = cellBrowserVM;
    }
}
