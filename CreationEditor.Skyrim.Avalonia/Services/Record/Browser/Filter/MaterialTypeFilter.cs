using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class MaterialTypeFilter : SimpleRecordFilter<IMaterialTypeGetter> {
    public MaterialTypeFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Arrows Stick", record => (record.Flags & MaterialType.Flag.ArrowsStick) != 0),
        new("Stair Material", record => (record.Flags & MaterialType.Flag.StairMaterial) != 0),
    }) {}
}
