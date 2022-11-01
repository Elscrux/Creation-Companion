using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using CreationEditor.Environment;
using CreationEditor.WPF.Models.Record;
using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Services.Record;
using DynamicData;
using DynamicData.Binding;
using Elscrux.WPF.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using MutagenLibrary.References.ReferenceCache;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.ViewModels.Record;

[TemplatePart(Name = "PART_RecordGrid", Type = typeof(DataGrid))]
public abstract class RecordListVM : DisposableUserControl, IRecordListVM {
    public const string RecordReadOnlyListStyle = "RecordReadOnlyListStyle";
    public const string RecordListStyle = "RecordListStyle";
    
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(RecordListVM));
    
    protected readonly IReferenceQuery ReferenceQuery;
    protected readonly IRecordController RecordController;

    public Type Type { get; set; } = null!;
    [Reactive] public IEnumerable Records { get; set; } = null!;
    [Reactive] public IRecordBrowserSettings RecordBrowserSettings { get; set; }

    public bool IsBusy {
        get => (bool) GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }
    
    protected readonly List<DataGridTextColumn> ExtraColumns = new();

    protected RecordListVM(
        IRecordBrowserSettings recordBrowserSettings,
        IReferenceQuery referenceQuery, 
        IRecordController recordController,
        bool isReadOnly) {
        ReferenceQuery = referenceQuery;
        RecordController = recordController;
        RecordBrowserSettings = recordBrowserSettings;
        
        SetResourceReference(StyleProperty, isReadOnly ? RecordReadOnlyListStyle : RecordListStyle);
    }

    public override void OnApplyTemplate() {
        base.OnApplyTemplate();

        var dataGrid = GetTemplateChild("PART_RecordGrid") as DataGrid;
        foreach (var column in ExtraColumns) {
            dataGrid?.Columns.Add(column);
        }
    }
}

public class RecordListVM<TMajorRecord, TMajorRecordGetter> : RecordListVM
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private readonly IRecordEditorController _recordEditorController;

    public ReferencedRecord<TMajorRecord, TMajorRecordGetter>? SelectedRecord { get; set; }
    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    private IObservableCache<ReferencedRecord<TMajorRecord, TMajorRecordGetter>, FormKey> _recordCache;

    [Reactive] public new IObservableCollection<ReferencedRecord<TMajorRecord, TMajorRecordGetter>> Records { get; set; }

    public RecordListVM(
        IReferenceQuery referenceQuery,
        IRecordBrowserSettings recordBrowserSettings,
        IRecordEditorController recordEditorController,
        IRecordController recordController)
        : base(recordBrowserSettings, referenceQuery, recordController, false) {
        _recordEditorController = recordEditorController;
        Type = typeof(TMajorRecordGetter);
        
        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = RecordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
            _recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newRecord);
        });
        
        EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = RecordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            _recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newOverride);
        });
        
        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            RecordController.DuplicateRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
        });
        
        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            Application.Current.Dispatcher.Invoke(() => RecordController.DeleteRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record));
            Records.Remove(SelectedRecord);
        });
        
        _recordCache = this.WhenAnyValue(x => x.RecordBrowserSettings.LinkCache, x => x.RecordBrowserSettings.SearchTerm)
            .RepublishLatestOnSignal(
                Observable.Merge(
                    NewRecord.EndingExecution(),
                    DuplicateSelectedRecord.EndingExecution(),
                    DeleteSelectedRecord.EndingExecution()))
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Do(_ => IsBusy = true)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .Select(x => {
                return Observable.Create<ReferencedRecord<TMajorRecord, TMajorRecordGetter>>(async (obs, cancel) => {
                    foreach (var record in x.Item1.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                        if (cancel.IsCancellationRequested) return;

                        //Skip when browser settings don't match
                        if (!RecordBrowserSettings.Filter(record)) continue;

                        var formLinks = ReferenceQuery.GetReferences(record.FormKey, RecordBrowserSettings.LinkCache);
                        var referencedRecord = new ReferencedRecord<TMajorRecord, TMajorRecordGetter>(record, formLinks);

                        obs.OnNext(referencedRecord);
                    }
                    obs.OnCompleted();
                });
            })
            .Select(x => x.ToObservableChangeSet(x => x.Record.FormKey))
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(_ => IsBusy = false)
            .ObserveOn(RxApp.TaskpoolScheduler)
            .AsObservableCache();

        Records = _recordCache.Connect()
            .ToObservableCollection(this);

        _recordEditorController.RecordChanged
            .Subscribe(e => {
                if (e.Record is not TMajorRecordGetter record) return;
                if (!_recordCache.TryGetValue(e.Record.FormKey, out var listRecord)) return;
                listRecord.Record = record;
            });
    }
}
