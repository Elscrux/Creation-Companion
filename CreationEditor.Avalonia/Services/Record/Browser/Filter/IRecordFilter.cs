using CreationEditor.Avalonia.Models.Record.Browser;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public interface IRecordFilter {
    Type Type { get; }

    IEnumerable<RecordFilterListing> GetListings(Type type);
}
