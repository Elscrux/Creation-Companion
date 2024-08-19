using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Mutagen.References.Record;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed class RecordListVM : ViewModel, IRecordListVM {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }
    [Reactive] public IReferencedRecord? SelectedRecord { get; set; }
    public IRecordContextMenuProvider RecordContextMenuProvider { get; }
    public ReactiveCommand<RecordListContext, Unit> PrimaryCommand { get; }

    public IObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollectionExtended<DataGridColumn>();

    public IObservable<bool> IsBusy { get; }

    private readonly Dictionary<Type, object> _settings = new();

    public RecordListVM(
        IRecordProvider recordProvider,
        IRecordContextMenuProvider recordContextMenuProvider,
        IObservable<Func<IReferencedRecord, bool>>? customFilter = null) {
        RecordProvider = recordProvider.DisposeWith(this);
        RecordContextMenuProvider = recordContextMenuProvider;

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

        PrimaryCommand = ReactiveCommand.Create<RecordListContext>(context => {
            RecordContextMenuProvider.ExecutePrimary(context);
        });
    }

    public IEnumerable<object> GetContextMenuItems(IReadOnlyList<IReferencedRecord> referencedRecords) {
        var recordListContext = GetRecordListContext(referencedRecords);

        return RecordContextMenuProvider.GetMenuItems(recordListContext);
    }

    public RecordListContext GetRecordListContext(IReadOnlyList<IReferencedRecord> referencedRecords) {
        return new RecordListContext(referencedRecords, RecordProvider.RecordTypes.ToList(), _settings);
    }

    public IRecordListVM AddSetting<T>(T t) {
        if (t is not null) _settings.Add(typeof(T), t);
        return this;
    }
}
