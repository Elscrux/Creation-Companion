using LeveledList.Model.Tier;
using Mutagen.Bethesda.Plugins.Records;
namespace LeveledList.Services.Record;

public interface IRecordTypeProvider {
    IEnumerable<RecordWithTier> GetRecords(IModGetter mod, string type);
}
