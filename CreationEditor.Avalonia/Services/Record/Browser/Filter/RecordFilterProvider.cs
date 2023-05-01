using Autofac;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public sealed class RecordFilterProvider : IRecordFilterProvider {
    public Dictionary<Type, IRecordFilter> RecordFilterCache { get; }

    public RecordFilterProvider(
        IComponentContext componentContext) {
        RecordFilterCache = typeof(IRecordFilter)
            .GetAllSubClass<IRecordFilter>(componentContext.Resolve)
            .ToDictionary(subRecordListing => subRecordListing.Type, subRecordListing => subRecordListing);
    }
}
