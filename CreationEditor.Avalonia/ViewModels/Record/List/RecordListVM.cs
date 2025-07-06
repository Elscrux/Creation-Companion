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
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Record.List;

public sealed partial class RecordListVM : ViewModel, IRecordListVM {
    public IEnumerable? Records { get; }

    public IRecordProvider RecordProvider { get; }
    [Reactive] public partial IReferencedRecord? SelectedRecord { get; set; }
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
            .WrapInProgressMarker(
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
                (filtering, busy) => filtering || busy)
            .ObserveOnGui();

        PrimaryCommand = ReactiveCommand.Create<RecordListContext>(context => {
            RecordContextMenuProvider.ExecutePrimary(context);
        });
    }

    public RecordListContext GetRecordListContext(IReadOnlyList<IReferencedRecord> referencedRecords) {
        return new RecordListContext(referencedRecords, RecordProvider.RecordTypes.ToList(), _settings);
    }

    public IRecordListVM AddSetting<T>(T t) {
        if (t is not null) _settings.Add(typeof(T), t);
        return this;
    }
}
