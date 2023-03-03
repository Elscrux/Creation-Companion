using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnProvider : IExtraColumnProvider {
    public Dictionary<Type, IExtraColumns> ExtraColumnsCache { get; }

    public ExtraColumnProvider() {
        ExtraColumnsCache = typeof(IExtraColumns)
            .GetSubclassesOf()
            .NotNull()
            .Select(type => Activator.CreateInstance(type) as IExtraColumns)
            .NotNull()
            .ToDictionary(extraColumns => extraColumns.Type, extraColumns => extraColumns);
    }
}
