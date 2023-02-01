using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Record;

public interface IReferencedRecord : IReferencedRecordIdentifier {
    public new IMajorRecordGetter Record { get; set; }
    
    public string RecordTypeName { get; }
}