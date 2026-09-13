using System.Reactive;
using ReleasePipeline.Models;
namespace ReleasePipeline.Services;

/// <summary>
/// Owns the globally-persisted set of release <see cref="Pipeline"/> definitions
/// and notifies about changes so the UI stays in sync.
/// </summary>
public interface IPipelineService {
    IReadOnlyList<Pipeline> Pipelines { get; }

    IObservable<Unit> PipelinesChanged { get; }

    Pipeline? GetPipeline(string name);

    /// <summary>
    /// Adds or replaces a pipeline. Pass <paramref name="previousName"/> when the pipeline was renamed
    /// so the old entry is removed too.
    /// </summary>
    void SavePipeline(Pipeline pipeline, string? previousName = null);

    void DeletePipeline(string name);
}
