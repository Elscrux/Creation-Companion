using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using CreationEditor.Avalonia.Behavior;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Windowing;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Docking;

/// <summary>
/// Floating dock window: a real FluentAvalonia app window hosting a tabbed dock.
/// Native title bar with caption buttons, native resizing and maximizing.
/// When the title bar is dragged over the main window, the native drag is cancelled
/// and converted into a re-dock drag of all contained items, like common docking
/// frameworks handle floating windows.
/// </summary>
public partial class FloatingDockWindow : FAAppWindow {
    private readonly FloatingDockVM? _viewModel;
    private readonly DispatcherTimer _nativeDragTimer;
    private PixelPoint? _lastNativeDragPosition;
    private Rect _lastBounds;
    private DateTime _lastPositionChangeUtc;
    private DateTime _suppressNativeDragUntilUtc;
    private bool _redockRunning;

    public FloatingDockWindow() {
        InitializeComponent();

        _nativeDragTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _nativeDragTimer.Tick += OnNativeDragTimerTick;

        Opened += OnWindowOpened;
        Closed += OnWindowClosed;
    }

    public FloatingDockWindow(FloatingDockVM vm) : this() {
        _viewModel = vm;
        DataContext = vm;

        Position = vm.Position;
        Width = vm.Size.Width;
        Height = vm.Size.Height;

        vm.Content.WhenAnyValue(x => x.ActiveTab)
            .Subscribe(tab => vm.Title = tab?.Header ?? GetMainWindow()?.Title ?? string.Empty);
        vm.Title = vm.Content.ActiveTab?.Header ?? GetMainWindow()?.Title ?? string.Empty;

        // Keep the view model position and size in sync for layout persistence
        PositionChanged += (_, args) => {
            _lastPositionChangeUtc = DateTime.UtcNow;
            vm.Position = Position;
        };
        this.GetObservable(BoundsProperty)
            .Subscribe(bounds => vm.Size = new Size(bounds.Width, bounds.Height));
    }

    private void OnWindowOpened(object? sender, EventArgs e) {
        _nativeDragTimer.Start();
    }

    private void OnWindowClosed(object? sender, EventArgs e) {
        _nativeDragTimer.Stop();

        // Window was closed externally - make sure the view model doesn't keep a stale reference
        if (_viewModel is { Window: {} window } && window == this) {
            _viewModel.CleanUp();
        }
    }

    /// <summary>
    /// While the window is visible, check if a native title bar drag is in progress
    /// and the cursor moved over the main window. In that case the native drag is
    /// cancelled and converted into a re-dock drag.
    /// </summary>
    private void OnNativeDragTimerTick(object? sender, EventArgs e) {
        if (_redockRunning || _viewModel is null) return;

        // Converting a native drag requires cancelling it, which is platform specific.
        // Without support, tab drags can still be used to re-dock items.
        if (!CursorHelper.IsSupported) return;

        // Don't interfere with managed dock drags (e.g. dragging a tab of this window)
        if (DockDragContext.Current is not null) return;

        var mainWindow = GetMainWindow();
        if (mainWindow is null) return;

        // The title bar is the region that starts native window drags - use its position
        // instead of the cursor, which has no OS independent API. The platform updates the
        // window position during native drags, so this check needs no OS interop.
        var titleBarPoint = this.PointToScreen(new Point(Width / 2, 13));
        if (!CursorHelper.IsInsideWindow(mainWindow, titleBarPoint)) {
            _lastNativeDragPosition = null;
            return;
        }

        // Require that the window was actually dragged recently, so a simple
        // press + mouse over of the main window doesn't trigger a conversion
        if (DateTime.UtcNow - _lastPositionChangeUtc > TimeSpan.FromMilliseconds(500)
         || DateTime.UtcNow < _suppressNativeDragUntilUtc) {
            _lastNativeDragPosition = null;
            return;
        }

        // Exclude native resizes: while resizing the window size changes, while moving it stays
        if (_lastBounds != Bounds) {
            _lastBounds = Bounds;
            _lastNativeDragPosition = null;
            return;
        }

        if (_lastNativeDragPosition is null) {
            _lastNativeDragPosition = titleBarPoint;
            return;
        }

        if (_lastNativeDragPosition == titleBarPoint) return;

        _lastNativeDragPosition = null;

        _ = ConvertNativeDragToRedockDrag(_viewModel, mainWindow);
    }

