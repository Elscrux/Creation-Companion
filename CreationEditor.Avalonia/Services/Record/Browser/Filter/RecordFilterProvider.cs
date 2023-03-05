using Autofac;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public class RecordFilterProvider : IRecordFilterProvider {
    public Dictionary<Type, IRecordFilter> RecordFilterCache { get; }

    public RecordFilterProvider(
        IComponentContext componentContext) {
        RecordFilterCache = typeof(IRecordFilter)
            .GetSubclassesOf()
            .NotNull()
            .Select(componentContext.Resolve)
            .OfType<IRecordFilter>()
            .NotNull()
            .ToDictionary(subRecordListing => subRecordListing.Type, subRecordListing => subRecordListing);
    }
}
