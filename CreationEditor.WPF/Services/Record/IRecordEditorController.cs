using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.WPF.Services.Record;

public class RecordEventArgs : EventArgs {
    public IMajorRecordGetter Record { get; set; }
    
    public RecordEventArgs(IMajorRecordGetter record) => Record = record;
}

public interface IRecordEditorController {
    public void OpenEditor<TMajorRecord, TMajorRecordGetter>(TMajorRecord record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    public void CloseEditor(IMajorRecord record);
    
    public event EventHandler<RecordEventArgs>? RecordChanged; 
}
