using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class IdleMarkerFilter : SimpleRecordFilter<IIdleMarkerGetter> {
    public IdleMarkerFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Ignored by Sandbox", record => (record.Flags & IdleMarker.Flag.IgnoredBySandbox) != 0),
        new("Do Once", record => (record.Flags & IdleMarker.Flag.DoOnce) != 0),
        new("Child Can Use", record => (record.MajorFlags & IdleMarker.MajorFlag.ChildCanUse) != 0),
    }) {}
}
