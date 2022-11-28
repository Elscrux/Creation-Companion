namespace CreationEditor.WPF.Services.Record;

public interface IExtraColumns {
    public Type Type { get; }
    public IEnumerable<ExtraColumn> Columns { get; }
}