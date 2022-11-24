using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.WPF.Services.Record;

public interface IRecordEditorController {
    public void OpenEditor<TMajorRecord, TMajorRecordGetter>(TMajorRecord record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    public void CloseEditor(IMajorRecord record);

    public IObservable<IMajorRecordGetter> RecordChanged { get; }
}
