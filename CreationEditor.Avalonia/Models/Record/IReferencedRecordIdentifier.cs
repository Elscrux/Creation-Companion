using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Record;

public interface IReferenced {
    public HashSet<IFormLinkIdentifier> References { get; }
}

public interface IReferencedRecordIdentifier : IReferenced, IFormLinkIdentifier {
    public IMajorRecordIdentifier Record { get; set; }
}
