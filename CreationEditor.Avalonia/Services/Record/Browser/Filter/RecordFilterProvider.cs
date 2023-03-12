using Autofac;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public class RecordFilterProvider : IRecordFilterProvider {
    public Dictionary<Type, IRecordFilter> RecordFilterCache { get; }

    public RecordFilterProvider(
        IComponentContext componentContext) {
        RecordFilterCache = typeof(IRecordFilter)
            .GetSubclassesOf()
            .Select(componentContext.Resolve)
            .OfType<IRecordFilter>()
            .ToDictionary(subRecordListing => subRecordListing.Type, subRecordListing => subRecordListing);
    }
}
