using System.Runtime.InteropServices;
namespace Windows.Interop;

public static partial class WinHelper {
    private const string WindowsDll = "user32.dll";

    //Sets a window to be a child window of another window
    [LibraryImport(WindowsDll)]
    public static partial IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [LibraryImport(WindowsDll, EntryPoint = "MoveWindowA")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)] bool bRepaint);

    //Sets window attributes
    [LibraryImport(WindowsDll, EntryPoint = "SetWindowLongA")]
    public static partial int SetWindowLong(IntPtr hWnd, int nIndex, WinStyle dwNewLong);

    [LibraryImport(WindowsDll, EntryPoint = "GetWindowLongPtrA")]
    public static partial IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
    [LibraryImport(WindowsDll, EntryPoint = "SetWindowLongPtrA")]
    public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public const int StyleIndex = -16;

    [Flags]
    public enum WinStyle {
        Visible = 0x10000000,
        Child = 0x40000000, //child window
        Border = 0x00800000, //window with border
        Frame = 0x00400000, //window with double border but no title
        ResizeBar = 0x00040000, //resize window bar
        Caption = Border | Frame //window with a title bar
    }
}
