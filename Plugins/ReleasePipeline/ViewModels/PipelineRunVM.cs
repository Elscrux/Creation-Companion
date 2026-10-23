using System.Diagnostics;
using System.Reactive.Subjects;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using DynamicData.Binding;
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
    private readonly PipelineContext _pipelineContext;
    private readonly PipelineRunner _pipelineRunner;
    private readonly Subject<bool> _isRunning = new();

    private CancellationTokenSource? _cts;

    public Pipeline Pipeline { get; }

    /// <summary>Per-action run state, in pipeline order</summary>
    public ObservableCollectionExtended<PipelineActionRunVM> ActionRuns { get; } = [];

    public IObservable<bool> IsRunning => _isRunning;
    public IObservable<bool> IsNotRunning => _isRunning.Negate();

    [Reactive] public partial bool HasRun { get; set; }
    [Reactive] public partial bool Succeeded { get; set; }
    [Reactive] public partial int CompletedSteps { get; set; }
    [Reactive] public partial int TotalSteps { get; set; }

    public double Progress => TotalSteps == 0 ? 0 : (double) CompletedSteps / TotalSteps;

    public PipelineRunVM(
        Pipeline pipeline,
        PipelineContext pipelineContext,
        PipelineRunner pipelineRunner) {
        Pipeline = pipeline;
        _pipelineContext = pipelineContext;
        _pipelineRunner = pipelineRunner;

        foreach (var action in pipeline.Actions) {
            ActionRuns.Add(new PipelineActionRunVM(action));
        }
        TotalSteps = ActionRuns.Count;

        this.WhenAnyValue(x => x.CompletedSteps)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Progress)))
            .DisposeWith(this);
    }

    [ReactiveCommand(CanExecute = nameof(IsNotRunning))]
    private async Task Start() {
        _cts = new CancellationTokenSource();
        _isRunning.OnNext(true);
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
                _pipelineContext,
                _cts.Token,
                progress: new ProgressObserver(this));

            Succeeded = outcome.Succeeded;
        } catch (OperationCanceledException) {
            foreach (var run in ActionRuns.Where(r => r.Status == RunStatus.Running)) {
                run.Status = RunStatus.Skipped;
                run.Message = "Cancelled";
            }
        } finally {
            _isRunning.OnNext(false);
            _cts?.Dispose();
            _cts = null;
        }
    }

    [ReactiveCommand(CanExecute = nameof(IsNotRunning))]
    private void Cancel() => _cts?.Cancel();

    [ReactiveCommand]
    private void OpenReleaseFolder() {
        if (!_pipelineContext.FileSystem.Directory.Exists(_pipelineContext.ReleaseFolder)) return;

        Process.Start(new ProcessStartInfo {
            FileName = _pipelineContext.ReleaseFolder,
            UseShellExecute = true,
        });
    }

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

        public void OnError(Exception error) {}
        public void OnCompleted() {}
    }
}
