namespace CreationEditor.WPF.Services.Record; 

public interface IExtraColumns {
    public Type Type { get; }
    public IEnumerable<ExtraColumn> Columns { get; }
}

public abstract class ExtraColumns<TSetter, TGetter> : IExtraColumns {
    public Type Type { get; } = typeof(TGetter);
    public abstract IEnumerable<ExtraColumn> Columns { get; }
}
