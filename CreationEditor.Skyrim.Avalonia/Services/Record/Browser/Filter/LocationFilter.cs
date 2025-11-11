using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class LocationFilter(ILinkCacheProvider linkCacheProvider) : RecordFilter<ILocationGetter> {
    private const char Separator = '\\';

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<ILocationGetter>()
            .GetRecursiveListings(GetParentLocationString, [Separator]);
    }

    private string GetParentLocationString(ILocationGetter location) {
        return string.Join(Separator,
            location
                .GetAllParentLocations(linkCacheProvider.LinkCache)
                .Reverse()
                .Select(l => l.EditorID));
    }
}
