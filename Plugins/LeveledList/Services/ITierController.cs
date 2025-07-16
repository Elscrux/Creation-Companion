using LeveledList.Model;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services;

public interface ITierController {
    IReadOnlyDictionary<IFormLinkGetter, Tier> GetAllRecordTiers();
    TierIdentifier? GetRecordTier(IMajorRecordGetter record);
    void SetRecordTier(IMajorRecordGetter record, TierIdentifier tier);
    void RemoveRecordTier(IMajorRecordGetter record);

    Dictionary<TierIdentifier, TierData> GetTiers(ListRecordType type);
    TierData? GetTierData(ListRecordType type, TierIdentifier tier);
    void SetTiers(ListRecordType type, Dictionary<TierIdentifier, TierData> tiers);
}
