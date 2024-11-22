using CreationEditor.Avalonia.Models.Record.Browser;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public interface IRecordFilterBuilder {
    IRecordFilterBuilder AddRecordType(Type type);
    IRecordFilterBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter;

    IEnumerable<RecordFilterListing> Build();
}
