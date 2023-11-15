using CreationEditor.Avalonia.Models.Record.Browser;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public sealed class RecordFilterBuilder : IRecordFilterBuilder {
    private readonly IRecordFilterProvider _provider;
    private readonly HashSet<RecordFilterListing> _recordFilters = [];

    public RecordFilterBuilder(
        IRecordFilterProvider provider) {
        _provider = provider;
    }

    public IRecordFilterBuilder AddRecordType(Type type) {
        _recordFilters.AddRange(type.AsEnumerable().Concat(type.GetInterfaces())
            .SelectWhere(@interface => _provider.RecordFilterCache.TryGetValue(@interface, out var recordFilter)
                ? TryGet<IEnumerable<RecordFilterListing>>.Succeed(recordFilter.GetListings(type))
                : TryGet<IEnumerable<RecordFilterListing>>.Failure)
            .SelectMany(c => c));

        return this;
    }

    public IRecordFilterBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter {
        return AddRecordType(typeof(TRecord));
    }

    public IEnumerable<RecordFilterListing> Build() {
        return _recordFilters;
    }
}
