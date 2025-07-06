using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Model.Tier;

public sealed record RecordWithTier(
    IMajorRecordGetter Record,
    TierIdentifier Tier
);
