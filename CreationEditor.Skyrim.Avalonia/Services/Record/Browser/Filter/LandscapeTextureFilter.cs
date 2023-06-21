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

public sealed class LandscapeTextureFilter : RecordFilter<ILandscapeTextureGetter> {
    private readonly IEditorEnvironment _editorEnvironment;

    public LandscapeTextureFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        var finishedFormKeys = new HashSet<FormKey>();

        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<ILandscapeTextureGetter>()
            .SelectWhere(landscapeTexture => {
                if (finishedFormKeys.Contains(landscapeTexture.MaterialType.FormKey)) return TryGet<RecordFilterListing>.Failure;
                if (!landscapeTexture.MaterialType.TryResolve(_editorEnvironment.LinkCache, out var materialType)) return TryGet<RecordFilterListing>.Failure;
                if (materialType.EditorID is null) return TryGet<RecordFilterListing>.Failure;

                finishedFormKeys.Add(materialType.FormKey);

                return TryGet<RecordFilterListing>.Succeed(new RecordFilterListing(materialType.EditorID, record => record is ILandscapeTextureGetter l && l.MaterialType.FormKey == materialType.FormKey));
            });
    }
}
