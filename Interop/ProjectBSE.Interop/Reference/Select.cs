using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CreationEditor.Resources;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "addSelectCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddSelectCallback(SelectCallback callback);
    private static readonly List<SelectCallback> SelectCallbacks = [];
    public static IDisposable AddSelectCallback(Action<FormKey[]> callback) {
        SelectCallback selectCallback = (count, keys) => callback(keys.ToFormKeyArray((int) count));
        SelectCallbacks.Add(selectCallback);
        AddSelectCallback(selectCallback);
        return new ActionDisposable(() => SelectCallbacks.Remove(selectCallback));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SelectCallback(ulong count, IntPtr keys);
}
