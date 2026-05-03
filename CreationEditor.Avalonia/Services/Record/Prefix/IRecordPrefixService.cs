namespace CreationEditor.Avalonia.Services.Record.Prefix;

public interface IRecordPrefixService {
    /// <summary>
    /// Prefix that is used for record EditorIDs.
    /// </summary>
    string Prefix { get; set; }

    /// <summary>
    /// An observable that emits the current prefix whenever it changes.
    /// </summary>
    IObservable<string> PrefixChanged { get; }

    /// <summary>
    /// Applies the current prefix to the given EditorID if it doesn't already have it.
    /// </summary>
    /// <param name="editorId">EditorID to apply the prefix to</param>
    /// <returns>Prefixed EditorID</returns>
    string ApplyPrefix(string editorId);
}
