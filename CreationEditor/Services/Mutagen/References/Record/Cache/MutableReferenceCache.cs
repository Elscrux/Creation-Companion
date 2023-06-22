using CreationEditor.Services.Mutagen.References.Record.Query;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Record.Cache;

public sealed class MutableReferenceCache : IReferenceCache {
    private readonly IModGetter _mutableMod;
    private readonly ImmutableReferenceCache? _immutableReferenceCache;
    private readonly ReferenceQuery.ModReferenceCache _mutableModReferenceCache;

    public MutableReferenceCache(IReferenceQuery referenceQuery, IModGetter mutableMod, IReadOnlyList<IModGetter> mods) {
        _mutableMod = mutableMod;
        referenceQuery.LoadModReferences(mutableMod);

        _mutableModReferenceCache = referenceQuery.ModCaches[mutableMod.ModKey];
        _immutableReferenceCache = new ImmutableReferenceCache(referenceQuery, mods);
    }

    public MutableReferenceCache(IReferenceQuery referenceQuery, IModGetter mutableMod, ImmutableReferenceCache? immutableReferenceCache = null) {
        _mutableMod = mutableMod;
        referenceQuery.LoadModReferences(mutableMod);
        _mutableModReferenceCache = referenceQuery.ModCaches[mutableMod.ModKey];

        if (immutableReferenceCache is not null) _immutableReferenceCache = new ImmutableReferenceCache(immutableReferenceCache);
    }

    public bool AddRecord(IMajorRecordGetter record) {
        return _mutableModReferenceCache.FormKeys.Add(record.FormKey);
    }

    public bool RemoveReference(FormKey formKey, IFormLinkIdentifier oldReference) {
        // when the record was not part of the MUTABLE MOD before we need to reevaluate all old form from the other one too  

        return _mutableModReferenceCache.Cache.TryGetValue(formKey, out var references)
         && references.Remove(oldReference);
    }

    public void RemoveReferences(FormKey formKey, IEnumerable<IFormLinkIdentifier> oldReferences) {
        if (!_mutableModReferenceCache.Cache.TryGetValue(formKey, out var references)) return;

        foreach (var oldReference in oldReferences) {
            references.Remove(oldReference);
        }
    }

    public bool AddReference(FormKey formKey, IFormLinkIdentifier newReference) {
        var references = _mutableModReferenceCache.Cache.GetOrAdd(formKey);

        return references.Add(newReference);
    }

    public void AddReferences(FormKey formKey, IEnumerable<IFormLinkIdentifier> newReferences) {
        var references = _mutableModReferenceCache.Cache.GetOrAdd(formKey);
        foreach (var newReference in newReferences) {
            references.Add(newReference);
        }
    }

    public IEnumerable<IFormLinkIdentifier> GetReferences(FormKey formKey, IReadOnlyList<IModGetter> modOrder) {
        if (_mutableModReferenceCache.Cache.TryGetValue(formKey, out var references)) {
            foreach (var reference in references) {
                yield return reference;
            }
        } else if (_immutableReferenceCache is not null) {
            foreach (var reference in _immutableReferenceCache.GetReferences(formKey, modOrder)) {
                yield return reference;
            }
        }
    }
}
