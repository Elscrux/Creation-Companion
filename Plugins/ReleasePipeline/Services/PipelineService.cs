using System.Reactive;
using System.Reactive.Subjects;
using CreationEditor.Services.State;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services;

public sealed class PipelineService : IPipelineService {
    private readonly IStateRepository<PipelineDTO, PipelineDTO, string> _stateRepository;
    private readonly IPipelineActionCatalog _actionCatalog;
    private readonly Dictionary<string, Pipeline> _pipelines = [];

    private readonly Subject<Unit> _pipelinesChanged = new();
    public IObservable<Unit> PipelinesChanged => _pipelinesChanged;

    public IReadOnlyList<Pipeline> Pipelines => _pipelines.Values.ToList();

    public PipelineService(
        IStateRepositoryFactory<PipelineDTO, PipelineDTO, string> stateRepositoryFactory,
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

    public void SavePipeline(Pipeline pipeline, string? previousName = null) {
        // Handle a rename: remove the stale entry keyed by the old name.
        if (previousName is not null &&
            previousName != pipeline.Name &&
            _pipelines.Remove(previousName)) {
            _stateRepository.Delete(previousName);
        }

        _pipelines[pipeline.Name] = pipeline;
        _stateRepository.Update(_ => ToDto(pipeline), pipeline.Name);

        _pipelinesChanged.OnNext(Unit.Default);
    }

    public void DeletePipeline(string name) {
        if (!_pipelines.Remove(name)) return;

        _stateRepository.Delete(name);
        _pipelinesChanged.OnNext(Unit.Default);
    }

    /// <summary>Rehydrates a <see cref="Pipeline"/> from its persisted DTO.</summary>
    private Pipeline? FromDto(PipelineDTO dto) {
        if (string.IsNullOrWhiteSpace(dto.Name)) return null;

        var pipeline = new Pipeline { Name = dto.Name };
        foreach (var actionDto in dto.Actions) {
            try {
                var action = _actionCatalog.Create(actionDto.Id);
                if (actionDto.Config is not null) {
                    SetConfig(action, actionDto.Config);
                }
                pipeline.Actions.Add(action);
            } catch (ArgumentException) {
                // Unknown action id: skip it so a stale pipeline still loads.
            }
        }

        return pipeline;
    }

    /// <summary>Projects a <see cref="Pipeline"/> into its persisted DTO.</summary>
    private static PipelineDTO ToDto(Pipeline pipeline) {
        var dto = new PipelineDTO { Name = pipeline.Name };
        foreach (var action in pipeline.Actions) {
            dto.Actions.Add(new ActionDTO {
                Id = action.Id,
                Config = GetConfig(action),
            });
        }

        return dto;
    }

    private static object? GetConfig(IPipelineAction action) {
        var configInterface = action.GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineAction<>));

        if (configInterface is null) return null;

        var configProperty = configInterface.GetProperty(nameof(IPipelineAction<object>.Config));
        return configProperty?.GetValue(action);
    }

    private static void SetConfig(IPipelineAction action, object config) {
        var configInterface = action.GetType()
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineAction<>));

        if (configInterface is null) return;

        var configProperty = configInterface.GetProperty(nameof(IPipelineAction<object>.Config));
        configProperty?.SetValue(action, config);
    }
}
