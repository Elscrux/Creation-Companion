using System.Reactive.Linq;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Integrations;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using ReleasePipeline.Services;
using ReleasePipeline.Services.ConfigEditor;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Edits a single <see cref="Pipeline"/>: its name, ordered actions (add/remove/reorder via
/// drag-drop) and each action's configuration.
/// </summary>
public sealed partial class ReleasePipelineEditorVM : ViewModel {
    private readonly IPipelineService _pipelineService;
    private readonly IPipelineActionCatalog _actionCatalog;
    private readonly IConfigEditorService _configEditorService;
    private readonly IIntegrationRegistry _integrationRegistry;
    private readonly IIntegrationProvider _integrationProvider;

    private string? _previousName;

    public Pipeline Pipeline { get; }

    public ObservableCollectionExtended<PipelineActionVM> ActionVMs { get; } = [];

    public IReadOnlyList<IPipelineAction> AvailableActions { get; }

    [Reactive] public partial string Name { get; set; }
    [Reactive] public partial IPipelineAction? SelectedAvailableAction { get; set; }

    public ReleasePipelineEditorVM(
        Pipeline pipeline,
        IPipelineService pipelineService,
        IPipelineActionCatalog actionCatalog,
        IConfigEditorService configEditorService,
        IIntegrationRegistry integrationRegistry,
        IIntegrationProvider integrationProvider) {
        Pipeline = pipeline;
        _pipelineService = pipelineService;
        _actionCatalog = actionCatalog;
        _configEditorService = configEditorService;
        _integrationRegistry = integrationRegistry;
        _integrationProvider = integrationProvider;

        _previousName = pipeline.Name;
        Name = pipeline.Name;
        AvailableActions = _actionCatalog.CreateAll();
        SelectedAvailableAction = AvailableActions.FirstOrDefault();

        foreach (var action in pipeline.Actions) {
            ActionVMs.Add(CreateActionVM(action));
        }

        // Update the pipeline's name as the user types (persisted on Save).
        this.WhenAnyValue(x => x.Name)
            .Where(name => name != Pipeline.Name)
            .Subscribe(name => Pipeline.Name = name);

        // Keep Pipeline.Actions in sync with the (drag-drop reorderable) ActionVMs order.
        ActionVMs.WhenCollectionChanges()
            .Subscribe(_ => {
                Pipeline.Actions.Clear();
                foreach (var vm in ActionVMs) {
                    Pipeline.Actions.Add(vm.Action);
                }
            })
            .DisposeWith(this);
    }

    private PipelineActionVM CreateActionVM(IPipelineAction action) {
        var vm = new PipelineActionVM(
            action,
            _configEditorService,
            _integrationRegistry,
            _integrationProvider);
        vm.RemoveRequested
            .Subscribe(RemoveAction)
            .DisposeWith(this);
        return vm;
    }

    [ReactiveCommand]
    private void AddAction(Guid actionId) {
        var action = _actionCatalog.Create(actionId);
        Pipeline.Actions.Add(action);
        ActionVMs.Add(CreateActionVM(action));
    }

    [ReactiveCommand]
    private void RemoveAction(PipelineActionVM actionVM) {
        Pipeline.Actions.Remove(actionVM.Action);
        ActionVMs.Remove(actionVM);
    }

    /// <summary>Persists the current pipeline state (name + actions) to the service.</summary>
    [ReactiveCommand]
    private void Save() {
        _pipelineService.SavePipeline(Pipeline, _previousName);
        _previousName = Pipeline.Name;
    }
}
