using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class QuestFilter(ILinkCacheProvider linkCacheProvider) : RecordFilter<IQuestGetter> {
    private const char QuestFilterSeparator = '\\';

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<IQuestGetter>()
            .GetRecursiveListings(quest => quest.Filter, [QuestFilterSeparator]);
    }
}
