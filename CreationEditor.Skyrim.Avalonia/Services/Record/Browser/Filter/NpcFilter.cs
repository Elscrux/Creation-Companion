using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class NpcFilter : RecordFilter<INpcGetter> {
    private readonly ILinkCacheProvider _linkCacheProvider;

    public NpcFilter(
        ILinkCacheProvider linkCacheProvider) {
        _linkCacheProvider = linkCacheProvider;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<IRaceGetter>()
            .NotNull()
            .Where(race => race.EditorID is not null)
            .Select(race => {
                var listing = new RecordFilterListing(race.EditorID!, record => record is INpcGetter npc && npc.Race.FormKey == race.FormKey);

                foreach (var gender in Enum.GetValues<MaleFemaleGender>()) {
                    listing.RecordFilters.Add(
                        new RecordFilterListing(
                            gender.ToString(),
                            record => record is INpcGetter npc && npc.Race.FormKey == race.FormKey && npc.GetGender() == gender,
                            listing));
                }

                return listing;
            });
    }
}