    /// <summary>
    /// Cancels the running native window drag and converts it into a re-dock drag
    /// of all contained items.
    /// </summary>
    private async Task ConvertNativeDragToRedockDrag(FloatingDockVM vm, Window mainWindow) {
        var items = vm.Content.Tabs.ToList();
        if (items.Count == 0) return;

        _redockRunning = true;
        _suppressNativeDragUntilUtc = DateTime.UtcNow + TimeSpan.FromSeconds(1);
        try {
            // Cancel the native move loop, give it a moment to unwind
            CursorHelper.CancelWindowMove(this);
            await Task.Delay(50);

            var activeItem = vm.Content.ActiveTab ?? items[0];
            var dragData = new DockDragData {
                Item = activeItem,
                Items = items,
            };

            // Hide the window so the drop targets of the main window can be reached
            Hide();

            await RunRedockDragAsync(vm, dragData, mainWindow);
        } finally {
            _redockRunning = false;
            _suppressNativeDragUntilUtc = DateTime.UtcNow + TimeSpan.FromSeconds(1);
        }
    }

    private static Window? GetMainWindow() {
        return Application.Current?.ApplicationLifetime switch {
            IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow,
            _ => null,
        };
    }

    /// <summary>
    /// Starts the OLE drag with the given items and follows the cursor with the window
    /// whenever it leaves the main window, so the drag can be continued anywhere.
    /// </summary>
    private async Task RunRedockDragAsync(FloatingDockVM vm, DockDragData dragData, Window mainWindow) {
        var savedPosition = Position;

        vm.DockingManager.SetEditMode(true);
        try {
            using (DockDragContext.Begin(dragData)) {
                // While the drag runs, the window follows the cursor whenever it leaves
                // the main window, like common docking frameworks handle floating windows
                var following = false;
                var followTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
                followTimer.Tick += (_, _) => {
                    var cursor = CursorHelper.GetCursorScreenPosition();
                    if (cursor is null) return;

                    if (CursorHelper.IsInsideWindow(mainWindow, cursor.Value)) {
                        if (following) {
                            following = false;
                            Hide();
                        }
                        return;
                    }

                    Position = PlaceTitleUnderCursor(cursor.Value);
                    if (!following) {
                        following = true;
                        Show();
                    }
                };
                followTimer.Start();

                // Synthetic pointer args for the drag source, the physical mouse state
                // is tracked by the OS drag loop itself
                var pointer = new Pointer(1, PointerType.Mouse, true);
                var properties = new PointerPointProperties(
                    RawInputModifiers.LeftMouseButton,
                    PointerUpdateKind.LeftButtonPressed);
                var args = new PointerPressedEventArgs(
                    this,
                    pointer,
                    this,
                    new Point(0, 0),
                    (ulong) Environment.TickCount,
                    properties,
                    KeyModifiers.None);

                var result = await DragDrop.DoDragDropAsync(args, IDataTransfer.Create(dragData), DragDropEffects.Move);
                followTimer.Stop();

                if (result != DragDropEffects.None) {
                    // Items were re-docked into a drop target. If the window was following
                    // the cursor (e.g. dropped into another floating dock), hide it - the
                    // drop handler empties this floating dock and cleans it up.
                    if (following) {
                        Hide();
                    }

                    return;
                }

                // Drag was released without a drop target - show the window again,
                // under the cursor if it was released outside of the main window
                var cursorAfter = CursorHelper.GetCursorScreenPosition();
                if (cursorAfter is not null && !CursorHelper.IsInsideWindow(mainWindow, cursorAfter.Value)) {
                    Position = PlaceTitleUnderCursor(cursorAfter.Value);
                    vm.Position = Position;
                } else {
                    Position = savedPosition;
                }

                Show();
            }
        } finally {
            vm.DockingManager.SetEditMode(false);
        }
    }

    /// <summary>
    /// Positions the window so that its title bar is centered under the given screen position.
    /// </summary>
    private PixelPoint PlaceTitleUnderCursor(PixelPoint cursor) {
        // Convert the device independent offsets to pixel offsets for multi DPI setups
        var origin = this.PointToScreen(new Point(0, 0));
        var halfWidth = this.PointToScreen(new Point(Width / 2, 0));
        var titleOffset = this.PointToScreen(new Point(0, 13));

        return new PixelPoint(
            cursor.X - (halfWidth.X - origin.X),
            cursor.Y - (titleOffset.Y - origin.Y));
    }

}
