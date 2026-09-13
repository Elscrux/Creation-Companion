using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.Views;
using DynamicData.Binding;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Mutagen.Bethesda.Skyrim;
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
    private readonly Func<Pipeline, ReleasePipelineEditorVM> _editorVMFactory;
    private readonly PipelineRunner _pipelineRunner;
    private readonly MainWindow _mainWindow;

    public ObservableCollectionExtended<PipelineCardVM> Pipelines { get; } = [];

    [Reactive] public partial Pipeline? SelectedPipeline { get; set; }
    [Reactive] public partial PipelineCardVM? SelectedCard { get; set; }
    [Reactive] public partial PipelineRunVM? SelectedRun { get; set; }
    [Reactive] public partial ReleasePipelineEditorVM? EditorVM { get; set; }
    [Reactive] public partial ReleaseContext? ReleaseContext { get; set; }
    [Reactive] public partial string ReleaseFolder { get; set; } = string.Empty;
    [Reactive] public partial bool ShowEditor { get; set; }
    [Reactive] public partial bool ShowRun { get; set; }
    [Reactive] public partial bool HasSelection { get; set; }

    public PipelineSelectionStepVM(
        IPipelineService pipelineService,
        Func<Pipeline, ReleasePipelineEditorVM> editorVMFactory,
        PipelineRunner pipelineRunner,
        MainWindow mainWindow) {
        _pipelineService = pipelineService;
        _editorVMFactory = editorVMFactory;
        _pipelineRunner = pipelineRunner;
        _mainWindow = mainWindow;

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
                EditorVM = card?.Pipeline is null ? null : _editorVMFactory(card.Pipeline);
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
                    .Subscribe(_ => UpdatePaneVisibility(card))
                    .DisposeWith(this);
            })
            .DisposeWith(this);

        RefreshPipelines();
    }

    private void UpdatePaneVisibility(PipelineCardVM? card) {
        HasSelection = card is not null;
        // Show the read-only run view only while the pipeline is running; return to the editor
        // once the run completes so the user can keep editing.
        ShowRun = card is not null && card.IsRunning;
        ShowEditor = card is not null && !card.IsRunning;
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

            Pipelines.Add(new PipelineCardVM(pipeline, _pipelineService, CreateRunVM) {
                IsSelected = pipeline == SelectedPipeline,
            });
        }

        // Ensure the "New Pipeline" add card is always the last entry.
        if (Pipelines.LastOrDefault() is not { IsAddCard: true }) {
            foreach (var addCard in Pipelines.Where(c => c.IsAddCard).ToList()) {
                Pipelines.Remove(addCard);
            }
            Pipelines.Add(PipelineCardVM.CreateAddCard());
        }
    }

    private PipelineRunVM CreateRunVM(Pipeline pipeline, ISkyrimModGetter mod, string folder)
        => new(pipeline, mod, folder, _pipelineRunner);

    [ReactiveCommand]
    private void SelectPipeline(PipelineCardVM pipeline) {
        SelectedPipeline = pipeline.Pipeline;
        SelectedCard = pipeline;
    }

    /// <summary>Opens the editor for the given pipeline card (same as selecting it).</summary>
    [ReactiveCommand]
    private void EditPipeline(PipelineCardVM pipeline) {
        SelectedPipeline = pipeline.Pipeline;
        SelectedCard = pipeline;
    }

    [ReactiveCommand]
    private void AddPipeline() {
        var pipeline = new Pipeline { Name = GetUniqueName("New Pipeline") };
        _pipelineService.SavePipeline(pipeline);
        SelectedPipeline = pipeline;

        // Select the newly created pipeline so its editor opens on the right immediately.
        SelectedCard = Pipelines.FirstOrDefault(c => c.Pipeline == pipeline);
    }

    /// <summary>Starts all non-running pipelines concurrently.</summary>
    [ReactiveCommand]
    private void RunAll() {
        foreach (var card in Pipelines.Where(c => !c.IsAddCard && !c.IsRunning)) {
            card.Start(this);
        }
    }

    private string GetUniqueName(string baseName) {
        var existingNames = _pipelineService.Pipelines.Select(p => p.Name).ToHashSet();
        if (!existingNames.Contains(baseName)) return baseName;

        var index = 2;
        while (existingNames.Contains($"{baseName} {index}")) {
            index++;
        }

        return $"{baseName} {index}";
    }

    [ReactiveCommand]
    private async Task DeletePipeline(PipelineCardVM pipeline) {
        if (pipeline.Pipeline is null) return;

        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Delete Pipeline",
            $"Are you sure you want to delete the pipeline '{pipeline.Pipeline.Name}'?",
            ButtonEnum.YesNo,
            Icon.Warning);

        var result = await messageBox.ShowWindowDialogAsync(_mainWindow);
        if (result != ButtonResult.Yes) return;

        _pipelineService.DeletePipeline(pipeline.Pipeline.Name);
        if (SelectedPipeline == pipeline.Pipeline) {
            SelectedPipeline = null;
            SelectedCard = null;
        }
    }
}
