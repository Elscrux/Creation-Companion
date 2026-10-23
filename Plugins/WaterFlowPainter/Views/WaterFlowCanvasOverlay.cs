using System.Reactive;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Rendering;
using Avalonia.Threading;
using Noggog;
using ReactiveUI;
using SkiaSharp;
using WaterFlowPainter.Model;
using WaterFlowPainter.Services;
using WaterFlowPainter.ViewModels;
namespace WaterFlowPainter.Views;

/// <summary>
/// Interaction overlay for the worldspace canvas. Paints the flowmap of the cell under the
/// pointer directly on the canvas, animates the water flow of cells that are rendered larger
/// than a threshold on screen and draws the brush cursor. Must be sized to the composite image size.
/// </summary>
public sealed class WaterFlowCanvasOverlay : Control, ICustomHitTest, IDisposable {
    private const int FlowmapSize = FlowmapData.Size;

    /// <summary>Minimum rendered cell size in screen pixels for the particle simulation to run.</summary>
    private const double SimulationCellThreshold = 50;

    private static readonly TimeSpan FrameTime = TimeSpan.FromMilliseconds(33);
    private static readonly FlowmapRenderer Renderer = new();

    private readonly CompositeDisposable _disposables = new();
    private readonly DispatcherTimer _timer;
    private WaterFlowPainterVM? _viewModel;
    private ZoomBorder? _zoomBorder;

    private readonly Dictionary<P2Int, (WaterCellVM Cell, FlowParticleSimulator.Simulation Simulation)> _simulations = new();
    private readonly Dictionary<P2Int, Bitmap> _cellImages = new();
    private readonly Dictionary<P2Int, Bitmap> _landMaskImages = new();
    private bool _isPainting;
    private Point? _lastPaintPosition;
    private Point _pointerPosition;
    private WaterCellVM? _hoveredCell;

    public WaterFlowCanvasOverlay() {
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.None);

