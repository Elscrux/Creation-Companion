namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public sealed class RecordFilterProvider : IRecordFilterProvider {
    public IReadOnlyDictionary<Type, IRecordFilter> RecordFilterCache { get; }

    public RecordFilterProvider(
        IEnumerable<IRecordFilter> recordFilters) {
        RecordFilterCache = recordFilters
            .ToDictionary(subRecordListing => subRecordListing.Type, subRecordListing => subRecordListing);
    }
}
