namespace CreationEditor.Services.Integrations.BSA;

/// <summary>
/// Integration capability for creating BSA (Bethesda Softworks Archive) files. Concrete tools
/// that can build BSA archives implement this interface.
/// </summary>
public interface IBSAArchiveCreator : IIntegration {
    string CategoryName => "BSA Archive Creator";

    /// <summary>Creates a BSA archive from the given source folder into the target path.</summary>
    Task<IntegrationRunResult> CreateArchiveAsync(
        string sourceFolder,
        string targetArchivePath,
        object config,
        CancellationToken token);
}
