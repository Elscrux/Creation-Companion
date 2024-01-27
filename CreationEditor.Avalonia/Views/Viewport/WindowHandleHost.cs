using System.Reflection;
using Windows.Interop;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
namespace CreationEditor.Avalonia.Views.Viewport;

public sealed class WindowHandleHost(IntPtr WindowHandle, string descriptor) : NativeControlHost {
    private Window? _rootWindow;

    private static readonly MethodInfo DestroyNativeControl = typeof(NativeControlHost).GetMethod("DestroyNativeControl", BindingFlags.Instance | BindingFlags.NonPublic)!;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        if (e.Root is not Window window) return;

        _rootWindow = window;

        var handle = _rootWindow.TryGetPlatformHandle();
        if (handle is null) return;

        if (OperatingSystem.IsWindows()) {
            WinHelper.SetParent(WindowHandle, handle.Handle);

            long style = WinHelper.GetWindowLongPtr(WindowHandle, WinHelper.StyleIndex);
            style &= (long) ~WinHelper.WinStyle.ResizeBar;
            style &= (long) ~WinHelper.WinStyle.Caption;

            WinHelper.SetWindowLongPtr(WindowHandle, WinHelper.StyleIndex, (nint) style);
        }

        DestroyNativeControl.Invoke(this, null);

        base.OnAttachedToVisualTree(e);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnDetachedFromVisualTree(e);

        _rootWindow = null;
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent) {
        switch (Environment.OSVersion.Platform) {
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.Win32NT:
            case PlatformID.WinCE:
                return new PlatformHandle(WindowHandle, descriptor);
            case PlatformID.Unix:
                break;
            case PlatformID.Xbox:
                break;
            case PlatformID.MacOSX:
                break;
            case PlatformID.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return base.CreateNativeControlCore(parent);
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control) {
        switch (Environment.OSVersion.Platform) {
            case PlatformID.Win32S:
            case PlatformID.Win32Windows:
            case PlatformID.Win32NT:
            case PlatformID.WinCE:
                break;
            case PlatformID.Unix:
                break;
            case PlatformID.Xbox:
                break;
            case PlatformID.MacOSX:
                break;
            case PlatformID.Other:
                break;
            default:
                base.DestroyNativeControlCore(control);
                break;
        }
    }
}
