using CreationEditor.Avalonia.Models.Record.Browser;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public abstract class RecordFilter<T> : IRecordFilter {
    public Type Type => typeof(T);

    public abstract IEnumerable<RecordFilterListing> GetListings(Type type);
}
