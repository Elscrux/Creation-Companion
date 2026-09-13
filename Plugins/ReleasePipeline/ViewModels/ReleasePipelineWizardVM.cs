using CreationEditor.Avalonia.ViewModels;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Check;
using ReleasePipeline.Services;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Main builder for the release flow. Owns the three wizard step view models and the shared
/// state (release context, selected pipeline) that flows between them. Navigation between
/// steps is handled by the <c>PageHost</c> via the per-step <c>OnNext</c> commands.
/// </summary>
public sealed partial class ReleasePipelineWizardVM : ViewModel {
    private readonly IPipelineService _pipelineService;

    public DataSourceStepVM DataSourceStep { get; }
    public ChecksStepVM ChecksStep { get; }
    public PipelineSelectionStepVM PipelineSelectionStep { get; }

    [Reactive] public partial ReleaseContext? ReleaseContext { get; set; }
    [Reactive] public partial Pipeline? SelectedPipeline { get; set; }

    public ReleasePipelineWizardVM(
        DataSourceStepVM dataSourceStep,
        ChecksStepVM checksStep,
        PipelineSelectionStepVM pipelineSelectionStep,
        IPipelineService pipelineService) {
        DataSourceStep = dataSourceStep;
        ChecksStep = checksStep;
        PipelineSelectionStep = pipelineSelectionStep;
        _pipelineService = pipelineService;

        // Propagate the release context from the data source step to the checks and pipeline steps.
        DataSourceStep.WhenAnyValue(x => x.ReleaseContext)
            .Subscribe(context => {
                ReleaseContext = context;
                ChecksStep.ReleaseContext = context;
                PipelineSelectionStep.ReleaseContext = context;
            })
            .DisposeWith(this);

        // Propagate the release folder from the data source step to the pipeline step.
        DataSourceStep.WhenAnyValue(x => x.ReleaseFolder)
            .Subscribe(folder => PipelineSelectionStep.ReleaseFolder = folder)
            .DisposeWith(this);

        // Track the selected pipeline.
        PipelineSelectionStep.WhenAnyValue(x => x.SelectedPipeline)
            .Subscribe(pipeline => SelectedPipeline = pipeline)
            .DisposeWith(this);
    }

    // ---- Page navigation (PageHost OnNext commands) ----

    /// <summary>Page 1 OnNext: gates advancing past the data source step on a valid release context.</summary>
    [ReactiveCommand(CanExecute = nameof(CanSelectDataSource))]
    private void SelectDataSource() { }

    private bool CanSelectDataSource() => DataSourceStep.ReleaseContext is not null;

    /// <summary>Page 2 OnNext: gates advancing past checks on a completed, non-blocking (or acknowledged) run.</summary>
    [ReactiveCommand(CanExecute = nameof(CanProceedFromChecks))]
    private void ProceedFromChecks() { }

    private bool CanProceedFromChecks() =>
        ChecksStep.CheckItems.Count > 0
        && ChecksStep.CheckItems.All(i => i.Status != CheckStatus.Pending)
        && (!ChecksStep.HasBlockingCheckFailures || ChecksStep.ContinueAnyway);
}

