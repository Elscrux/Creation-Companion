using System.Collections.Concurrent;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public sealed class ExtraColumnsBuilder(IExtraColumnProvider provider) : IExtraColumnsBuilder {
    private readonly ConcurrentDictionary<IUntypedExtraColumns, byte> _extraColumns = [];

    public IExtraColumnsBuilder AddRecordType(Type recordType) {
        foreach (var @interface in recordType.AsEnumerable().Concat(recordType.GetInterfaces())) {
            if (!provider.ExtraColumnsCache.TryGetValue(@interface, out var extraColumns)) continue;

            foreach (var extraColumn in extraColumns) {
                _extraColumns.TryAdd(extraColumn, 0);
            }
        }

        foreach (var extraColumn in provider.AutoAttachingExtraColumnsCache
            .Where(c => c.CanAttachTo(recordType))) {
            _extraColumns.TryAdd(extraColumn, 0);
        }

        return this;
    }

    public IExtraColumnsBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter {
        return AddRecordType(typeof(TRecord));
    }

    public IExtraColumnsBuilder AddColumnType(Type columnType) {
        if (Activator.CreateInstance(columnType) is IUntypedExtraColumns extraColumns) {
            _extraColumns.TryAdd(extraColumns, 0);
        }

        return this;
    }

    public IExtraColumnsBuilder AddColumnType<TExtraColumns>()
        where TExtraColumns : IUntypedExtraColumns {
        return AddColumnType(typeof(TExtraColumns));
    }

    public IEnumerable<DataGridColumn> Build() {
        var finalColumns = _extraColumns
            .Select(x => x.Key)
            .SelectMany(c => Dispatcher.UIThread.Invoke(() => c.CreateColumns().ToArray()))
            .OrderByDescending(c => c.Priority)
            .Select(c => c.Column)
            .ToList();

        _extraColumns.Clear();

        return finalColumns;
    }
}
