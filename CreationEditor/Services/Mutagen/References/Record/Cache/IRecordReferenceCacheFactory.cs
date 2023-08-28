using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public interface IRecordReferenceCacheFactory {
    Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(IReadOnlyList<IModGetter> mods);
    Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(ImmutableRecordReferenceCache immutableRecordReferenceCache);
    Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IMod mutableMod, IReadOnlyList<IModGetter> mods);
    Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IMod mutableMod, ImmutableRecordReferenceCache? immutableRecordReferenceCache = null);
}
