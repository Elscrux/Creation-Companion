using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.DataSource;
namespace CreationEditor.Avalonia.Views.DataSource;

public partial class DataSourcePicker : ReactiveUserControl<IDataSourcePickerVM> {
    public DataSourcePicker() {
        InitializeComponent();
    }

    public DataSourcePicker(IDataSourcePickerVM vm) : this() {
        DataContext = vm;
    }
}
