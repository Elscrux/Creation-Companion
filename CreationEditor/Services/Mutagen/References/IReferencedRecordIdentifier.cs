using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferenced {
    public ICollection<IFormLinkIdentifier> References { get; }
}
public interface IReferencedRecordIdentifier : IReferenced, IFormLinkIdentifier {
    public IMajorRecordIdentifier Record { get; set; }
}
