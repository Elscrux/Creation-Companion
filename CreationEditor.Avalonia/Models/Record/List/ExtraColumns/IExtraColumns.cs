namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public interface IExtraColumns {
    public Type Type { get; }
    public IEnumerable<ExtraColumn> Columns { get; }
}