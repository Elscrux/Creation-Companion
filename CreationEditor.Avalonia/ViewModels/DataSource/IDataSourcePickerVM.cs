using System.Collections.ObjectModel;
using CreationEditor.Services.DataSource;
namespace CreationEditor.Avalonia.ViewModels.DataSource;

public interface IDataSourcePickerVM {
    ReadOnlyObservableCollection<IDataSource> DataSources { get; }
    Func<IDataSource, bool>? Filter { get; set; }
}
