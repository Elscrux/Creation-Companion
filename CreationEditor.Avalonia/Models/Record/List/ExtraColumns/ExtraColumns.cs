namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public abstract class ExtraColumns<TType> : IExtraColumns {
    public Type Type { get; } = typeof(TType);
    public abstract IEnumerable<ExtraColumn> CreateColumns();
}
