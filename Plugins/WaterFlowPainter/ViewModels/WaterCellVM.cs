using CreationEditor.Avalonia.ViewModels;
using Noggog;
using ReactiveUI.SourceGenerators;
using SkiaSharp;
using WaterFlowPainter.Model;
using WaterFlowPainter.Services;
namespace WaterFlowPainter.ViewModels;

/// <summary>A cell with flowmap enabled water and its loaded flowmap data.</summary>
public sealed partial class WaterCellVM : ViewModel {
    public P2Int Position { get; }
    public float WaterHeight { get; }

    public FlowmapData Flowmap { get; }

    /// <summary>Mask of the landscape above the water level, or null if the cell has no landscape.</summary>
    public SKBitmap? LandMask { get; }

    /// <summary>Snapshots of the flowmap taken before each stroke, used for undo.</summary>
    public Stack<FlowmapData> UndoStack { get; } = new();

    [Reactive] public partial bool IsModified { get; set; }

    public string DisplayName => $"Cell {Position.X}.{Position.Y}";

    public WaterCellVM(FlowmapService.FlowmapCell cell, FlowmapData flowmap, SKBitmap? landMask) {
        Position = cell.Position;
        WaterHeight = cell.WaterHeight;
        Flowmap = flowmap;
        LandMask = landMask;
    }
}
