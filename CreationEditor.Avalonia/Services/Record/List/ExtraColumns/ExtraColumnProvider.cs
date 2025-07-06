using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnProvider : IExtraColumnProvider {
    public Dictionary<Type, List<IExtraColumns>> ExtraColumnsCache { get; }
    public IReadOnlyList<IConditionalExtraColumns> AutoAttachingExtraColumnsCache { get; }

    public ExtraColumnProvider(
        IEnumerable<IExtraColumns> extraColumns,
        IEnumerable<IConditionalExtraColumns> autoAttachingExtraColumns) {
        ExtraColumnsCache = extraColumns.GroupBy(x => x.Type).ToDictionary(e => e.Key, columns => columns.ToList());
        AutoAttachingExtraColumnsCache = autoAttachingExtraColumns.ToArray();
    }
}
