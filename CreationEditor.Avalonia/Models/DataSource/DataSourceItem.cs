using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Services.DataSource;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.DataSource;

public partial class DataSourceItem(IDataSource dataSource) : ReactiveObject, IReactiveSelectable {
    [Reactive] public partial bool IsSelected { get; set; }
    public IDataSource DataSource { get; } = dataSource;
}
