using CreationEditor.Avalonia.ViewModels;
using Noggog;
using ReactiveUI.SourceGenerators;
using SkiaSharp;
using WaterFlowPainter.Model;
using WaterFlowPainter.Services;
namespace WaterFlowPainter.ViewModels;

/// <summary>A cell with water and its loaded flowmap data.</summary>
public sealed partial class WaterCellVM(FlowmapService.FlowmapCell cell, Func<FlowmapData> getFlowmap, Func<SKBitmap?> getLandMask) : ViewModel {
    public P2Int Position { get; } = cell.Position;
    public float WaterHeight { get; } = cell.WaterHeight;

    /// <summary>Whether the cell's water has the flowmap flag enabled and can be painted.</summary>
    public bool FlowmapEnabled { get; } = cell.FlowmapEnabled;

    public FlowmapData Flowmap => field ??= getFlowmap();

    /// <summary>Mask of the landscape above the water level, or null if the cell has no landscape.</summary>
    public SKBitmap? LandMask => field ??= getLandMask();

    /// <summary>Snapshots of the flowmap taken before each stroke, used for undo.</summary>
    public Stack<FlowmapData> UndoStack { get; } = new();

    [Reactive] public partial bool IsModified { get; set; }

    public string DisplayName => $"Cell {Position.X}.{Position.Y}";

}
