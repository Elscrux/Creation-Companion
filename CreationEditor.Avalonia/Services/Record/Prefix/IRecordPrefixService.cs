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
}
