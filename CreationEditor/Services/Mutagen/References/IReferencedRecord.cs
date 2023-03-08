using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferencedRecord : IReferencedRecordIdentifier {
    public new IMajorRecordGetter Record { get; set; }

    public string RecordTypeName { get; }
}
