using System.Collections;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using DynamicData;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVM : ViewModel, IRecordListVM {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }

    public IList<DataGridColumn> Columns { get; } = new List<DataGridColumn>();

    public IObservable<bool> IsBusy { get; }

    public RecordListVM(IRecordProvider recordProvider) {
        RecordProvider = recordProvider.DisposeWith(this);

        Records = RecordProvider.RecordCache
            .Connect()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .WrapInInProgressMarker(
                x => x.Filter(RecordProvider.Filter, false),
                out var isFiltering)
            .ToObservableCollection(this);

        IsBusy = isFiltering
            .CombineLatest(
                RecordProvider.IsBusy,
                (filtering, busy) => (Filtering: filtering, Busy: busy))
            .ObserveOnGui()
            .Select(list => list.Filtering || list.Busy);
    }
}
