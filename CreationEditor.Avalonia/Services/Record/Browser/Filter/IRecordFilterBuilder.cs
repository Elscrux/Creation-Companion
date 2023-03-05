using CreationEditor.Avalonia.Models.Record.Browser;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public interface IRecordFilterBuilder {
    public IRecordFilterBuilder AddRecordType(Type type);
    public IRecordFilterBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter;

    public IEnumerable<RecordFilterListing> Build();
}
