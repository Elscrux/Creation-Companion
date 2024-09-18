using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class GlobalFilter : SimpleRecordFilter<IGlobalGetter> {
    public GlobalFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Short", record => record.TypeChar == 's'),
        new("Long", record => record.TypeChar == 'l'),
        new("Float", record => record.TypeChar == 'f'),
    }) {}
}
