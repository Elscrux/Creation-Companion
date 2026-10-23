using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
using ReleasePipeline.Services;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// UI wrapper around a <see cref="Pipeline"/> card in the selection list. Provides the run state
/// (start / cancel / progress) for running the pipeline directly from the card. Also used as the
/// "New Pipeline" add card (when <see cref="IsAddCard"/> is true).
/// </summary>
public sealed partial class PipelineCardVM : ViewModel {
    private readonly Func<Pipeline, PipelineContext, PipelineRunVM> _runVMFactory;

    public Pipeline? Pipeline { get; }

    /// <summary>True when this card is the "New Pipeline" add entry, not a real pipeline</summary>
    public bool IsAddCard { get; }

    [Reactive] public partial int ActionCount { get; set; }

    /// <summary>The active run for this pipeline, created lazily on first start</summary>
    [Reactive] public partial PipelineRunVM? Run { get; set; }

    [Reactive] public partial string Name { get; set; }
    [Reactive] public partial bool IsSelected { get; set; }
    [Reactive] public partial bool IsRunning { get; set; }
    [Reactive] public partial bool HasRun { get; set; }
    [Reactive] public partial bool Succeeded { get; set; }
    [Reactive] public partial int CompletedSteps { get; set; }
    [Reactive] public partial int TotalSteps { get; set; }

    public double Progress => TotalSteps == 0 ? 0 : (double) CompletedSteps / TotalSteps;

    /// <summary>True when a run has completed and can be viewed again (read-only)</summary>
    public bool CanViewRun => HasRun && !IsRunning;

    /// <summary>Creates the "New Pipeline" add card</summary>
    public static PipelineCardVM CreateAddCard() => new();

    private PipelineCardVM() {
        IsAddCard = true;
        Name = "New Pipeline";
        _runVMFactory = null!;
    }

    public PipelineCardVM(
        Pipeline pipeline,
        IPipelineService pipelineService,
        Func<Pipeline, PipelineContext, PipelineRunVM> runVMFactory) {
        Pipeline = pipeline;
        var pipelineService1 = pipelineService;
        _runVMFactory = runVMFactory;
        var previousName = pipeline.Name;
        Name = pipeline.Name;
        ActionCount = pipeline.Actions.Count;

        this.WhenAnyValue(x => x.CompletedSteps)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Progress)))
            .DisposeWith(this);

        this.WhenAnyValue(x => x.Name)
            .Where(name => name != previousName)
            .Subscribe(name => {
                if (string.IsNullOrWhiteSpace(name)) {
                    Name = previousName;
                    return;
                }

                pipelineService1.DeletePipeline(previousName);
                pipelineService1.SavePipeline(Pipeline);
                previousName = Pipeline.Name;
            })
            .DisposeWith(this);
    }

    /// <summary>Refreshes the card's display values from the underlying pipeline</summary>
    public void SyncFromPipeline() {
        if (Pipeline is null) return;

        Name = Pipeline.Name;
        ActionCount = Pipeline.Actions.Count;
    }

    /// <summary>
    /// Starts a run for this pipeline. Requires a release context (mod + folder) to have been
    /// provided by the wizard. The run VM is created once and reused across runs.
    /// </summary>
    [ReactiveCommand]
    public async Task Start(PipelineSelectionStepVM step) {
        if (IsRunning) return;
        if (step.PipelineContext is null) return;
        if (Pipeline is null) return;

        // Create the run VM first so it's available when the card is selected.
        if (Run is null) {
            Run = _runVMFactory(Pipeline, step.PipelineContext);

            // Mirror the run VM's state onto the card for the compact card display.
            Run.IsRunning
                .Subscribe(running => {
                    IsRunning = running;
                    this.RaisePropertyChanged(nameof(CanViewRun));
                })
                .DisposeWith(this);
            Run.WhenAnyValue(x => x.HasRun)
                .Subscribe(hasRun => {
                    HasRun = hasRun;
                    this.RaisePropertyChanged(nameof(CanViewRun));
                })
                .DisposeWith(this);
            Run.WhenAnyValue(x => x.Succeeded)
                .Subscribe(succeeded => Succeeded = succeeded)
                .DisposeWith(this);
            Run.WhenAnyValue(x => x.CompletedSteps)
                .Subscribe(steps => CompletedSteps = steps)
                .DisposeWith(this);
            Run.WhenAnyValue(x => x.TotalSteps)
                .Subscribe(steps => TotalSteps = steps)
                .DisposeWith(this);
        }

        // Select this card so the right pane shows its run view while running.
        step.SelectedPipeline = Pipeline;
        step.SelectedCard = this;
        step.ViewingRun = false;

        // Run the pipeline on the thread pool so the UI thread stays responsive.
        await Task.Run(() => Run.StartCommand.Execute());
    }

    [ReactiveCommand(CanExecute = nameof(IsRunning))]
    public void Cancel() => Run?.CancelCommand.Execute().Subscribe();
}
