using System.Reactive.Disposables;
using System.Runtime.InteropServices;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

public static class InteropUtility {
    public static string?[] ToStringArray(this IntPtr pointer, int count) {
        var result = new string?[count];

        for (var i = 0; i < count; i++) {
            var ptr = Marshal.ReadIntPtr(pointer, i * IntPtr.Size);
            result[i] = Marshal.PtrToStringAnsi(ptr);
        }

        return result;
    }

    public static IDisposable ToUnmanagedMemory<T>(this IList<T> list, out IntPtr pointer) where T : notnull {
        var sizeOf = Marshal.SizeOf<Interop.AlphaLayer>();
        pointer = Marshal.AllocHGlobal(sizeOf * list.Count);
        for (var i = 0; i < list.Count; i++) {
            var itemPtr = new IntPtr(pointer + (i * sizeOf));
            Marshal.StructureToPtr(list[i], itemPtr, false);
        }

        var clearPointer = pointer;
        return Disposable.Create(() => Marshal.FreeHGlobal(clearPointer));
    }
}
