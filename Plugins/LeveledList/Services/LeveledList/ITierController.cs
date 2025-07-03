using LeveledList.Model;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services.LeveledList;

public interface ITierController {
    IReadOnlyDictionary<IFormLinkGetter, Tier> GetAllRecordTiers();
    TierIdentifier? GetRecordTier(IMajorRecordGetter record);
    void SetRecordTier(IMajorRecordGetter record, TierIdentifier tier);
    void RemoveRecordTier(IMajorRecordGetter record);

    IEnumerable<TierIdentifier> GetTiers(ListRecordType type);
    void SetTiers(ListRecordType type, IEnumerable<TierIdentifier> tiers);
}
