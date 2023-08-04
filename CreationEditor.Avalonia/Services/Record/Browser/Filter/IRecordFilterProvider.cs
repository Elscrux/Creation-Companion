namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public interface IRecordFilterProvider {
    /// <summary>
    /// Record filter per type
    /// </summary>
    IReadOnlyDictionary<Type, IRecordFilter> RecordFilterCache { get; }
}
