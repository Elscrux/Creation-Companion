using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.State;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace LeveledList.Services;

public interface ITierController {
    TierIdentifier? GetTier(IMajorRecordGetter record);

    IEnumerable<TierIdentifier> GetTiers<TMajorRecordGetter>();
    IEnumerable<TierIdentifier> GetTiers(Type recordType);
    void SetTier(IMajorRecordGetter record, TierIdentifier tier);
}

public sealed record TierDefinitions(List<TierIdentifier> Tiers);

public sealed class TierController(
    IRecordDecorationController recordDecorationController,
    ICachedStateRepository<TierDefinitions, string> stateRepository) : ITierController {
    private readonly Dictionary<Type, IReadOnlyList<TierIdentifier>> _tiers = [];

    public TierIdentifier? GetTier(IMajorRecordGetter record) {
        var tier = recordDecorationController.Get<Tier>(record);
        return tier?.TierIdentifier;
    }

    public IEnumerable<TierIdentifier> GetTiers<TMajorRecordGetter>() => GetTiers(typeof(TMajorRecordGetter));
    public IEnumerable<TierIdentifier> GetTiers(Type recordType) {
        var recordTypeName = recordType.Name;
        if (_tiers.TryGetValue()) {
            var tierDefinitions = stateRepository.Load(recordTypeName);
        }
        if (recordType.IsAssignableFrom(typeof(IArmorGetter))) {
            return [
                "light/fur",
                "light/hide",
                "light/leather",
                "heavy/iron",
                "heavy/steel",
            ];
        }

        if (recordType.IsAssignableFrom(typeof(IWeaponGetter))) {
            
        }

        return [];
    }

    public void SetTier(IMajorRecordGetter record, TierIdentifier tier) {
        recordDecorationController.Update(record, tier);
    }
    
    
}
