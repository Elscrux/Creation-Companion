using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services;

public interface ITierProvider {
    TierIdentifier? GetTier(IMajorRecordGetter record);
}
