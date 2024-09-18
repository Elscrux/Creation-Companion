using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class ImageSpaceAdapterFilter : SimpleRecordFilter<IImageSpaceAdapterGetter> {
    public ImageSpaceAdapterFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Animatable", record => record.Animatable),
        new("Static", record => !record.Animatable),
    }) {}
}
