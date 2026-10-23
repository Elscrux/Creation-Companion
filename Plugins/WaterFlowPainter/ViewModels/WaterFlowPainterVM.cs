using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CreationEditor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Config;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.Textures;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Serilog;
using SkiaSharp;
using WaterFlowPainter.Services;
namespace WaterFlowPainter.ViewModels;

public sealed class WaterFlowPainterConfig : IConfigDefinition {
    public static IntegrationConfigOption TextureProcessorOption { get; } = new(
        "Texture Processor",
        typeof(ITextureProcessor),
        "Integration used to convert painted flowmaps into DDS files");

    public IReadOnlyList<IConfigOption> Options { get; } = [
        TextureProcessorOption,
    ];
}

public sealed partial class WaterFlowPainterVM : ViewModel {
    private const int MaxOverviewPixels = 4096;

    private readonly FlowmapService _flowmapService;
    private readonly CellHeightmapService _cellHeightmapService;
    private readonly FlowmapRenderer _flowmapRenderer;
    private readonly FlowmapPainter _flowmapPainter;
    private readonly ILogger _logger;
    private readonly IIntegrationRegistry _integrationRegistry;
    private readonly Subject<Unit> _flowPreviewRefresh = new();

    public FlowParticleSimulator ParticleSimulator { get; }

    public ILinkCacheProvider LinkCacheProvider { get; }
    public ConfigVM ConfigVM { get; }

    public ObservableCollectionExtended<WaterCellVM> Cells { get; } = new();

    /// <summary>Raised when a cell's flowmap changed and the canvas overlay should redraw its cached image.</summary>
    public event EventHandler<WaterCellVM>? EditInvalidated;

    [Reactive] public partial FormKey WorldspaceFormKey { get; set; }
    [Reactive] public partial int LeftCell { get; set; }
    [Reactive] public partial int RightCell { get; set; }
    [Reactive] public partial int TopCell { get; set; }
    [Reactive] public partial int BottomCell { get; set; }

    /// <summary>Overview pixels per cell, decimated from the 64x64 flowmap to bound the canvas size.</summary>
    [Reactive] public partial int OverviewCellSize { get; set; }

    [Reactive] public partial bool ShowArrows { get; set; }
    [Reactive] public partial int BusyTasks { get; set; }

    public bool IsBusy => BusyTasks > 0;
    [Reactive] public partial WaterCellVM? LastPaintedCell { get; set; }
    [Reactive] public partial IImage? FlowPreviewImage { get; set; }
    [Reactive] public partial double CompositeWidth { get; set; }
    [Reactive] public partial double CompositeHeight { get; set; }
    [Reactive] public partial FlowBrushMode BrushMode { get; set; }
    [Reactive] public partial double BrushSize { get; set; }
    [Reactive] public partial double BrushStrength { get; set; }
    [Reactive] public partial double SpeedValue { get; set; }
    [Reactive] public partial double ScaleValue { get; set; }
    [Reactive] public partial string StatusText { get; set; } = string.Empty;

    public IObservable<bool> CanSave { get; }

    public WaterFlowPainterVM(
        FlowmapService flowmapService,
        CellHeightmapService cellHeightmapService,
        FlowmapRenderer flowmapRenderer,
        FlowmapPainter flowmapPainter,
        FlowParticleSimulator particleSimulator,
        IIntegrationRegistry integrationRegistry,
        IConfigVMFactory configVMFactory,
        ILinkCacheProvider linkCacheProvider,
        ILogger logger) {
        _flowmapService = flowmapService;
        _cellHeightmapService = cellHeightmapService;
        _flowmapRenderer = flowmapRenderer;
        _flowmapPainter = flowmapPainter;
        ParticleSimulator = particleSimulator;
        _integrationRegistry = integrationRegistry;
        _logger = logger;
        LinkCacheProvider = linkCacheProvider;

        ConfigVM = configVMFactory.Create(new WaterFlowPainterConfig());
        ConfigVM.DisposeWith(this);

        LeftCell = -2;
        RightCell = 2;
        BottomCell = -2;
        TopCell = 2;
        OverviewCellSize = FlowmapRenderer.FlowmapPixelSize;
        BrushSize = 4;
        BrushStrength = 0.5;
        SpeedValue = 1;
        ScaleValue = 0.25;

        CanSave = Cells
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsModified)
            .ToCollection()
            .Select(cells => cells.Any(x => x.IsModified))
            .ObserveOnGui();

