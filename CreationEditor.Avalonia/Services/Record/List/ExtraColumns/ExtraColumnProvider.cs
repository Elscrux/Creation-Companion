using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnProvider : IExtraColumnProvider {
    public IReadOnlyDictionary<Type, IExtraColumns> ExtraColumnsCache { get; }
    public IReadOnlyList<IConditionalExtraColumns> AutoAttachingExtraColumnsCache { get; }

    public ExtraColumnProvider(
        IEnumerable<IExtraColumns> extraColumns,
        IEnumerable<IConditionalExtraColumns> autoAttachingExtraColumns) {
        ExtraColumnsCache = extraColumns.ToDictionary(e => e.Type, columns => columns);
        AutoAttachingExtraColumnsCache = autoAttachingExtraColumns.ToArray();
    }
}
