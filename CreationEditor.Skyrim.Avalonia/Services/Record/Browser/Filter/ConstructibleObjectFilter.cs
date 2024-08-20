using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class ConstructibleObjectFilter : RecordFilter<IConstructibleObjectGetter> {
    private const string Byoh = "BYOH";

    private readonly ILinkCacheProvider _linkCacheProvider;

    public ConstructibleObjectFilter(
        ILinkCacheProvider linkCacheProvider) {
        _linkCacheProvider = linkCacheProvider;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        var finishedFormKeys = new HashSet<FormKey>();
        var byohListing = new RecordFilterListing(
            Byoh,
            record => record is IConstructibleObjectGetter constructible
             && constructible.WorkbenchKeyword.TryResolve(_linkCacheProvider.LinkCache, out var keyword)
             && keyword.EditorID is not null && keyword.EditorID.StartsWith(Byoh, StringComparison.OrdinalIgnoreCase));

        return _linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<IConstructibleObjectGetter>()
            .SelectWhere(constructible => {
                if (finishedFormKeys.Contains(constructible.WorkbenchKeyword.FormKey)
                 || !constructible.WorkbenchKeyword.TryResolve(_linkCacheProvider.LinkCache, out var keyword)
                 || keyword.EditorID is null) return TryGet<RecordFilterListing>.Failure;

                finishedFormKeys.Add(keyword.FormKey);

                var listing = new RecordFilterListing(
                    keyword.EditorID,
                    record =>
                        record is IConstructibleObjectGetter c
                     && c.WorkbenchKeyword.FormKey == keyword.FormKey);

                // BYOH specific filtering
                if (keyword.EditorID.StartsWith(Byoh, StringComparison.OrdinalIgnoreCase)) {
                    listing.Parent = byohListing;
                    byohListing.RecordFilters.AddSorted(listing);
                    return TryGet<RecordFilterListing>.Succeed(byohListing);
                }

                return TryGet<RecordFilterListing>.Succeed(listing);
            })
            .Distinct();
    }
}
