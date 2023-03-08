using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim.Extension;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class LocationFilter : RecordFilter<ILocationGetter> {
    private const char Separator = '\\';

    private readonly IEditorEnvironment _editorEnvironment;

    public LocationFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<ILocationGetter>()
            .GetRecursiveListings(Separator, GetParentLocationString);
    }

    private string GetParentLocationString(ILocationGetter location) {
        return string.Join(Separator,
            location
                .GetAllParentLocations(_editorEnvironment.LinkCache)
                .Reverse()
                .Select(l => l.EditorID));
    }
}
