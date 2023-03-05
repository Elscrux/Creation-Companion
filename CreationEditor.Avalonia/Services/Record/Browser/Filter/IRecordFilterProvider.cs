namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public interface IRecordFilterProvider {
    public Dictionary<Type, IRecordFilter> RecordFilterCache { get; }
}
