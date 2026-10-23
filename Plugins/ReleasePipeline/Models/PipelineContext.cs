using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Models;

/// <summary>
/// Context handed to each <see cref="IPipelineAction"/> when a pipeline runs.
/// </summary>
public sealed record PipelineContext(
    ISkyrimModGetter Mod,
    IDataSource DataSource,
    IReadOnlyList<IDataSource> AdditionalDataSources,
    string ReleaseFolder) {
    public IFileSystem FileSystem => DataSource.FileSystem;

    /// <summary>
    /// Directory that actions read from and modify. Starts at the data source; the runner
    /// switches it to a temporary working copy of the data source before the first action with
    /// <see cref="IPipelineAction.RequiresInPlaceModification"/> runs, so in-place modifications
    /// like re-encoding or compression never alter the data source itself.
    /// </summary>
    public string WorkingDirectory { get; set; } = DataSource.Path;
}
