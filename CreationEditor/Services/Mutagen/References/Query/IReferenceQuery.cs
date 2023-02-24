using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Query;

public interface IReferenceQuery {
    IReadOnlyDictionary<ModKey, ReferenceQuery.ModReferenceCache> ModCaches { get; }

    void LoadModReferences(IModGetter mod);
    void LoadModReferences(IReadOnlyList<IModGetter> mods);
}
