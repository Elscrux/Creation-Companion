using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnsBuilder(IExtraColumnProvider provider) : IExtraColumnsBuilder {
    private readonly HashSet<IUntypedExtraColumns> _extraColumns = [];

    public IExtraColumnsBuilder AddRecordType(Type recordType) {
        _extraColumns.AddRange(recordType.AsEnumerable().Concat(recordType.GetInterfaces())
            .SelectWhere(@interface => provider.ExtraColumnsCache.TryGetValue(@interface, out var extraColumns)
                ? TryGet<IEnumerable<IUntypedExtraColumns>>.Succeed(extraColumns)
                : TryGet<IEnumerable<IUntypedExtraColumns>>.Failure)
            .SelectMany(c => c));

        _extraColumns.AddRange(
            provider.AutoAttachingExtraColumnsCache
                .Where(c => c.CanAttachTo(recordType)));

        return this;
    }

    public IExtraColumnsBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter {
        return AddRecordType(typeof(TRecord));
    }

    public IExtraColumnsBuilder AddColumnType(Type columnType) {
        if (Activator.CreateInstance(columnType) is IUntypedExtraColumns extraColumns) {
            _extraColumns.Add(extraColumns);
        }

        return this;
    }

    public IExtraColumnsBuilder AddColumnType<TExtraColumns>()
        where TExtraColumns : IUntypedExtraColumns {
        return AddColumnType(typeof(TExtraColumns));
    }

    public IEnumerable<DataGridColumn> Build() {
        var finalColumns = _extraColumns
            .SelectMany(c => Dispatcher.UIThread.Invoke(() => c.CreateColumns().ToArray()))
            .OrderByDescending(c => c.Priority)
            .Select(c => c.Column)
            .ToList();

        _extraColumns.Clear();

        return finalColumns;
    }
}
