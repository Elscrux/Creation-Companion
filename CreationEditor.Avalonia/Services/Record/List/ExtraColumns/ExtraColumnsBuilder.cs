using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnsBuilder(IExtraColumnProvider provider) : IExtraColumnsBuilder {
    private readonly HashSet<ExtraColumn> _extraColumns = [];

    public IExtraColumnsBuilder AddRecordType(Type recordType) {
        _extraColumns.AddRange(recordType.AsEnumerable().Concat(recordType.GetInterfaces())
            .SelectWhere(@interface => provider.ExtraColumnsCache.TryGetValue(@interface, out var extraColumn)
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
