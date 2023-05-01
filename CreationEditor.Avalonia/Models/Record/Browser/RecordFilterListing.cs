using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public sealed record RecordFilterListing(
    string DisplayName,
    Func<IMajorRecordGetter, bool> FilterInternal,
    IRecordFilterContainer? Parent = null) : IRecordFilterContainer, IComparable, IComparable<RecordFilterListing> {
    public IRecordFilterContainer? Parent { get; set; } = Parent;
    public IObservableCollection<RecordFilterListing> RecordFilters { get; } = new ObservableCollectionExtended<RecordFilterListing>();

    public int CompareTo(object? obj) {
        return obj is RecordFilterListing listing ? CompareTo(listing) : 0;
    }

    public int CompareTo(RecordFilterListing? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        var noFilters = RecordFilters.Count == 0;
        var otherNoFilters = other.RecordFilters.Count == 0;

        if (noFilters != otherNoFilters) return noFilters ? 1 : -1;

        return string.Compare(DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase);
    }

    public bool Filter(IMajorRecordGetter record) => FilterInternal(record);
}
