using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Services.State;
using ReleasePipeline.Models;
namespace ReleasePipeline.Services;

public sealed class PipelineService : IPipelineService {
    private readonly IStateRepository<PipelineDto, PipelineDto, string> _stateRepository;
    private readonly IPipelineActionCatalog _actionCatalog;
    private readonly Dictionary<string, Pipeline> _pipelines = [];

    private readonly Subject<Unit> _pipelinesChanged = new();
    public IObservable<Unit> PipelinesChanged => _pipelinesChanged;

    public IReadOnlyList<Pipeline> Pipelines => _pipelines.Values.ToList();

    public PipelineService(
        IStateRepositoryFactory<PipelineDto, PipelineDto, string> stateRepositoryFactory,
        IPipelineActionCatalog actionCatalog) {
        _stateRepository = stateRepositoryFactory.CreateCached("ReleasePipelines");
        _actionCatalog = actionCatalog;

        foreach (var dto in _stateRepository.LoadAll()) {
            var pipeline = FromDto(dto);
            if (pipeline is not null) {
                _pipelines[pipeline.Name] = pipeline;
            }
        }
    }

    public Pipeline? GetPipeline(string name) => _pipelines.GetValueOrDefault(name);

    public Pipeline CreatePipeline(string? name = null) {
        var pipeline = new Pipeline { Name = GetUniqueName(name ?? "New Pipeline") };
        SavePipeline(pipeline);
        return pipeline;
    }

    public void SavePipeline(Pipeline pipeline) {
        _pipelines[pipeline.Name] = pipeline;
        _stateRepository.Update(_ => ToDto(pipeline), pipeline.Name);

        _pipelinesChanged.OnNext(Unit.Default);
    }

    public void DeletePipeline(string name) {
        if (!_pipelines.Remove(name)) return;

        _stateRepository.Delete(name);
        _pipelinesChanged.OnNext(Unit.Default);
    }

    public Pipeline DuplicatePipeline(Pipeline pipeline, string? newName = null) {
        newName ??= GetUniqueName(pipeline.Name);
        var newPipeline = new Pipeline {
            Name = newName,
            Actions = pipeline.Actions.Select(a => _actionCatalog.Create(a.Id)).ToList(),
        };

        // Copy config values from the original actions to the new actions
        for (var i = 0; i < pipeline.Actions.Count; i++) {
            newPipeline.Actions[i].ConfigValues = pipeline.Actions[i].ConfigValues;
        }

        SavePipeline(newPipeline);
        return newPipeline;
    }

    private string GetUniqueName(string baseName) {
        var existingNames = Pipelines.Select(p => p.Name).ToHashSet();
        if (!existingNames.Contains(baseName)) return baseName;

        var index = 2;
        while (existingNames.Contains($"{baseName} {index}")) {
            index++;
        }

        return $"{baseName} {index}";
    }

    /// <summary>Rehydrates a <see cref="Pipeline"/> from its persisted DTO</summary>
    private Pipeline? FromDto(PipelineDto dto) {
        if (string.IsNullOrWhiteSpace(dto.Name)) return null;

        var pipeline = new Pipeline { Name = dto.Name };
        foreach (var actionDto in dto.Actions) {
            try {
                var action = _actionCatalog.Create(actionDto.Id);
                if (actionDto.Config is not null) {
                    action.ConfigValues = actionDto.Config;
                }
                pipeline.Actions.Add(action);
            } catch (ArgumentException) {
                // Unknown action id: skip it so a stale pipeline still loads.
            }
        }

        return pipeline;
    }

    /// <summary>Projects a <see cref="Pipeline"/> into its persisted DTO</summary>
    private static PipelineDto ToDto(Pipeline pipeline) {
        var dto = new PipelineDto { Name = pipeline.Name };
        foreach (var action in pipeline.Actions) {
            dto.Actions.Add(new ActionDto {
                Id = action.Id,
                Config = action.ConfigValues,
            });
        }

        return dto;
    }
}
