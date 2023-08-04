using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnProvider : IExtraColumnProvider {
    public IReadOnlyDictionary<Type, IExtraColumns> ExtraColumnsCache { get; }

    public ExtraColumnProvider() {
        ExtraColumnsCache = typeof(IExtraColumns)
            .GetAllSubClasses<IExtraColumns>()
            .ToDictionary(extraColumns => extraColumns.Type, extraColumns => extraColumns);
    }
}
