using System.Collections;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Provider;
using DynamicData;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordSelectionVM : ViewModel {
    public IRecordProvider RecordProvider { get; }

    public IEnumerable? Records { get; }

    public IObservable<bool> IsBusy { get; }

    public RecordSelectionVM(
        IRecordProvider recordProvider) {
        RecordProvider = recordProvider.DisposeWith(this);

        Records = RecordProvider.RecordCache
            .Connect()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .WrapInProgressMarker(
                x => x.Filter(RecordProvider.Filter, false),
                out var isFiltering)
            .ToObservableCollection(this);

        IsBusy = isFiltering
            .CombineLatest(
                RecordProvider.IsBusy,
                (filtering, busy) => filtering || busy)
            .ObserveOnGui()
            .Publish()
            .RefCount();
    }
}
