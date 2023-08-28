using CreationEditor.Services.Mutagen.References.Record.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Query;

public interface IRecordReferenceQuery {
    IReadOnlyDictionary<ModKey, ModReferenceCache> ModCaches { get; }

    void LoadModReferences(IModGetter mod);
    Task LoadModReferences(IReadOnlyList<IModGetter> mods);
}
