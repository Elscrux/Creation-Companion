using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.DataSource;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.DataSource;
using CreationEditor.Services.DataSource;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.DataSource;

public sealed partial class DataSourceSelectionVM : ViewModel, IDataSourceSelectionVM {
    public ReadOnlyObservableCollection<DataSourceItem> DataSources { get; }
    public Func<IReactiveSelectable, bool> CanSelect => selectable => selectable is DataSourceItem { DataSource.IsReadOnly: false };
    [Reactive] public partial bool ShowArchiveDataSources { get; set; }
    public IBinding DataSourceIsEnabled { get; } = new Binding($"{nameof(DataSourceItem.DataSource)}.{nameof(DataSourceItem.DataSource.IsReadOnly)}");

    public DataSourceSelectionVM(
        IDataSourceService dataSourceService) {
        DataSources = dataSourceService.DataSourcesChanged
            .Select(dataSources => dataSources.Select(d => new DataSourceItem(d) { IsSelected = true }))
            .ToObservableCollection(this);
    }
}
