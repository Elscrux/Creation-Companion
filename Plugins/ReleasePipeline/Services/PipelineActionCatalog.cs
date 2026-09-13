using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services;

/// <summary>
/// Supplies the known <see cref="IPipelineAction" /> types so the UI can add configured
/// actions to a pipeline. Each call returns fresh instances.
/// </summary>
public interface IPipelineActionCatalog {
    IReadOnlyList<IPipelineAction> CreateAll();
    IPipelineAction Create(Guid id);
}

public sealed class PipelineActionCatalog(Func<IPipelineAction>[] pipelineActionFactories) : IPipelineActionCatalog {
    public IReadOnlyList<IPipelineAction> CreateAll() => pipelineActionFactories.Select(f => f()).ToList();

    public IPipelineAction Create(Guid id) {
        var instance = pipelineActionFactories.Select(f => f()).FirstOrDefault(a => a.Id == id);
        return instance ?? throw new ArgumentException($"Unknown pipeline action id '{id}'", nameof(id));
    }
}