        this.WhenAnyValue(x => x.BusyTasks)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(IsBusy)));

        // Load all flowmap cells of the selected worldspace
        this.WhenAnyValue(x => x.WorldspaceFormKey)
            .Where(worldspace => !worldspace.IsNull)
            .ThrottleMedium()
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            .ObserveOnTaskpool()
            .Select(LoadCells)
            .ObserveOnGui()
            .Do(cells => {
                Cells.LoadOptimized(cells);

                if (cells.Count > 0) {
                    SetCellRange(
                        cells.Min(c => c.Position.X) - 1,
                        cells.Max(c => c.Position.X) + 1,
                        cells.Min(c => c.Position.Y) - 1,
                        cells.Max(c => c.Position.Y) + 1);
                }

                StatusText = $"Found {cells.Count} cells with flowmap enabled water.";
                BusyTasks--;
            })
            .Subscribe(
                _ => {},
                e => {
                    BusyTasks--;
                    StatusText = "Failed to load the cells of the worldspace, see log.";
                    _logger.Here().Error(e, "Failed to load flowmap cells: {Message}", e.Message);
                })
            .DisposeWith(this);

        var gridUpdates = this.WhenAnyValue(
                x => x.LeftCell,
                x => x.TopCell,
                x => x.RightCell,
                x => x.BottomCell,
                (left, top, right, bottom) => (Left: left, Top: top, Right: right, Bottom: bottom))
            .ThrottleMedium();

        // Flow preview: rebuilt on grid, arrow toggle and flowmap changes
        var flowPreviewTrigger = Observable.Merge(
            gridUpdates.Select(_ => Unit.Default),
            this.WhenAnyValue(x => x.ShowArrows).Select(_ => Unit.Default),
            _flowPreviewRefresh);

        flowPreviewTrigger
            .ThrottleMedium()
            .ObserveOnGui()
            .Do(_ => BusyTasks++)
            // Snapshot the cells on the GUI thread, the collection must not be touched from the taskpool
            .Select(_ => Cells.ToList())
            .ObserveOnTaskpool()
            .Select(cells => BuildFlowPreview(cells))
            .ObserveOnGui()
            .Do(bitmap => {
                var width = bitmap.Width;
                var height = bitmap.Height;
                FlowPreviewImage = ToImage(bitmap);
                bitmap.Dispose();
                CompositeWidth = width;
                CompositeHeight = height;
                BusyTasks--;
            })
            .Subscribe(
                _ => {},
                e => {
                    BusyTasks--;
                    StatusText = "Failed to build the flow preview, see log.";
                    _logger.Here().Error(e, "Failed to build flow preview: {Message}", e.Message);
                })
            .DisposeWith(this);
    }

    /// <summary>Sets the visible cell range and derives the overview resolution from its extent.</summary>
    private void SetCellRange(int left, int right, int bottom, int top) {
        LeftCell = left;
        RightCell = right;
        BottomCell = bottom;
        TopCell = top;

        var maxExtent = Math.Max(right - left, top - bottom);
        OverviewCellSize = Math.Clamp(MaxOverviewPixels / Math.Max(1, maxExtent), 4, FlowmapRenderer.FlowmapPixelSize);
    }

    private List<WaterCellVM> LoadCells(FormKey worldspaceFormKey) {
        var cells = new List<WaterCellVM>();
        foreach (var flowmapCell in _flowmapService.GetFlowmapCells(worldspaceFormKey)) {
            try {
                var flowmap = _flowmapService.LoadFlowmap(flowmapCell.FlowmapPath);
                var landMask = _cellHeightmapService.GetLandMask(flowmapCell.Cell);
                cells.Add(new WaterCellVM(flowmapCell, flowmap, landMask));
            } catch (Exception e) {
                _logger.Here().Error(e, "Failed to load cell {X}.{Y}: {Message}", flowmapCell.Position.X, flowmapCell.Position.Y, e.Message);
            }
        }

        return cells;
    }

    private SKBitmap BuildFlowPreview(IReadOnlyList<WaterCellVM> cells) {
        var renderCells = cells.Select(cell => new FlowmapRenderer.RenderCell(cell.Position, cell.Flowmap, cell.LandMask));
        return _flowmapRenderer.CreateFlowPreview(renderCells, LeftCell, BottomCell, RightCell, TopCell, ShowArrows, OverviewCellSize);
    }

    private static IImage? ToImage(SKBitmap bitmap) {
        using var stream = FlowmapRenderer.EncodePng(bitmap);
        return new Bitmap(stream);
    }

    #region Painting
    /// <summary>Snapshots the cell's flowmap before a stroke starts, for undo.</summary>
    public void BeginStroke(WaterCellVM cell) {
        LastPaintedCell = cell;
        cell.UndoStack.Push(cell.Flowmap.Clone());
    }

    /// <summary>Paints a brush dab at the given position (in flowmap pixels) of the cell.</summary>
    /// <param name="cell">Cell to paint on</param>
    /// <param name="x">Brush center X in flowmap pixels</param>
    /// <param name="y">Brush center Y in flowmap pixels</param>
    /// <param name="direction">Flow direction in radians, measured from east towards north</param>
    public void PaintAt(WaterCellVM cell, double x, double y, double direction) {
        LastPaintedCell = cell;
        cell.IsModified = true;

        _flowmapPainter.Paint(
            cell.Flowmap,
            (float) x,
            (float) y,
            BrushMode,
            (float) BrushSize,
            (float) BrushStrength,
            (float) direction,
            (float) SpeedValue,
            (float) ScaleValue);

        EditInvalidated?.Invoke(this, cell);
    }

    /// <summary>Refreshes the overview preview after a stroke ended.</summary>
    public void EndStroke() {
        _flowPreviewRefresh.OnNext(Unit.Default);
    }

    [ReactiveCommand]
    private void Undo() {
        if (LastPaintedCell is null || LastPaintedCell.UndoStack.Count == 0) return;

        var snapshot = LastPaintedCell.UndoStack.Pop();
        LastPaintedCell.Flowmap.SetFrom(snapshot);
        EditInvalidated?.Invoke(this, LastPaintedCell);
        _flowPreviewRefresh.OnNext(Unit.Default);
    }

    [ReactiveCommand]
    private void SetBrushMode(FlowBrushMode mode) {
        BrushMode = mode;
    }
    #endregion

    [ReactiveCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAll() {
        var configValues = ConfigVM.GetConfigValues();
        var processorConfig = configValues.TryGet(WaterFlowPainterConfig.TextureProcessorOption);
        if (processorConfig is null) {
            StatusText = "No texture processor integration configured.";
            return;
        }

        var processor = _integrationRegistry.TryGetIntegration<ITextureProcessor>(processorConfig.IntegrationId);
        if (processor is null) {
            StatusText = "The configured texture processor integration is not available.";
            return;
        }

        var modifiedCells = Cells.Where(cell => cell.IsModified).ToList();
        BusyTasks++;
        try {
            var savedCells = await Task.Run(async () => {
                var saved = new List<WaterCellVM>();
                foreach (var cell in modifiedCells) {
                    if (await _flowmapService.SaveFlowmap(cell.Flowmap, cell.Position, processor, processorConfig.ConfigValues)) {
                        saved.Add(cell);
                    }
                }

                return saved;
            });

            foreach (var cell in savedCells) {
                cell.IsModified = false;
            }

            StatusText = savedCells.Count == modifiedCells.Count
                ? $"Saved {savedCells.Count} flowmap{(savedCells.Count == 1 ? "" : "s")}."
                : $"Saved {savedCells.Count} of {modifiedCells.Count} flowmaps, see log for errors.";
        } finally {
            BusyTasks--;
        }
    }
}
