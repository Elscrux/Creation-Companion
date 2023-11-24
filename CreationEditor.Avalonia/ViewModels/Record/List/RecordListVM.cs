using System.Collections;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVM : ViewModel, IRecordListVM {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }
    public IRecordContextMenuProvider RecordContextMenuProvider { get; }

    public IObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollectionExtended<DataGridColumn>();

    public IObservable<bool> IsBusy { get; }

    public RecordListVM(
        IRecordProvider recordProvider,
        Func<IObservable<IMajorRecordGetter?>, IRecordContextMenuProvider> recordContextMenuProviderFactory,
        IObservable<Func<IReferencedRecord,bool>>? customFilter = null) {
        RecordProvider = recordProvider.DisposeWith(this);
        var selectedRecordObservable = this
            .WhenAnyValue(vm => vm.SelectedRecord)
            .Select(referencedRecord => referencedRecord?.Record);
        RecordContextMenuProvider = recordContextMenuProviderFactory(selectedRecordObservable);

        Records = RecordProvider.RecordCache
            .Connect()
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .WrapInInProgressMarker(
                x => {
                    var observable = x.Filter(RecordProvider.Filter, false);
                    if (customFilter is not null) observable = observable.Filter(customFilter);
                    return observable;
                },
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
