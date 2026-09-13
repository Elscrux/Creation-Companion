using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.ViewModels;
using Mutagen.Bethesda.Skyrim;
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
    private readonly IPipelineService _pipelineService;
    private readonly Func<Pipeline, ISkyrimModGetter, string, PipelineRunVM> _runVMFactory;
    private string _previousName;

    public Pipeline? Pipeline { get; }

    /// <summary>True when this card is the "New Pipeline" add entry, not a real pipeline.</summary>
    public bool IsAddCard { get; }

    [Reactive] public partial int ActionCount { get; set; }

    /// <summary>The active run for this pipeline, created lazily on first start.</summary>
    [Reactive] public partial PipelineRunVM? Run { get; set; }

    [Reactive] public partial string Name { get; set; }
    [Reactive] public partial bool IsSelected { get; set; }
    [Reactive] public partial bool IsRunning { get; set; }
    [Reactive] public partial bool HasRun { get; set; }
    [Reactive] public partial bool Succeeded { get; set; }
    [Reactive] public partial int CompletedSteps { get; set; }
    [Reactive] public partial int TotalSteps { get; set; }

    public double Progress => TotalSteps == 0 ? 0 : (double)CompletedSteps / TotalSteps;

    public ReactiveCommand<PipelineSelectionStepVM, Unit> StartCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    /// <summary>Creates the "New Pipeline" add card.</summary>
    public static PipelineCardVM CreateAddCard() => new();

    private PipelineCardVM() {
        IsAddCard = true;
        Name = "New Pipeline";
        _pipelineService = null!;
        _runVMFactory = null!;
        _previousName = string.Empty;

        var canStart = this.WhenAnyValue(x => x.IsRunning, running => !running);
        StartCommand = ReactiveCommand.Create<PipelineSelectionStepVM>(_ => { }, canStart);
        CancelCommand = ReactiveCommand.Create(() => { }, canStart);
    }

    public PipelineCardVM(
        Pipeline pipeline,
        IPipelineService pipelineService,
        Func<Pipeline, ISkyrimModGetter, string, PipelineRunVM> runVMFactory) {
        Pipeline = pipeline;
        _pipelineService = pipelineService;
        _runVMFactory = runVMFactory;
        _previousName = pipeline.Name;
        Name = pipeline.Name;
        ActionCount = pipeline.Actions.Count;

        var canStart = this.WhenAnyValue(x => x.IsRunning, running => !running);
        StartCommand = ReactiveCommand.Create<PipelineSelectionStepVM>(Start, canStart);
        CancelCommand = ReactiveCommand.Create(Cancel, canStart);

        this.WhenAnyValue(x => x.CompletedSteps)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Progress)))
            .DisposeWith(this);

        this.WhenAnyValue(x => x.Name)
            .Where(name => name != _previousName)
            .Subscribe(name => {
                if (string.IsNullOrWhiteSpace(name)) {
                    Name = _previousName;
                    return;
                }

                _pipelineService.SavePipeline(Pipeline, _previousName);
                _previousName = Pipeline.Name;
            })
            .DisposeWith(this);
    }

    /// <summary>Refreshes the card's display values from the underlying pipeline.</summary>
    public void SyncFromPipeline() {
        if (Pipeline is null) return;
        Name = Pipeline.Name;
        ActionCount = Pipeline.Actions.Count;
    }

    /// <summary>
    /// Starts a run for this pipeline. Requires a release context (mod + folder) to have been
    /// provided by the wizard. The run VM is created once and reused across runs.
    /// </summary>
    public void Start(PipelineSelectionStepVM step) {
        if (IsRunning) return;
        if (step.ReleaseContext is null) return;
        if (Pipeline is null) return;

        var mod = step.ReleaseContext.Mod;
        var releaseFolder = step.ReleaseFolder;

        // Create the run VM first so it's available when the card is selected.
        if (Run is null) {
            Run = _runVMFactory(Pipeline, mod, releaseFolder);

            // Mirror the run VM's state onto the card for the compact card display.
            Run.WhenAnyValue(x => x.IsRunning)
                .Subscribe(running => IsRunning = running)
                .DisposeWith(this);
            Run.WhenAnyValue(x => x.HasRun)
                .Subscribe(hasRun => HasRun = hasRun)
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

        Run.StartCommand.Execute().Subscribe();
    }

    private void Cancel() => Run?.CancelCommand.Execute().Subscribe();
}
