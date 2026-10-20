using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
namespace CreationEditor.Avalonia.Views.Docking;

// TODO: Replace with cross platform solution using Avalonia when it's supported or some other library
/// <summary>
/// Helpers for screen space queries needed by the docking drag system.
/// All platform interop is centralized here.
/// </summary>
public static class CursorHelper {
    /// <summary>
    /// Indicates if the platform interop is available on the current OS,
    /// e.g. for features that have no OS independent alternative.
    /// </summary>
    public static bool IsSupported => OperatingSystem.IsWindows();

    /// <summary>
    /// Current position of the cursor in screen coordinates, if available on this platform.
    /// </summary>
    public static PixelPoint? GetCursorScreenPosition() {
        if (!IsSupported) return null;
        if (!GetCursorPos(out var point)) return null;

        return new PixelPoint(point.X, point.Y);
    }

    /// <summary>
    /// Checks if the given screen position is inside the bounds of the top level window.
    /// </summary>
    public static bool IsInsideWindow(TopLevel topLevel, PixelPoint screenPosition) {
        var topLeft = topLevel.PointToScreen(new Point(0, 0));
        var bottomRight = topLevel.PointToScreen(new Point(topLevel.Bounds.Width, topLevel.Bounds.Height));

        return screenPosition.X >= topLeft.X
         && screenPosition.X < bottomRight.X
         && screenPosition.Y >= topLeft.Y
         && screenPosition.Y < bottomRight.Y;
    }

    /// <summary>
    /// Cancels a running native window move (title bar drag) of the given window.
    /// No-op on platforms without support.
    /// </summary>
    public static void CancelWindowMove(TopLevel topLevel) {
        if (!IsSupported) return;
        if (topLevel.TryGetPlatformHandle() is not {} handle) return;

        PostMessage(handle.Handle, WmCancelMode, IntPtr.Zero, IntPtr.Zero);
        PostMessage(handle.Handle, WmKeyDown, VkEscape, IntPtr.Zero);
        PostMessage(handle.Handle, WmKeyUp, VkEscape, IntPtr.Zero);
    }

    private const uint WmCancelMode = 0x001F;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const int VkEscape = 0x1B;

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out NativePoint point);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint {
        public int X;
        public int Y;
    }
}
