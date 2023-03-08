using Avalonia.Controls;
using AvaloniaEdit.Utils;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnsBuilder : IExtraColumnsBuilder {
    private readonly IExtraColumnProvider _provider;
    private readonly HashSet<ExtraColumn> _extraColumns = new();

    public ExtraColumnsBuilder(
        IExtraColumnProvider provider) {
        _provider = provider;
    }

    public IExtraColumnsBuilder AddRecordType(Type type) {
        _extraColumns.AddRange(type.AsEnumerable().Concat(type.GetInterfaces())
            .SelectWhere(@interface => _provider.ExtraColumnsCache.TryGetValue(@interface, out var extraColumn)
                ? TryGet<IEnumerable<ExtraColumn>>.Succeed(extraColumn.Columns)
                : TryGet<IEnumerable<ExtraColumn>>.Failure)
            .SelectMany(c => c));

        return this;
    }

    public IExtraColumnsBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter {
        return AddRecordType(typeof(TRecord));
    }

    public IExtraColumnsBuilder AddColumnType(Type columnType) {
        if (Activator.CreateInstance(columnType) is IUntypedExtraColumns extraColumns) {
            _extraColumns.AddRange(extraColumns.Columns);
        }

        return this;
    }

    public IExtraColumnsBuilder AddColumnType<TExtraColumns>()
        where TExtraColumns : IUntypedExtraColumns {
        return AddColumnType(typeof(TExtraColumns));
    }

    public IEnumerable<DataGridColumn> Build() {
        return _extraColumns
            .OrderByDescending(c => c.Priority)
            .Select(c => c.Column);
    }
}
