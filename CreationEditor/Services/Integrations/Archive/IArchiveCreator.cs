using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Integrations.Archive;

/// <summary>
/// Integration capability for compressing loose files into archive files.
/// Concrete tools that can build archives implement this interface.
/// </summary>
public interface IArchiveCreator : IIntegration {
    /// <summary>Creates an archive from the given source folder into the target path</summary>
    Task<IntegrationRunResult> CreateArchive(
        IDataSource dataSource,
        string targetArchivePath,
        ConfigValues config,
        CancellationToken token);
}
