using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnsBuilder {
    public IExtraColumnsBuilder AddRecordType(Type recordType);
    public IExtraColumnsBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter;

    public IExtraColumnsBuilder AddColumnType(Type columnType);
    public IExtraColumnsBuilder AddColumnType<TExtraColumns>()
        where TExtraColumns : IUntypedExtraColumns;

    public IEnumerable<DataGridColumn> Build();
}
