using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Provider;

public sealed class RecordProvider<TMajorRecord, TMajorRecordGetter> : ViewModel, IRecordProvider<IReferencedRecord<TMajorRecordGetter>>
    where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private readonly CompositeDisposable _referencesDisposable = new();

    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }

    public SourceCache<IReferencedRecord, FormKey> RecordCache { get; } = new(x => x.Record.FormKey);

    [Reactive] public IReferencedRecord<TMajorRecordGetter>? SelectedRecord { get; set; }
    IReferencedRecord? IRecordProvider.SelectedRecord {
        get => SelectedRecord;
        set {
            if (value is IReferencedRecord<TMajorRecordGetter> referencedRecord) {
                SelectedRecord = referencedRecord;
            }
        }
    }

    public IObservable<Func<IReferencedRecord, bool>> Filter { get; }

    public IObservable<bool> IsBusy { get; set; }

    public IList<MenuItem> ContextMenuItems { get; }
    public ReactiveCommand<Unit, Unit>? DoubleTapCommand { get; }

    public ReactiveCommand<Unit, Unit> NewRecord { get; }
    public ReactiveCommand<Unit, Unit> EditSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DuplicateSelectedRecord { get; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedRecord { get; }

    public RecordProvider(
        IMenuItemProvider menuItemProvider,
        IRecordEditorController recordEditorController,
        IRecordController recordController,
        IRecordBrowserSettingsVM recordBrowserSettingsVM,
        IRecordReferenceController recordReferenceController) {
        RecordBrowserSettingsVM = recordBrowserSettingsVM;

        Filter = IRecordProvider<IReferencedRecord>.DefaultFilter(RecordBrowserSettingsVM);

        NewRecord = ReactiveCommand.Create(() => {
            var newRecord = recordController.CreateRecord<TMajorRecord, TMajorRecordGetter>();
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newRecord);
        });

        DoubleTapCommand = EditSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            var newOverride = recordController.GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            recordEditorController.OpenEditor<TMajorRecord, TMajorRecordGetter>(newOverride);
        });

        DuplicateSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            recordController.DuplicateRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
        });

        DeleteSelectedRecord = ReactiveCommand.Create(() => {
            if (SelectedRecord is null) return;

            recordController.DeleteRecord<TMajorRecord, TMajorRecordGetter>(SelectedRecord.Record);
            RecordCache.Remove(SelectedRecord);
        });

        this.WhenAnyValue(x => x.RecordBrowserSettingsVM.LinkCache)
            .ObserveOnTaskpool()
            .WrapInInProgressMarker(x => x.Do(linkCache => {
                _referencesDisposable.Clear();

                RecordCache.Clear();
                RecordCache.Edit(updater => {
                    foreach (var record in linkCache.PriorityOrder.WinningOverrides<TMajorRecordGetter>()) {
                        recordReferenceController.GetReferencedRecord(record, out var referencedRecord).DisposeWith(_referencesDisposable);

                        updater.AddOrUpdate(referencedRecord);
                    }
                });
            }), out var isBusy)
            .Subscribe()
            .DisposeWith(this);

        IsBusy = isBusy;

        recordController.RecordChanged
            .Merge(recordController.RecordCreated)
            .Subscribe(majorRecord => {
                if (majorRecord is not TMajorRecordGetter record) return;

                if (RecordCache.TryGetValue(record.FormKey, out var listRecord)) {
                    // Modify value
                    listRecord.Record = record;
                } else {
                    // Create new entry
                    recordReferenceController.GetReferencedRecord(record, out var outListRecord).DisposeWith(_referencesDisposable);
                    listRecord = outListRecord;
                }

                // Force update
                RecordCache.AddOrUpdate(listRecord);
            })
            .DisposeWith(this);

        recordController.RecordDeleted
            .Subscribe(record => RecordCache.RemoveKey(record.FormKey))
            .DisposeWith(this);

        ContextMenuItems = new List<MenuItem> {
            menuItemProvider.New(NewRecord),
            menuItemProvider.Edit(EditSelectedRecord),
            menuItemProvider.Duplicate(DuplicateSelectedRecord),
            menuItemProvider.Delete(DeleteSelectedRecord),
        };
    }

    public override void Dispose() {
        base.Dispose();

        _referencesDisposable.Dispose();
        RecordCache.Clear();
        RecordCache.Dispose();
    }
}
