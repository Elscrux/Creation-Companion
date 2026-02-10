using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Services.DataSource;
using DynamicData;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.DataSource;

public sealed partial class SingleDataSourcePickerVM : ViewModel, IDataSourcePickerVM {
    public ReadOnlyObservableCollection<IDataSource> DataSources { get; }

    [Reactive] public partial Func<IDataSource, bool>? Filter { get; set; }
    [Reactive] public partial IDataSource? SelectedDataSource { get; set; }
    [Reactive] public partial string SelectionText { get; set; }

    public IObservable<bool> HasDataSourceSelected { get; }
    public IObservable<IDataSource?> SelectedDataSourceChanged { get; }

    public SingleDataSourcePickerVM(IDataSourceService dataSourceService) {
        SelectionText = string.Empty;

        DataSources = dataSourceService.DataSourcesChanged
            .Select(x => x.AsObservableChangeSet())
            .Switch()
            .Filter(this.WhenAnyValue(x => x.Filter)
                .Select(filter => new Func<IDataSource, bool>(dataSource => filter is null || filter(dataSource))))
            .ToObservableCollection(this);

        this.WhenAnyValue(x => x.Filter)
            .Subscribe(filter => {
                if (filter is null) {
                    SelectedDataSource ??= DataSources.FirstOrDefault();
                } else if (SelectedDataSource is null || !filter(SelectedDataSource)) {
                    SelectedDataSource = DataSources.FirstOrDefault(filter);
                }
            })
            .DisposeWith(this);

        DataSources.WhenCollectionChanges()
            .Subscribe(_ => {
                if (SelectedDataSource is null || (Filter is not null && !Filter(SelectedDataSource))) {
                    SelectedDataSource = DataSources.FirstOrDefault(d => d.Name.Equals(SelectionText, StringComparison.OrdinalIgnoreCase));
                }
            })
            .DisposeWith(this);

        HasDataSourceSelected = this.WhenAnyValue(x => x.SelectedDataSource)
            .Select(dataSource => dataSource is not null)
            .Replay(1)
            .RefCount();
        SelectedDataSourceChanged = this.WhenAnyValue(x => x.SelectedDataSource)
            .Replay(1)
            .RefCount();
    }

    public void SelectDataSource(IDataSource dataSource) {
        if (DataSources.Contains(dataSource)) {
            SelectedDataSource = dataSource;
        }
    }
}
