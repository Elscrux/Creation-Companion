using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record;

public interface IReferencedRecordIdentifier : IReferenced, IFormLinkIdentifier {
    IMajorRecordIdentifierGetter Record { get; set; }
}
