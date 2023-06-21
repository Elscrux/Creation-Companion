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
    private readonly IEditorEnvironment _editorEnvironment;

    public AcousticSpaceFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        var finishedFormKeys = new HashSet<FormKey>();

        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<IAcousticSpaceGetter>()
            .SelectWhere(acousticSpace => {
                if (finishedFormKeys.Contains(acousticSpace.EnvironmentType.FormKey)) return TryGet<RecordFilterListing>.Failure;
                if (!acousticSpace.EnvironmentType.TryResolve(_editorEnvironment.LinkCache, out var reverbParameters)) return TryGet<RecordFilterListing>.Failure;
                if (reverbParameters.EditorID is null) return TryGet<RecordFilterListing>.Failure;

                finishedFormKeys.Add(reverbParameters.FormKey);

                return TryGet<RecordFilterListing>.Succeed(new RecordFilterListing(reverbParameters.EditorID, record => record is IAcousticSpaceGetter a && a.EnvironmentType.FormKey == reverbParameters.FormKey));
            });
    }
}
