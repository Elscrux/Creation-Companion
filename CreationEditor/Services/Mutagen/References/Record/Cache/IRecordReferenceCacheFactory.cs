using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public interface IRecordReferenceCacheFactory {
    Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(IReadOnlyList<IModGetter> mods);
    Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(ImmutableRecordReferenceCache immutableRecordReferenceCache);
    Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IReadOnlyList<IMod> mutableMods, IReadOnlyList<IModGetter> mods);
    Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IReadOnlyList<IMod> mutableMods, ImmutableRecordReferenceCache? immutableRecordReferenceCache = null);
}
