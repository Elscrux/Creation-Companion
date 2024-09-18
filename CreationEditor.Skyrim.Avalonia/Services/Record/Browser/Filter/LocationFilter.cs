using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class LocationFilter : RecordFilter<ILocationGetter> {
    private const char Separator = '\\';

    private readonly ILinkCacheProvider _linkCacheProvider;

    public LocationFilter(
        ILinkCacheProvider linkCacheProvider) {
        _linkCacheProvider = linkCacheProvider;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<ILocationGetter>()
            .GetRecursiveListings(GetParentLocationString, Separator);
    }

    private string GetParentLocationString(ILocationGetter location) {
        return string.Join(Separator,
            location
                .GetAllParentLocations(_linkCacheProvider.LinkCache)
                .Reverse()
                .Select(l => l.EditorID));
    }
}
