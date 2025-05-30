using System.Collections;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.DataSource;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.ViewModels.Dialog;
using DynamicData.Binding;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.DataSource;

public interface IDataSourceSelectionVM : ISaveDialogVM {
    ObservableCollectionExtended<DataSourceItem> DataSources { get; }
    Func<IReactiveSelectable, bool> CanSelect { get; }
    IBinding DataSourceIsEnabled { get; }
    string? AddedDataSourcePath { get; set; }
    ReactiveCommand<IList, Unit> RemoveDataSource { get; }
    ReactiveCommand<Unit, Unit> RefreshDataSources { get; }
    ReactiveCommand<Unit, Unit> ApplyDataSourcesChanges { get; }
    IObservable<bool> AnyLocalChanges { get; }
    ITreeDataGridSource DataSourceTreeSource { get; }
    Func<object, bool> CanRemoveDataSource { get; }
}
