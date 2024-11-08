using System.Reactive;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Filter;

public interface IRecordBrowserSettings {
    /// <summary>
    /// Search term to filter records by editor id.
    /// </summary>
    string SearchTerm { get; set; }

    /// <summary>
    /// Search filter executor
    /// </summary>
    ISearchFilter SearchFilter { get; }

    /// <summary>
    /// Scope of the mods to search in
    /// </summary>
    IModScopeProvider ModScopeProvider { get; }

    /// <summary>
    /// Custom filter to apply to additionally filter records with
    /// </summary>
    Func<IMajorRecordGetter, bool>? CustomFilter { get; set; }

    /// <summary>
    /// Observable that fires when any setting changes
    /// </summary>
    IObservable<Unit> SettingsChanged { get; }

    /// <summary>
    /// Filter a record by the current settings
    /// </summary>
    /// <param name="record">Record to filter</param>
    /// <returns>true if the record passed the filter, false if it was filtered out</returns>
    bool Filter(IMajorRecordGetter record);

    /// <summary>
    /// Filter a record identifier by the current settings
    /// </summary>
    /// <param name="record">Record identifier to filter</param>
    /// <returns>true if the record passed the filter, false if it was filtered out</returns>
    bool Filter(IMajorRecordIdentifierGetter
        record);
}
