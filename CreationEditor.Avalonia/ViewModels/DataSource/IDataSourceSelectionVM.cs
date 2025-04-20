using System.Collections.ObjectModel;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.DataSource;
using CreationEditor.Avalonia.Models.Selectables;
namespace CreationEditor.Avalonia.ViewModels.DataSource;

public interface IDataSourceSelectionVM {
    ReadOnlyObservableCollection<DataSourceItem> DataSources { get; }
    Func<IReactiveSelectable, bool> CanSelect { get; }
    bool ShowArchiveDataSources { get; }
    IBinding DataSourceIsEnabled { get; }
}
