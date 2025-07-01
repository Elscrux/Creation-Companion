using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services.LeveledList;

public interface ITierController {
    IReadOnlyDictionary<FormKey, Tier> GetAllRecordTiers();
    TierIdentifier? GetRecordTier(IMajorRecordGetter record);
    void SetRecordTier(IMajorRecordGetter record, TierIdentifier tier);
    void RemoveRecordTier(IMajorRecordGetter record);

    IEnumerable<TierIdentifier> GetTiers<TMajorRecordGetter>();
    IEnumerable<TierIdentifier> GetTiers(Type recordType);
    void SetTiers(Type recordType, IEnumerable<TierIdentifier> tiers);
}
