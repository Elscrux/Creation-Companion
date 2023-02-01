using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnProvider : IExtraColumnProvider {
    public Dictionary<Type, IExtraColumns> ExtraColumnsCache { get; } = new();

    public ExtraColumnProvider() {
        typeof(IExtraColumns)
            .GetSubclassesOf()
            .NotNull()
            .Select(type => Activator.CreateInstance(type) as IExtraColumns)
            .NotNull()
            .ForEach(extraColumns => ExtraColumnsCache.Add(extraColumns.Type, extraColumns));
    }
}
