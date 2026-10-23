using CreationEditor.Avalonia.ViewModels;
using DynamicData.Binding;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Check;
using ReleasePipeline.Services;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Wizard step 2: run the fixed pre-release check suite and review per-check results.
/// </summary>
public sealed partial class ChecksStepVM : ViewModel {
    private readonly PipelineRunner _pipelineRunner;

    public ObservableCollectionExtended<CheckResultVM> CheckItems { get; } = [];

    [Reactive] public partial PipelineContext? PipelineContext { get; set; }
    [Reactive] public partial bool HasBlockingCheckFailures { get; set; }
    [Reactive] public partial bool ContinueAnyway { get; set; }
    [Reactive] public partial bool IsRunning { get; set; }

    public ChecksStepVM(
        PipelineRunner pipelineRunner,
        Func<IReleaseCheck, CheckResultVM> checkResultVMFactory) {
        _pipelineRunner = pipelineRunner;

        // Populate the check list from the runner's fixed suite.
        foreach (var check in pipelineRunner.Checks) {
            CheckItems.Add(checkResultVMFactory(check));
        }
    }

    [ReactiveCommand]
    // TODO [ReactiveCommand(RunInBackground = true)]
    private async Task RunChecks(CancellationToken token) {
        if (PipelineContext is null) return;

        foreach (var item in CheckItems) {
            item.Status = CheckStatus.Pending;
        }

        HasBlockingCheckFailures = false;
        IsRunning = true;
        try {
            var outcomes = await _pipelineRunner.RunChecksAsync(PipelineContext.Mod, token);
            foreach (var outcome in outcomes) {
                var item = CheckItems.FirstOrDefault(i => i.Check == outcome.Check);
                item?.ApplyResult(outcome.Result);
            }

            HasBlockingCheckFailures = outcomes.Any(o => o.Result.Status == CheckStatus.Failed);
        } finally {
            IsRunning = false;
        }
    }
}
