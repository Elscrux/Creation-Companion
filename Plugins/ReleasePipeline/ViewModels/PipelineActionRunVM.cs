using CreationEditor.Avalonia.ViewModels;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Read-only wrapper around a single pipeline action for the runtime view. Tracks the action's
/// live run state (pending / running / passed / failed / ...) and any info message emitted while
/// it runs.
/// </summary>
public sealed partial class PipelineActionRunVM : ViewModel {
    public IPipelineAction Action { get; }

    public string DisplayName => Action.DisplayName;
    public string Description => Action.Description;

    [Reactive] public partial RunStatus Status { get; set; }
    [Reactive] public partial string? Message { get; set; }

    public PipelineActionRunVM(IPipelineAction action) {
        Action = action;
        Status = RunStatus.Pending;
    }
}
