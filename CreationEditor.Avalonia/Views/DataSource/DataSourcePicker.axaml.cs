using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Avalonia.ViewModels.Mod;
namespace CreationEditor.Avalonia.Views.DataSource;

public partial class DataSourcePicker : ReactiveUserControl<IDataSourcePickerVM> {
    public DataSourcePicker() {
        InitializeComponent();
    }

    public DataSourcePicker(IDataSourcePickerVM vm) : this() {
        DataContext = vm;
    }
}
