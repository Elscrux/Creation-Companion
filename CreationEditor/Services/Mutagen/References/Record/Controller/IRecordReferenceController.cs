using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public interface IRecordReferenceController : IReferenceController<IMajorRecordGetter> {
    /// <summary>
    /// Get references to the given form key.
    /// Note that this will only return the references that currently exist.
    /// The references might change with more updates.
    /// </summary>
    /// <param name="formKey">Form key to get references for</param>
    /// <returns>Enumerable of form links that reference the given form key</returns>
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey);

    /// <summary>
    /// Get a decorated record that always contains the latest information about references to this record. 
    /// </summary>
    /// <param name="record">Record to get referenced record version of</param>
    /// <param name="outReferencedRecord">Out variable that contains the referenced record</param>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Disposable that can be disposed to stop updates to the referenced record</returns>
    public IDisposable GetReferencedRecord<TMajorRecordGetter>(TMajorRecordGetter record, out IReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordGetter;
}
