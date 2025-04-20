using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.DataSource;
namespace CreationEditor.Avalonia.Views.DataSource;

public partial class DataSourceSelection : ReactiveUserControl<IDataSourceSelectionVM> {
    public DataSourceSelection() {
        InitializeComponent();
    }
}
