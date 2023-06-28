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
}
