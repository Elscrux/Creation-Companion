using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Models.Mod.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Services.Record.Editor;

public enum RecordUpdateReason {
    /**
     * Record was updated via the editor.
     */
    Edit,
    /**
     * Record was updated by going to a state in the saved history.
     */
    History,
    /**
     * Record was updated by an external source.
     */
    External,
}

public record RecordUpdate<TMajorRecord>(TMajorRecord Record, RecordUpdateReason Reason)
    where TMajorRecord : class, IMajorRecordGetter;

public interface IRecordEditorCore : IDisposableDropoff {
    ReactiveCommand<Unit, Unit> Save { get; }
    ReactiveCommand<Unit, Unit> Undo { get; }
    ReactiveCommand<Unit, Unit> Redo { get; }

    ILinkCacheProvider LinkCacheProvider { get; }
}

public interface IRecordEditorCore<TEditableRecord, TMajorRecord, TMajorRecordGetter> : IRecordEditorCore
    where TEditableRecord : class, IEditableRecord<TMajorRecord>, TMajorRecord
    where TMajorRecord : class, IMajorRecordInternal, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    RecordEditorHistory<TEditableRecord, TMajorRecord, TMajorRecordGetter> RecordEditorHistory { get; }
    RecordEditorValidator<TEditableRecord, TMajorRecord, TMajorRecordGetter> RecordEditorValidator { get; }
    TMajorRecord Record { get; set; }
    TEditableRecord EditableRecord { get; set; }
    IObservable<RecordUpdate<TMajorRecordGetter>> RecordChanged { get; }
}

public class EditableRecordConverter<TEditableRecord, TMajorRecord, TMajorRecordGetter>(
    Func<TMajorRecordGetter, TEditableRecord> convertToEditable,
    Func<TEditableRecord, TMajorRecord> convertToRecord)
    where TEditableRecord : class, IEditableRecord<TMajorRecord>, TMajorRecord
    where TMajorRecord : class, IMajorRecordInternal, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    public TMajorRecord ConvertToRecord(TEditableRecord editableRecord) => convertToRecord(editableRecord);
    public TEditableRecord ConvertToEditable(TMajorRecordGetter majorRecord) => convertToEditable(majorRecord);
}

public sealed class RecordEditorCore<TEditableRecord, TMajorRecord, TMajorRecordGetter>
    : ViewModel, IRecordEditorCore<TEditableRecord, TMajorRecord, TMajorRecordGetter>
    where TEditableRecord : class, IEditableRecord<TMajorRecord>, TMajorRecord
    where TMajorRecord : class, IMajorRecordInternal, TMajorRecordGetter
    where TMajorRecordGetter : class, IMajorRecordGetter {
    private readonly EditableRecordConverter<TEditableRecord, TMajorRecord, TMajorRecordGetter> _convertEditableRecord;
    public RecordEditorHistory<TEditableRecord, TMajorRecord, TMajorRecordGetter> RecordEditorHistory { get; }
    public RecordEditorValidator<TEditableRecord, TMajorRecord, TMajorRecordGetter> RecordEditorValidator { get; }
    public ILinkCacheProvider LinkCacheProvider { get; }

    public TMajorRecord Record { get; set; }
    [Reactive] public TEditableRecord EditableRecord { get; set; }

    private readonly Subject<RecordUpdate<TMajorRecordGetter>> _recordChanged = new();
    public IObservable<RecordUpdate<TMajorRecordGetter>> RecordChanged => _recordChanged;

    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<Unit, Unit> Undo { get; }
    public ReactiveCommand<Unit, Unit> Redo { get; }
    public ReactiveCommand<IDatedItem<TMajorRecord>, Unit> ApplyRecordHistory { get; }

    public RecordEditorCore(
        TMajorRecord record,
        Func<IRecordEditorCore<TEditableRecord, TMajorRecord, TMajorRecordGetter>, RecordEditorValidator<TEditableRecord, TMajorRecord, TMajorRecordGetter>> recordEditorValidatorFactory,
        EditableRecordConverter<TEditableRecord, TMajorRecord, TMajorRecordGetter> convertEditableRecord,
        IRecordController recordController,
        IRecordEditorController recordEditorController,
        ILinkCacheProvider linkCacheProvider) {
        _convertEditableRecord = convertEditableRecord;

        Record = record;
        EditableRecord = convertEditableRecord.ConvertToEditable(Record);

        LinkCacheProvider = linkCacheProvider;
        RecordEditorValidator = recordEditorValidatorFactory(this);
        RecordEditorHistory = new RecordEditorHistory<TEditableRecord, TMajorRecord, TMajorRecordGetter>(this,
            r => {
                EditableRecord = convertEditableRecord.ConvertToEditable(r);
                UpdateRecord(RecordUpdateReason.History);
            });

        Save = ReactiveCommand.Create(() => {
            recordController.RegisterUpdate(Record, () => EditableRecord.CopyTo(Record));
            recordEditorController.CloseEditor(EditableRecord);
        });
        Undo = ReactiveCommand.Create(RecordEditorHistory.Undo);
        Redo = ReactiveCommand.Create(RecordEditorHistory.Redo);
        ApplyRecordHistory = ReactiveCommand.Create<IDatedItem<TMajorRecord>>(RecordEditorHistory.SetToState);

        this.WhenAnyValue(x => x.EditableRecord)
            .Subscribe(editableRecord => {
                (editableRecord as INotifyPropertyChanged).Events().PropertyChanged
                    .Subscribe(_ => UpdateRecord(RecordUpdateReason.Edit));
                // todo, merge all nested fields (or forward them via the EditableRecord implementation?) .Merge((editableRecord.NestedRecord as INotifyPropertyChanged).Events().PropertyChanged)
            })
            .DisposeWith(this);

        recordController.RecordInActiveModChanged
            .OfType<TMajorRecordGetter>()
            .Where(r => r.FormKey == Record.FormKey)
            .Subscribe(r => {
                EditableRecord = convertEditableRecord.ConvertToEditable(r);
                UpdateRecord( RecordUpdateReason.External);
            })
            .DisposeWith(this);
    }

    private void UpdateRecord(RecordUpdateReason reason) {
        _recordChanged.OnNext(new RecordUpdate<TMajorRecordGetter>(_convertEditableRecord.ConvertToRecord(EditableRecord), reason));
    }
}
