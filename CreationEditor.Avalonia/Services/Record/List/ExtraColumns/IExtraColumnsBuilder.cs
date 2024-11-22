using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnsBuilder {
    /// <summary>
    /// Adds all columns for the given record type and its interfaces.
    /// </summary>
    /// <param name="recordType">Type to add columns for</param>
    /// <returns>This instance for builder pattern</returns>
    IExtraColumnsBuilder AddRecordType(Type recordType);

    /// <summary>
    /// Adds all columns for the given record type and its interfaces.
    /// </summary>
    /// <typeparam name="TRecord">Type to add columns for</typeparam>
    /// <returns>This instance for builder pattern</returns>
    IExtraColumnsBuilder AddRecordType<TRecord>()
        where TRecord : IMajorRecordQueryableGetter;

    /// <summary>
    /// Adds all columns for the given column type.
    /// </summary>
    /// <param name="columnType">Type to add columns for, extending IUntypedExtraColumns</param>
    /// <returns>This instance for builder pattern</returns>
    IExtraColumnsBuilder AddColumnType(Type columnType);

    /// <summary>
    /// Adds all columns for the given column type.
    /// </summary>
    /// <typeparam name="TExtraColumns"></typeparam>
    /// <returns>This instance for builder pattern</returns>
    IExtraColumnsBuilder AddColumnType<TExtraColumns>()
        where TExtraColumns : IUntypedExtraColumns;

    /// <summary>
    /// Build the extra columns.
    /// </summary>
    /// <returns>Enumerable of extra columns</returns>
    IEnumerable<DataGridColumn> Build();
}
