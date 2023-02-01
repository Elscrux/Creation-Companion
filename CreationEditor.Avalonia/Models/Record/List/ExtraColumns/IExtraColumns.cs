namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public interface IUntypedExtraColumns {
    public IEnumerable<ExtraColumn> Columns { get; }
}

public interface IExtraColumns : IUntypedExtraColumns {
    public Type Type { get; }
}