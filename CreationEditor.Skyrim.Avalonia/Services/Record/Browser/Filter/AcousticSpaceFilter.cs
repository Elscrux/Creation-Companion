using System;
using System.Collections.Generic;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class AcousticSpaceFilter : RecordFilter<IAcousticSpaceGetter> {
    private readonly ILinkCacheProvider _linkCacheProvider;

    public AcousticSpaceFilter(
        ILinkCacheProvider linkCacheProvider) {
        _linkCacheProvider = linkCacheProvider;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        var finishedFormKeys = new HashSet<FormKey>();

        return _linkCacheProvider.LinkCache.PriorityOrder.WinningOverrides<IAcousticSpaceGetter>()
            .SelectWhere(acousticSpace => {
                if (finishedFormKeys.Contains(acousticSpace.EnvironmentType.FormKey)
                 || !acousticSpace.EnvironmentType.TryResolve(_linkCacheProvider.LinkCache, out var reverbParameters)
                 || reverbParameters.EditorID is null) return TryGet<RecordFilterListing>.Failure;

                finishedFormKeys.Add(reverbParameters.FormKey);

                return TryGet<RecordFilterListing>.Succeed(new RecordFilterListing(
                    reverbParameters.EditorID,
                    record =>
                        record is IAcousticSpaceGetter a
                     && a.EnvironmentType.FormKey == reverbParameters.FormKey));
            });
    }
}
