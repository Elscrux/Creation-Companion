namespace CreationEditor.Services.Asset;

public interface IDeleteDirectoryProvider {
    /// <summary>
    /// Directory where deleted assets are moved to
    /// </summary>
    string DeleteDirectory { get; }
}
