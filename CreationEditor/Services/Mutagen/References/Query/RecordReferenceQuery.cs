using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Query;

public sealed class RecordReferenceQuery : IReferenceQuery<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> {
    public string Name => "Mod Form Links";

    public IModGetter? ReferenceToSource(IFormLinkIdentifier reference) => null;

    public void FillCache(IModGetter source, RecordReferenceCache cache) {
        foreach (var record in source.EnumerateMajorRecords()) {
            cache.FormKeys.Add(record.FormKey);

            foreach (var formLink in record.EnumerateFormLinks().Where(formLink => !formLink.IsNull)) {
                var references = cache.Cache.GetOrAdd(formLink.FormKey);
                references.Add(FormLinkInformation.Factory(record));
            }
        }
    }

    public static IEnumerable<IFormLinkIdentifier> ParseRecord(IMajorRecordGetter record) => record.EnumerateFormLinks();
}
