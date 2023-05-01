using CreationEditor.Avalonia.Models.Record.Browser;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public abstract class SimpleRecordFilter<T> : IRecordFilter {
    public Type Type => typeof(T);

    private readonly List<RecordFilterListing> _filters;

    protected SimpleRecordFilter(IEnumerable<SimpleRecordFilterEntry> entries) {
        _filters = entries
            .Select(entry => new RecordFilterListing(entry.Name, record => record is T t && entry.Filter(t)))
            .ToList();
    }

    public IEnumerable<RecordFilterListing> GetListings(Type type) => _filters;

    protected sealed record SimpleRecordFilterEntry(string Name, Func<T, bool> Filter);
}
