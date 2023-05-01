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

    private readonly IEditorEnvironment _editorEnvironment;

    public ConstructibleObjectFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        var finishedFormKeys = new HashSet<FormKey>();
        var byohListing = new RecordFilterListing(Byoh, record => record is IConstructibleObjectGetter constructible
         && constructible.WorkbenchKeyword.TryResolve(_editorEnvironment.LinkCache, out var keyword)
         && keyword.EditorID != null && keyword.EditorID.StartsWith(Byoh));

        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<IConstructibleObjectGetter>()
            .SelectWhere(constructible => {
                if (finishedFormKeys.Contains(constructible.WorkbenchKeyword.FormKey)) return TryGet<RecordFilterListing>.Failure;
                if (!constructible.WorkbenchKeyword.TryResolve(_editorEnvironment.LinkCache, out var keyword)) return TryGet<RecordFilterListing>.Failure;
                if (keyword.EditorID == null) return TryGet<RecordFilterListing>.Failure;

                finishedFormKeys.Add(keyword.FormKey);

                var listing = new RecordFilterListing(keyword.EditorID, record => record is IConstructibleObjectGetter c && c.WorkbenchKeyword.FormKey == keyword.FormKey);

                // BYOH specific filtering
                if (keyword.EditorID.StartsWith(Byoh)) {
                    listing.Parent = byohListing;
                    byohListing.RecordFilters.AddSorted(listing);
                    return TryGet<RecordFilterListing>.Succeed(byohListing);
                }

                return TryGet<RecordFilterListing>.Succeed(listing);
            })
            .Distinct();
    }
}