        _timer = new DispatcherTimer(FrameTime, DispatcherPriority.Render, (_, _) => OnFrame());
        DetachedFromVisualTree += (_, _) => _timer.Stop();
    }

    protected override void OnDataContextChanged(EventArgs e) {
        base.OnDataContextChanged(e);
        _disposables.Clear();
        _timer.Stop();
        _viewModel = DataContext as WaterFlowPainterVM;
        if (_viewModel is null) return;

        InvalidateVisual();

        _viewModel.EditInvalidated += OnEditInvalidated;
        _disposables.Add(Disposable.Create(() => _viewModel.EditInvalidated -= OnEditInvalidated));

        _disposables.Add(_viewModel.WhenAnyValue(
                x => x.CompositeWidth,
                x => x.CompositeHeight,
                x => x.OverviewCellSize,
                x => x.LeftCell,
                x => x.TopCell,
                (_, _, _, _, _) => Unit.Default)
            .Subscribe(_ => {
                UpdateSimulations();
                InvalidateVisual();
            }));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);

        _zoomBorder = FindZoomBorder();
        if (_zoomBorder is not null) {
            _zoomBorder.ZoomChanged += OnZoomChanged;
            UpdateSimulations();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnDetachedFromVisualTree(e);

        if (_zoomBorder is not null) {
            _zoomBorder.ZoomChanged -= OnZoomChanged;
            _zoomBorder = null;
        }
    }

    private void OnZoomChanged(object? sender, ZoomChangedEventArgs e) => UpdateSimulations();

    private ZoomBorder? FindZoomBorder() {
        Visual? current = this;
        while (current is not null) {
            if (current is ZoomBorder zoomBorder) return zoomBorder;

            current = current.Parent as Visual;
        }

        return null;
    }

    private void OnEditInvalidated(object? sender, WaterCellVM cell) {
        RefreshCellImage(cell);
        InvalidateVisual();
    }

    private void OnFrame() {
        if (_viewModel is null) return;

        foreach (var (_, simulation) in _simulations.Values) {
            _viewModel.ParticleSimulator.Tick(simulation, (float) FrameTime.TotalSeconds);
        }

        InvalidateVisual();
    }

    /// <summary>
    /// Starts the simulation of the cells that are visible on screen and rendered larger than the
    /// threshold, and stops the others.
    /// </summary>
    private void UpdateSimulations() {
        if (_viewModel is null) return;

        var zoomBorder = _zoomBorder ?? FindZoomBorder();
        if (zoomBorder is null) return;

        // The zoom border scales the canvas, so the rendered cell size is the layout cell size
        // multiplied by the scale factor of the render transform
        var scale = GetCanvasScale();
        var renderedCellSize = _viewModel.OverviewCellSize * scale;
        if (renderedCellSize < SimulationCellThreshold) {
            StopAllSimulations();
            return;
        }

        // Map the viewport of the zoom border into canvas coordinates to find the visible cells.
        // The zoom border transforms the canvas with viewport = matrix * canvas + layoutOffset.
        Visual? canvas = _zoomBorder?.Child;
        var matrix = canvas?.RenderTransform?.Value ?? Matrix.Identity;
        var layoutOffset = canvas?.Bounds.Position ?? default;
        var inverse = matrix.Invert();
        var viewportSize = zoomBorder.Bounds.Size;
        var topLeft = inverse.Transform(new Point(-layoutOffset.X, -layoutOffset.Y));
        var bottomRight = inverse.Transform(new Point(viewportSize.Width - layoutOffset.X, viewportSize.Height - layoutOffset.Y));

        var cellPixelSize = _viewModel.OverviewCellSize;
        var minCellX = _viewModel.LeftCell + (int) Math.Floor(Math.Min(topLeft.X, bottomRight.X) / cellPixelSize);
        var maxCellX = _viewModel.LeftCell + (int) Math.Floor(Math.Max(topLeft.X, bottomRight.X) / cellPixelSize);
        // Canvas Y grows southwards, cell Y grows northwards
        var maxCellY = _viewModel.TopCell - (int) Math.Floor(Math.Min(topLeft.Y, bottomRight.Y) / cellPixelSize);
        var minCellY = _viewModel.TopCell - (int) Math.Floor(Math.Max(topLeft.Y, bottomRight.Y) / cellPixelSize);

        var visiblePositions = new HashSet<P2Int>();
        foreach (var cell in _viewModel.Cells) {
            if (cell.Position.X >= minCellX
             && cell.Position.X <= maxCellX
             && cell.Position.Y >= minCellY
             && cell.Position.Y <= maxCellY) {
                visiblePositions.Add(cell.Position);
                if (!_simulations.ContainsKey(cell.Position)) {
                    _simulations[cell.Position] = (cell, _viewModel.ParticleSimulator.Create(cell.Flowmap));
                }
            }
        }

        foreach (var position in _simulations.Keys.ToList()) {
            if (!visiblePositions.Contains(position)) {
                _simulations[position].Simulation.Dispose();
                _simulations.Remove(position);
            }
        }

        if (_simulations.Count > 0) {
            if (!_timer.IsEnabled) _timer.Start();
        } else {
            _timer.Stop();
        }
    }

    private void StopAllSimulations() {
        foreach (var (_, simulation) in _simulations.Values) {
            simulation.Dispose();
        }

        _simulations.Clear();
        _timer.Stop();
    }

    /// <summary>Gets the scale factor the zoom border applies to the canvas.</summary>
    private double GetCanvasScale() {
        // Walk up to the zoom border and derive the scale from the render transform of the parent
        Visual? current = this;
        while (current is not null) {
            if (current.Parent is Visual { RenderTransform: { Value.M11: > 0 } transform }) {
                return transform.Value.M11;
            }

            current = current.Parent as Visual;
        }

        return 1;
    }

    /// <summary>Renders the cell's flowmap color coded into a cached bitmap, so painting on the canvas is visible immediately.</summary>
    private void RefreshCellImage(WaterCellVM cell) {
        var size = _viewModel?.OverviewCellSize ?? FlowmapRenderer.FlowmapPixelSize;
        if (_cellImages.Remove(cell.Position, out var oldImage)) {
            oldImage.Dispose();
        }

        var bitmap = new SKBitmap(size, size, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        using (var canvas = new SKCanvas(bitmap)) {
            Renderer.DrawFlowmap(canvas, cell.Flowmap, 0, 0, false, size);
        }

        using var stream = FlowmapRenderer.EncodePng(bitmap);
        _cellImages[cell.Position] = new Bitmap(stream);
        bitmap.Dispose();
    }

    /// <summary>Gets the cached land mask image of a cell, creating it on first use.</summary>
    private Bitmap? GetLandMaskImage(WaterCellVM cell) {
        if (cell.LandMask is null) return null;
        if (_landMaskImages.TryGetValue(cell.Position, out var cached)) return cached;

        using var stream = FlowmapRenderer.EncodePng(cell.LandMask);
        var image = new Bitmap(stream);
        _landMaskImages[cell.Position] = image;
        return image;
    }

    public override void Render(DrawingContext context) {
        base.Render(context);
        if (_viewModel is null) return;

        var cellPixelSize = _viewModel.OverviewCellSize;

        // Land masks of the zoomed in cells, shown together with the particle animations
        if (_simulations.Count > 0) {
            foreach (var (position, entry) in _simulations) {
                if (GetLandMaskImage(entry.Cell) is not {} mask) continue;

                context.DrawImage(mask, CellRect(position, cellPixelSize));
            }
        }

        // Live flowmaps of painted cells, drawn over the static preview
        foreach (var (position, image) in _cellImages) {
            var rect = CellRect(position, cellPixelSize);
            context.DrawImage(image, rect);
        }

        // Particle animations of zoomed in cells
        foreach (var (position, entry) in _simulations) {
            var rect = CellRect(position, cellPixelSize);
            context.DrawImage(entry.Simulation.Bitmap, rect);
        }

        // Brush cursor
        var radius = _viewModel.BrushSize / FlowmapSize * cellPixelSize;
        var brushPen = new Pen(Brushes.White, 1.5);
        context.DrawEllipse(null, brushPen, _pointerPosition, radius, radius);
    }

    private Rect CellRect(P2Int position, int cellPixelSize) {
        var x = (position.X - _viewModel!.LeftCell) * cellPixelSize;
        var y = (_viewModel.TopCell - position.Y) * cellPixelSize;
        return new Rect(x, y, cellPixelSize, cellPixelSize);
    }

    /// <summary>Gets the cell at the given position in control coordinates, or null if there is none.</summary>
    private WaterCellVM? GetCellAt(Point position) {
        if (_viewModel is null) return null;

        var cellPixelSize = _viewModel.OverviewCellSize;
        var cellX = _viewModel.LeftCell + (int) Math.Floor(position.X / cellPixelSize);
        var cellY = _viewModel.TopCell - (int) Math.Floor(position.Y / cellPixelSize);
        return _viewModel.Cells.FirstOrDefault(c => c.Position.X == cellX && c.Position.Y == cellY);
    }

    /// <summary>
    /// Paints the brush area at the given position into every cell the brush touches, so large
    /// brushes paint seamlessly across cell borders.
    /// </summary>
    private void PaintBrush(Point position, double direction) {
        if (_viewModel is null) return;

        var cellPixelSize = _viewModel.OverviewCellSize;
        var radiusCanvas = _viewModel.BrushSize * cellPixelSize / (double) FlowmapSize;

        var minCellX = _viewModel.LeftCell + (int) Math.Floor((position.X - radiusCanvas) / cellPixelSize);
        var maxCellX = _viewModel.LeftCell + (int) Math.Floor((position.X + radiusCanvas) / cellPixelSize);
        // Canvas Y grows southwards, cell Y grows northwards
        var maxCellY = _viewModel.TopCell - (int) Math.Floor((position.Y - radiusCanvas) / cellPixelSize);
        var minCellY = _viewModel.TopCell - (int) Math.Floor((position.Y + radiusCanvas) / cellPixelSize);

        foreach (var cell in _viewModel.Cells) {
            if (cell.Position.X < minCellX
             || cell.Position.X > maxCellX
             || cell.Position.Y < minCellY
             || cell.Position.Y > maxCellY) {
                continue;
            }

            PaintInCell(cell, position, direction);
        }
    }

    /// <summary>Maps a position in control coordinates to flowmap pixel coordinates within the given cell and paints there.</summary>
    private void PaintInCell(WaterCellVM cell, Point position, double direction) {
        if (_viewModel is null) return;

        var cellPixelSize = _viewModel.OverviewCellSize;
        var origin = CellRect(cell.Position, cellPixelSize).Position;
        var x = (position.X - origin.X) / cellPixelSize * FlowmapSize;
        // Flowmap Y grows northwards, the control's Y grows downwards
        var y = (1 - (position.Y - origin.Y) / cellPixelSize) * FlowmapSize;
        _viewModel.PaintAt(cell, x, y, direction);
    }

    /// <summary>
    /// Paints dabs along the segment from <paramref name="from" /> to <paramref name="to" />,
    /// spaced at half the brush radius so fast strokes don't leave gaps.
    /// </summary>
    private void PaintAlongSegment(Point from, Point to, double direction) {
        if (_viewModel is null) return;

        var cellPixelSize = _viewModel.OverviewCellSize;
        var spacing = Math.Max(1, _viewModel.BrushSize * 0.5 * cellPixelSize / (double) FlowmapSize);
        var dx = to.X - from.X;
        var dy = to.Y - from.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        var steps = Math.Max(1, (int) Math.Ceiling(distance / spacing));

        for (var i = 1; i <= steps; i++) {
            var t = i / (double) steps;
            PaintBrush(new Point(from.X + dx * t, from.Y + dy * t), direction);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e) {
        base.OnPointerPressed(e);
        if (_viewModel is null) return;
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;

        var position = e.GetCurrentPoint(this).Position;
        var cell = GetCellAt(position);
        if (cell is null) return;

        _isPainting = true;
        _lastPaintPosition = position;
        e.Pointer.Capture(this);
        _viewModel.BeginStroke(cell);

        // The direction brush needs a drag movement to define the direction, so it only paints on move
        if (_viewModel.BrushMode != FlowBrushMode.Direction) {
            PaintBrush(position, 0);
        }

        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e) {
        base.OnPointerMoved(e);
        if (_viewModel is null) return;

        var position = e.GetPosition(this);
        _pointerPosition = position;

        var cell = GetCellAt(position);
        if (cell != _hoveredCell) {
            _hoveredCell = cell;
            InvalidateVisual();
        }

        if (!_isPainting || cell is null || _lastPaintPosition is not {} last) return;

        var dx = position.X - last.X;
        var dy = position.Y - last.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        if (distance < 0.5) return;

        if (_viewModel.BrushMode == FlowBrushMode.Direction) {
            // The drag direction defines the flow direction. Screen Y grows southwards,
            // the flow direction is measured from east towards north, so invert Y.
            var direction = Math.Atan2(-dy, dx);
            PaintAlongSegment(last, position, direction);
        } else {
            PaintAlongSegment(last, position, 0);
        }

        _lastPaintPosition = position;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e) {
        base.OnPointerReleased(e);
        if (!_isPainting) return;

        _isPainting = false;
        _lastPaintPosition = null;
        e.Pointer.Capture(null);
        _viewModel?.EndStroke();
    }

    protected override void OnPointerExited(PointerEventArgs e) {
        base.OnPointerExited(e);
        _hoveredCell = null;
        InvalidateVisual();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e) {
        base.OnPointerWheelChanged(e);
        if (_viewModel is null) return;

        // Ctrl + scroll adjusts the brush size, other wheel input zooms the canvas
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;

        var step = Math.Max(1, (int) (_viewModel.BrushSize * 0.1));
        var delta = e.Delta.Y > 0 ? step : -step;
        _viewModel.BrushSize = Math.Clamp(_viewModel.BrushSize + delta, 1, 80);
        e.Handled = true;
    }

    /// <summary>Makes the control hit-testable, as it only draws in Render and has no background.</summary>
    public bool HitTest(Point point) => new Rect(Bounds.Size).Contains(point);

    public void Dispose() {
        _disposables.Dispose();
        _timer.Stop();
        foreach (var (_, simulation) in _simulations.Values) {
            simulation.Dispose();
        }

        _simulations.Clear();
        foreach (var image in _cellImages.Values) {
            image.Dispose();
        }

        _cellImages.Clear();
        foreach (var image in _landMaskImages.Values) {
            image.Dispose();
        }

        _landMaskImages.Clear();
    }
}
