using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class FactionFilter : SimpleRecordFilter<IFactionGetter> {
    public FactionFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Crime", record => (record.Flags & Faction.FactionFlag.TrackCrime) != 0),
        new("Vendor", record => (record.Flags & Faction.FactionFlag.Vendor) != 0),
    }) {}
}
