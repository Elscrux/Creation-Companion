using System.Collections;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using Noggog;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public interface IRecordListVM : IDisposableDropoff {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }

    public IList<DataGridColumn> Columns { get; }

    public IObservable<bool> IsBusy { get; }
}
