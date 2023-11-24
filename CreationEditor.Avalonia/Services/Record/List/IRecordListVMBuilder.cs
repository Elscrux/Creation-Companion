using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.List.ExtraColumns;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Record.List;

public interface IRecordListVMBuilder {
    /// <summary>
    /// Set the extra columns to use when creating record lists. If not set, no extra columns will be used.
    /// </summary>
    /// <param name="extraColumnsBuilder">Extra columns builder to use</param>
    /// <returns>This instance for builder pattern</returns>
    IRecordListVMBuilder WithExtraColumns(IExtraColumnsBuilder extraColumnsBuilder);

    /// <summary>
    /// Set the record provider factory to use when creating record lists. If not set, the default factory will be used.
    /// </summary>
    /// <param name="contextMenuProviderFactory">Context menu provider factory to use</param>
    /// <returns>This instance for builder pattern</returns>
    IRecordListVMBuilder WithContextMenuProviderFactory(Func<IObservable<IMajorRecordGetter?>, IRecordContextMenuProvider> contextMenuProviderFactory);

    /// <summary>
    /// Set the default record browser settings to use when creating record lists. If not set, the default settings will be used.
    /// </summary>
    /// <param name="browserSettings">Record browser settings to use</param>
    /// <returns>This instance for builder pattern</returns>
    IRecordListVMBuilder WithBrowserSettings(IRecordBrowserSettings browserSettings);

    /// <summary>
    /// Create a record list for a list of form links.
    /// </summary>
    /// <param name="identifiers">Enumerable of form links</param>
    /// <returns>Record list with the form links</returns>
    IRecordListVM BuildWithSource(IEnumerable<IFormLinkIdentifier> identifiers);

    /// <summary>
    /// Create a record list for all records of the given type.
    /// </summary>
    /// <param name="type">Type to create a list for</param>
    /// <returns>Record list with all record of the type</returns>
    IRecordListVM BuildWithSource(Type type);

    /// <summary>
    /// Create a record list for a specific record provider.
    /// </summary>
    /// <param name="recordProvider">Record provider to create a list for</param>
    /// <returns>Record list with the record provider</returns>
    IRecordListVM BuildWithSource(IRecordProvider recordProvider);
}
