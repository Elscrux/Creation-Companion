using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public class ExtraColumnProvider : IExtraColumnProvider {
    private readonly Dictionary<Type, IExtraColumns> _extraColumnsCache = new();

    public ExtraColumnProvider() {
        typeof(IExtraColumns)
            .GetSubclassesOf()
            .NotNull()
            .Select(type => Activator.CreateInstance(type) as IExtraColumns)
            .NotNull()
            .ForEach(extraColumns => _extraColumnsCache.Add(extraColumns.Type, extraColumns));
    }

    public List<DataGridColumn> GetColumns(Type type) {
        return type.GetInterfaces()
            .SelectWhere(@interface => _extraColumnsCache.TryGetValue(@interface, out var extraColumn)
                ? TryGet<IEnumerable<ExtraColumn>>.Succeed(extraColumn.Columns) 
                : TryGet<IEnumerable<ExtraColumn>>.Failure)
            .SelectMany(c => c)
            .OrderByDescending(c => c.Priority)
            .Select(c => c.Column)
            .ToList();
    }
}
