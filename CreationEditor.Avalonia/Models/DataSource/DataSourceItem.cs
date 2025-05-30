using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Services.DataSource;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.DataSource;

public partial class DataSourceItem(IDataSource dataSource, IEnumerable<IDataSource>? children = null) : ReactiveObject, IReactiveSelectable {
    [Reactive] public partial bool IsSelected { get; set; }
    [Reactive] public partial bool IsActive { get; set; }
    public IDataSource DataSource { get; } = dataSource;
    public IReadOnlyList<DataSourceItem> Children { get; } = children?
        .Select(dataSource => new DataSourceItem(dataSource)).ToList() ?? [];
}
