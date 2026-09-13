using System.Reactive;
using CreationEditor.Avalonia.ViewModels;
using DynamicData.Binding;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using ReleasePipeline.Services;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Orchestrates a single pipeline run. Owns the per-action run state, the step progress bar and
/// the start / cancel commands. A run is tied to one pipeline and one release context.
/// </summary>
public sealed partial class PipelineRunVM : ViewModel {
    private readonly PipelineRunner _pipelineRunner;
    private CancellationTokenSource? _cts;

    public Pipeline Pipeline { get; }
    public ISkyrimModGetter Mod { get; }
    public string ReleaseFolder { get; }

    /// <summary>Per-action run state, in pipeline order.</summary>
    public ObservableCollectionExtended<PipelineActionRunVM> ActionRuns { get; } = [];

    [Reactive] public partial bool IsRunning { get; set; }
    [Reactive] public partial bool HasRun { get; set; }
    [Reactive] public partial bool Succeeded { get; set; }
    [Reactive] public partial int CompletedSteps { get; set; }
    [Reactive] public partial int TotalSteps { get; set; }

    public double Progress => TotalSteps == 0 ? 0 : (double)CompletedSteps / TotalSteps;

    public ReactiveCommand<Unit, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public PipelineRunVM(
        Pipeline pipeline,
        ISkyrimModGetter mod,
        string releaseFolder,
        PipelineRunner pipelineRunner) {
        Pipeline = pipeline;
        Mod = mod;
        ReleaseFolder = releaseFolder;
        _pipelineRunner = pipelineRunner;

        foreach (var action in pipeline.Actions) {
            ActionRuns.Add(new PipelineActionRunVM(action));
        }
        TotalSteps = ActionRuns.Count;

        var canStart = this.WhenAnyValue(x => x.IsRunning, running => !running);
        StartCommand = ReactiveCommand.CreateFromTask(StartAsync, canStart);
        CancelCommand = ReactiveCommand.Create(Cancel, canStart);

        this.WhenAnyValue(x => x.CompletedSteps)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Progress)))
            .DisposeWith(this);
    }

    private async Task StartAsync() {
        if (IsRunning) return;

        _cts = new CancellationTokenSource();
        IsRunning = true;
        HasRun = true;
        Succeeded = false;
        CompletedSteps = 0;

        foreach (var run in ActionRuns) {
            run.Status = RunStatus.Pending;
            run.Message = null;
        }

        try {
            var outcome = await _pipelineRunner.RunPipelineAsync(
                Pipeline,
                Mod,
                ReleaseFolder,
                _cts.Token,
                progress: new ProgressObserver(this));

            Succeeded = outcome.Succeeded;
        } catch (OperationCanceledException) {
            foreach (var run in ActionRuns.Where(r => r.Status == RunStatus.Running)) {
                run.Status = RunStatus.Skipped;
                run.Message = "Cancelled";
            }
        } finally {
            IsRunning = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private void Cancel() => _cts?.Cancel();

    /// <summary>Forwards <see cref="ActionRunProgress"/> into the matching action run VM.</summary>
    private sealed class ProgressObserver(PipelineRunVM owner) : IObserver<ActionRunProgress> {
        public void OnNext(ActionRunProgress value) {
            var run = owner.ActionRuns.FirstOrDefault(r => r.Action == value.Action);
            if (run is null) return;

            run.Status = value.Status;
            run.Message = value.Message;

            if (value.Status is RunStatus.Passed or RunStatus.Failed or RunStatus.Warned or RunStatus.Skipped) {
                owner.CompletedSteps++;
            }
        }

        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}
