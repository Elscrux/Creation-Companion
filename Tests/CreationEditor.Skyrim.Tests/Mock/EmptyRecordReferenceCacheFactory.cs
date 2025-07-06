using System.Collections.Concurrent;
using AutoFixture;
using AutoFixture.Kernel;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Mutagen.References.Record.Query;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Skyrim.Tests.Mock;

public sealed class EmptyRecordReferenceCacheFactory(ISpecimenBuilder builder) : IRecordReferenceCacheFactory {
    private readonly RecordReferenceQuery _recordReferenceQuery = builder.Create<RecordReferenceQuery>();

    public Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(IReadOnlyList<IModGetter> mods) {
        return Task.FromResult(new ImmutableRecordReferenceCache(
            mods.ToDictionary(x => x.ModKey,
                _ => new ModReferenceCache(new ConcurrentDictionary<FormKey, HashSet<IFormLinkIdentifier>>(), []))));
    }

    public Task<ImmutableRecordReferenceCache> GetImmutableRecordReferenceCache(ImmutableRecordReferenceCache immutableRecordReferenceCache) {
        return Task.FromResult(new ImmutableRecordReferenceCache(immutableRecordReferenceCache));
    }

    public Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(IReadOnlyList<IMod> mutableMods, IReadOnlyList<IModGetter> mods) {
        return Task.FromResult(new MutableRecordReferenceCache(mods
            .Concat(mutableMods)
            .DistinctBy(x => x.ModKey)
            .ToDictionary(
                x => x.ModKey,
                x => _recordReferenceQuery.BuildReferenceCache(x))));
    }

    public Task<MutableRecordReferenceCache> GetMutableRecordReferenceCache(
        IReadOnlyList<IMod> mutableMods,
        ImmutableRecordReferenceCache? immutableRecordReferenceCache = null) {
        return Task.FromResult(new MutableRecordReferenceCache(mutableMods
                .DistinctBy(x => x.ModKey)
                .ToDictionary(
                    x => x.ModKey,
                    x => _recordReferenceQuery.BuildReferenceCache(x)),
            immutableRecordReferenceCache));
    }
}
