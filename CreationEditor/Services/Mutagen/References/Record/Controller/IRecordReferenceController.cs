using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public interface IRecordReferenceController : IReferenceController<IMajorRecordGetter> {
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey);

    public IDisposable GetReferencedRecord<TMajorRecordGetter>(TMajorRecordGetter record, out IReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordGetter;
}
