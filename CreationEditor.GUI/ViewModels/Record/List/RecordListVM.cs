using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using CreationEditor.Environment;
using CreationEditor.GUI.Models.Record;
using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.Services.Record;
using CreationEditor.GUI.Views.Record;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using MutagenLibrary.References.ReferenceCache;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Record;

public abstract class RecordListVM : ViewModel, IRecordListVM {
    protected readonly IReferenceQuery ReferenceQuery;
    protected readonly IRecordController RecordController;

    public UserControl View { get; set; } = null!;

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
    private readonly IRecordEditorController _recordEditorController;

    public ReferencedRecord<TMajorRecord, TMajorRecordGetter>? SelectedRecord { get; set; }
    public ReactiveCommand<Unit, TMajorRecord> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, TMajorRecord?> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    [Reactive] public new IObservableCollection<ReferencedRecord<TMajorRecord, TMajorRecordGetter>> Records { get; set; }

    public RecordListVM(
        IReferenceQuery referenceQuery,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordEditorController recordEditorController,
        IRecordController recordController)
        : base(recordBrowserSettings, referenceQuery, recordController) {
        _recordEditorController = recordEditorController;
        View = new RecordList(this);
        Type = typeof(TMajorRecordGetter);
        
        Records = this.WhenAnyValue(x => x.RecordBrowserSettings.LinkCache, x => x.RecordBrowserSettings.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                IsBusy = true;
                return Observable.Create<ReferencedRecord<TMajorRecord, TMajorRecordGetter>>((obs, cancel) => {
                    Records?.Clear();
                    try {
                        foreach (var recordIdentifier in x.Item1.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                            if (cancel.IsCancellationRequested) return Task.CompletedTask;

                            //Skip when browser settings don't match
                            if (!RecordBrowserSettings.Filter(recordIdentifier)) continue;

                            var formLinks = ReferenceQuery.GetReferences(recordIdentifier.FormKey, RecordBrowserSettings.LinkCache);
                            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(recordIdentifier, formLinks);

                            obs.OnNext(referencedRecord);
                        }
                        obs.OnCompleted();
                        return Task.CompletedTask;
                    } finally {
                        IsBusy = false;
                    }
                });
            })
            .Select(x => x.ToObservableChangeSet())
            .Switch()
            .ToObservableCollection(this);

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = RecordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(newRecord);
            Application.Current.Dispatcher.Invoke(() => Records.Add(referencedRecord));
            
            _recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newRecord);

            return newRecord;
        });
        
        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = RecordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            _recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newOverride);
        });
        
        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return null;
            
            var newRecord = RecordController.DuplicateRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(newRecord);
            Application.Current.Dispatcher.Invoke(() => Records.Add(referencedRecord));
            return newRecord;
        });
        
        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            Application.Current.Dispatcher.Invoke(() => RecordController.DeleteRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record));
            Records.Remove(SelectedRecord);
        });
    }
}
