using LeveledList.Model;
using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services.Record;

public interface ILeveledListRecordTypeProvider {
    ListRecordType? GetListRecordType(IMajorRecordGetter record);
    IEnumerable<RecordWithTier> GetRecords(IEnumerable<IModGetter> mods, ListRecordType type);
}
