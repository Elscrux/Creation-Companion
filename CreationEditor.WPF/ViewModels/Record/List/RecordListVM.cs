using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor.Environment;
using CreationEditor.WPF.Models.Record;
using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Services.Record;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.ViewModels.Record;

public abstract class RecordListVM : ViewModel, IRecordListVM {
    protected readonly IReferenceQuery ReferenceQuery;
    protected readonly IRecordController RecordController;

    public Type Type { get; set; } = null!;
    [Reactive] public IEnumerable Records { get; set; } = null!;
    [Reactive] public IRecordBrowserSettings RecordBrowserSettings { get; set; }

    [Reactive] public bool IsBusy { get; set; }

    protected RecordListVM(
        IRecordBrowserSettings recordBrowserSettings,
        IReferenceQuery referenceQuery, 
        IRecordController recordController) {
        ReferenceQuery = referenceQuery;
        RecordController = recordController;
        RecordBrowserSettings = recordBrowserSettings;
    }
}

public class RecordListVM<TMajorRecord, TMajorRecordGetter> : RecordListVM
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {

    public ReferencedRecord<TMajorRecord, TMajorRecordGetter>? SelectedRecord { get; set; }
    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    protected static readonly SourceCache<ReferencedRecord<TMajorRecord, TMajorRecordGetter>, FormKey> UpdateCache = new(x => x.Record.FormKey);

    public new IObservableCollection<ReferencedRecord<TMajorRecord, TMajorRecordGetter>> Records { get; }

    public RecordListVM(
        IReferenceQuery referenceQuery,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordEditorController recordEditorController,
        IRecordController recordController)
        : base(recordBrowserSettings, referenceQuery, recordController) {
        Type = typeof(TMajorRecordGetter);
        
        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = RecordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newRecord);
            
            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(newRecord);
            UpdateCache.AddOrUpdate(referencedRecord);
        });
        
        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = RecordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newOverride);
        });
        
        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            var duplicate = RecordController.DuplicateRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            
            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(duplicate);
            UpdateCache.AddOrUpdate(referencedRecord);
        });
        
        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            Dispatcher.UIThread.Post(() => RecordController.DeleteRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record));
            Records!.Remove(SelectedRecord);
        });
        
        var recordCache = this.WhenAnyValue(x => x.RecordBrowserSettings.LinkCache, x => x.RecordBrowserSettings.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Where(x => x.Item1.ListedOrder.Count > 0)
            .Do(_ => IsBusy = true)
            .ObserveOnTaskpool()
            .Select(x => {
                return Observable.Create<ReferencedRecord<TMajorRecord, TMajorRecordGetter>>((obs, cancel) => {
                    foreach (var record in x.Item1.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;

                        //Skip when browser settings don't match
                        if (!RecordBrowserSettings.Filter(record)) continue;

                        var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettings.LinkCache);
                        var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(record, formLinks);

                        obs.OnNext(referencedRecord);
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                }).ToObservableChangeSet(refRecord => refRecord.Record.FormKey);
            })
            .Switch()
            .ObserveOnGui()
            .Do(_ => IsBusy = false)
            .ObserveOnTaskpool()
            .AsObservableCache();

        var finalCache = Observable.Merge(
            recordCache.Connect(),
            UpdateCache.Connect())
            .AsObservableCache();
        
        Records = finalCache
            .Connect()
            .ToObservableCollection(this);

        recordEditorController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not TMajorRecordGetter record) return;
                if (!finalCache.TryGetValue(record.FormKey, out var listRecord)) return;
                
                listRecord.Record = record;
                UpdateCache.AddOrUpdate(listRecord);
            });
    }
}