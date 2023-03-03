using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Extension;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Services.Mutagen.References.Controller;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordProvider<TMajorRecord, TMajorRecordGetter> : ViewModel, IRecordProvider<ReferencedRecord<TMajorRecordGetter>>
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
    

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    [Reactive] public ReferencedRecord<TMajorRecordGetter>? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is ReferencedRecord<TMajorRecordGetter> referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }
    
    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }
    
    public IObservable<bool> IsBusy { get; set; }
    
    public IList<IMenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; init; }

    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    public RecordProvider(
        IRecordEditorController recordEditorController,
        IRecordController recordController,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IReferenceController referenceController) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newRecord);
            
            referenceController.GetRecord(newRecord, out var referencedRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });
        
        DoubleTapCommand = EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;

            var newOverride = recordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newOverride);
        });
        
        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            var duplicate = recordController.DuplicateRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            
            referenceController.GetRecord(duplicate, out var referencedRecord);
            RecordCache.AddOrUpdate(referencedRecord);
        });
        
        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord == null) return;
            
            recordController.DeleteRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        var cacheDisposable = new CompositeDisposable();

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(linkCache => {
                cacheDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                        cacheDisposable.Add(referenceController.GetRecord(record, out var referencedRecord));

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);
        
        IsBusy = isBusy;

        recordController.RecordChanged
            .Subscribe(majorRecord => {
                if (majorRecord is not TMajorRecordGetter record) return;
                
                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    referenceController.GetRecord(record, out var outListRecord);
                    listRecord = outListRecord;
                }
                
                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        ContextMenuItems = new List<IMenuItem> {
            new MenuItem { Header = "New", Command = NewRecord },
            new MenuItem { Header = "Edit", Command = EditSelectedRecord },
            new MenuItem { Header = "Duplicate", Command = DuplicateSelectedRecord },
            new MenuItem { Header = "Delete", Command = DeleteSelectedRecord },
        };
    }
}