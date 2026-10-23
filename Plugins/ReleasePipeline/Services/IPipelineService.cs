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
    Pipeline CreatePipeline(string? name = null);
    void SavePipeline(Pipeline pipeline);
    void DeletePipeline(string name);
    Pipeline DuplicatePipeline(Pipeline pipeline, string? newName = null);
}
