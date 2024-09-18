using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class WeatherFilter : SimpleRecordFilter<IWeatherGetter> {
    public WeatherFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Cloudy", record => (record.Flags & Weather.Flag.Cloudy) != 0),
        new("Pleasant", record => (record.Flags & Weather.Flag.Pleasant) != 0),
        new("Rainy", record => (record.Flags & Weather.Flag.Rainy) != 0),
        new("Snow", record => (record.Flags & Weather.Flag.Snow) != 0),
    }) {}
}
