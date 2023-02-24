using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Cache;

public interface IReferenceCache {
    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> modOrder);
}