using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Integrations.Lip;

public interface ILipFuzerRuntimeConfigDefinition : IConfigDefinition {
    static abstract TextConfigOption AudioExtensionOption { get; }
}

/// <summary>
/// Integration capability for packaging fuz files from lip and audio files. Concrete tools that
/// can fuze lips files implement this interface.
/// </summary>
public interface ILipFuzer : IIntegration {
    public string LipFileExtension { get; }
    public string FuzeFileExtension { get; }

    /// <summary>Packages fuz files from the lip and audio files in the given directory</summary>
    Task<IntegrationRunResult> PackageFuz(
        string dir,
        ConfigValues config,
        CancellationToken token);

    Task<IntegrationRunResult> PackageFuz(
        DataSourceFileLink file,
        ConfigValues config,
        CancellationToken token);

    /// <summary>Extracts lip and audio files from fuz files in the given directory</summary>
    Task<IntegrationRunResult> ExtractFuz(
        string dir,
        ConfigValues config,
        CancellationToken token);

    Task<IntegrationRunResult> ExtractFuz(
        DataSourceFileLink file,
        ConfigValues config,
        CancellationToken token);

}
