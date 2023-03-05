using CreationEditor.Avalonia.Models.Record.Browser;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public interface IRecordFilter {
    public Type Type { get; }

    public IEnumerable<RecordFilterListing> GetListings(Type type);
}
