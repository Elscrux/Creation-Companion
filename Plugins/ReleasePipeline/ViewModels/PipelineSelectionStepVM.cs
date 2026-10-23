using CreationEditor;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using DynamicData.Binding;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models;
using ReleasePipeline.Services;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// Wizard step 3: pick a pipeline from a card list. Selecting a card opens its editor inline on
/// the right side of a split view (or a read-only runtime view while it is running).
/// </summary>
public sealed partial class PipelineSelectionStepVM : ViewModel {
    private readonly IPipelineService _pipelineService;
    private readonly Func<Pipeline, PipelineCardVM> _pipelineCardVMFactory;
    private readonly IMainWindowProvider _mainWindowProvider;

    public ObservableCollectionExtended<PipelineCardVM> Pipelines { get; } = [];

    [Reactive] public partial Pipeline? SelectedPipeline { get; set; }
    [Reactive] public partial PipelineCardVM? SelectedCard { get; set; }
    [Reactive] public partial PipelineRunVM? SelectedRun { get; set; }
    [Reactive] public partial PipelineEditorVM? EditorVM { get; set; }
    [Reactive] public partial PipelineContext? PipelineContext { get; set; }
    [Reactive] public partial bool ShowEditor { get; set; }
    [Reactive] public partial bool ShowRun { get; set; }
    [Reactive] public partial bool HasSelection { get; set; }

    /// <summary>True while the user is viewing a completed run's results (read-only)</summary>
    [Reactive] public partial bool ViewingRun { get; set; }

    public PipelineSelectionStepVM(
        IPipelineService pipelineService,
        Func<Pipeline, PipelineEditorVM> editorVMFactory,
        Func<Pipeline, PipelineCardVM> pipelineCardVMFactory,
        IMainWindowProvider mainWindowProvider) {
        _pipelineService = pipelineService;
        _pipelineCardVMFactory = pipelineCardVMFactory;
        _mainWindowProvider = mainWindowProvider;

        // Keep the observable collection in sync with the service.
        pipelineService.PipelinesChanged
            .Subscribe(_ => RefreshPipelines())
            .DisposeWith(this);

        // Mark the selected card with an accent background.
        this.WhenAnyValue(x => x.SelectedPipeline)
            .Subscribe(selected => {
                foreach (var card in Pipelines) {
                    card.IsSelected = card.Pipeline == selected;
                }
            })
            .DisposeWith(this);

        // When the selected card changes, build (or clear) the inline editor and update the
        // editor / runtime visibility.
        this.WhenAnyValue(x => x.SelectedCard)
            .Subscribe(card => {
                EditorVM = card?.Pipeline is null ? null : editorVMFactory(card.Pipeline);
                SelectedRun = card?.Run;
                UpdatePaneVisibility(card);

                // React to the card's run state changing (start / finish) to switch panes.
                card?.WhenAnyValue(c => c.Run)
                    .Subscribe(run => {
                        SelectedRun = run;
                        UpdatePaneVisibility(card);
                    })
                    .DisposeWith(this);
                card?.WhenAnyValue(c => c.IsRunning)
                    .WhereTrue()
                    .Subscribe(_ => UpdatePaneVisibility(card))
                    .DisposeWith(this);
            })
            .DisposeWith(this);

        RefreshPipelines();
    }

    private void UpdatePaneVisibility(PipelineCardVM? card) {
        HasSelection = card is not null;
        // Show the read-only run view while the pipeline is running, or when the user explicitly
        // chose to view the last run's results; otherwise show the editor.
        ShowRun = card is not null && (card.IsRunning || ViewingRun);
        ShowEditor = card is not null && !card.IsRunning && !ViewingRun;
    }

    private void RefreshPipelines() {
        var servicePipelines = _pipelineService.Pipelines;

        // Remove real cards whose pipeline no longer exists (keep the add card).
        foreach (var card in Pipelines.Where(c => !c.IsAddCard && !servicePipelines.Contains(c.Pipeline)).ToList()) {
            Pipelines.Remove(card);
        }

        // Refresh existing cards and add cards for new pipelines, preserving card instances.
        foreach (var pipeline in servicePipelines) {
            var existing = Pipelines.FirstOrDefault(c => c.Pipeline == pipeline);
            if (existing is not null) {
                existing.SyncFromPipeline();
                continue;
            }

            var pipelineCardVM = _pipelineCardVMFactory(pipeline);
            pipelineCardVM.IsSelected = pipeline == SelectedPipeline;
            Pipelines.Add(pipelineCardVM);
        }

        // Ensure the "New Pipeline" add card is always the last entry.
        if (Pipelines.LastOrDefault() is not { IsAddCard: true }) {
            foreach (var addCard in Pipelines.Where(c => c.IsAddCard).ToList()) {
                Pipelines.Remove(addCard);
            }
            Pipelines.Add(PipelineCardVM.CreateAddCard());
        }
    }

    [ReactiveCommand]
    private void SelectPipeline(PipelineCardVM pipeline) {
        ViewingRun = false;
        SelectedPipeline = pipeline.Pipeline;
        SelectedCard = pipeline;
    }

    /// <summary>Opens the editor for the given pipeline card (same as selecting it)</summary>
    [ReactiveCommand]
    private void EditPipeline(PipelineCardVM pipeline) {
        ViewingRun = false;
        SelectedPipeline = pipeline.Pipeline;
        SelectedCard = null;
        SelectedCard = pipeline;
    }

    /// <summary>Shows the last run's results for the given pipeline card (read-only)</summary>
    [ReactiveCommand]
    private void ViewRun(PipelineCardVM pipeline) {
        ViewingRun = true;
        SelectedPipeline = pipeline.Pipeline;
        SelectedCard = null;
        SelectedCard = pipeline;
    }

    [ReactiveCommand]
    private void AddPipeline() {
        var pipeline = _pipelineService.CreatePipeline();
        SelectedPipeline = pipeline;

        // Select the newly created pipeline so its editor opens on the right immediately.
        SelectedCard = Pipelines.FirstOrDefault(c => c.Pipeline == pipeline);
    }

    /// <summary>Starts all non-running pipelines concurrently</summary>
    [ReactiveCommand]
    private async Task RunAll() {
        await Task.WhenAll(Pipelines
            .Where(c => !c.IsAddCard && !c.IsRunning)
            .Select(c => c.Start(this)));
    }

    [ReactiveCommand]
    private void DuplicatePipeline(PipelineCardVM pipeline) {
        if (pipeline.Pipeline is null) return;

        var newPipeline = _pipelineService.DuplicatePipeline(pipeline.Pipeline);

        // Select the newly created duplicate so its editor opens on the right immediately.
        SelectedPipeline = newPipeline;
        SelectedCard = Pipelines.FirstOrDefault(c => c.Pipeline == newPipeline);
    }

    [ReactiveCommand]
    private async Task DeletePipeline(PipelineCardVM pipeline) {
        if (_mainWindowProvider.Current is null) return;
        if (pipeline.Pipeline is null) return;

        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Delete Pipeline",
            $"Are you sure you want to delete the pipeline '{pipeline.Pipeline.Name}'?",
            ButtonEnum.YesNo,
            Icon.Warning);

        var result = await messageBox.ShowWindowDialogAsync(_mainWindowProvider.Current);
        if (result != ButtonResult.Yes) return;

        _pipelineService.DeletePipeline(pipeline.Pipeline.Name);
        if (SelectedPipeline == pipeline.Pipeline) {
            SelectedPipeline = null;
            SelectedCard = null;
        }
    }
}
