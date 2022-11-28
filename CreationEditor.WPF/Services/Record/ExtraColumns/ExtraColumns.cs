namespace CreationEditor.WPF.Services.Record;

public abstract class ExtraColumns<TSetter, TGetter> : IExtraColumns {
    public Type Type { get; } = typeof(TGetter);
    public abstract IEnumerable<ExtraColumn> Columns { get; }
}
