using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Mutagen.References;
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
    public IContextMenuProvider ContextMenuProvider { get; }
    public ReactiveCommand<SelectedListContext, Unit> PrimaryCommand { get; }

    public IObservableCollection<DataGridColumn> Columns { get; } = new ObservableCollectionExtended<DataGridColumn>();

    public IObservable<bool> IsBusy { get; }

    private readonly Dictionary<Type, object> _settings = new();

    public RecordListVM(
        IRecordProvider recordProvider,
        IContextMenuProvider contextMenuProvider,
        IObservable<Func<IReferencedRecord, bool>>? customFilter = null) {
        RecordProvider = recordProvider.DisposeWith(this);
        ContextMenuProvider = contextMenuProvider;

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
            .ObserveOnGui()
            .Publish()
            .RefCount();

        PrimaryCommand = ReactiveCommand.Create<SelectedListContext>(context => {
            ContextMenuProvider.ExecutePrimary(context);
        });
    }

    public SelectedListContext GetRecordListContext(IReadOnlyList<IReferencedRecord> referencedRecords) {
        return new SelectedListContext(referencedRecords, [], RecordProvider.RecordTypes.ToList(), _settings);
    }

    public IRecordListVM AddSetting<T>(T t) {
        if (t is not null) _settings.Add(typeof(T), t);
        return this;
    }
}
