using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CreationEditor.Resources;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "deleteReferences", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool DeleteReferences_Native(ulong count, string[] formKeys);
    public static void DeleteReferences(FormKey[] formKeys) => DeleteReferences_Native((ulong) formKeys.Length, formKeys.ToStringArray());

    [LibraryImport(DllName, EntryPoint = "addDeleteCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddDeleteCallback(DeleteCallback callback);
    private static readonly List<DeleteCallback> DeleteCallbacks = [];
    public static IDisposable AddDeleteCallback(Action<FormKey[]> callback) {
        DeleteCallback deleteCallback = (count, keys) => callback(keys.ToFormKeyArray((int) count));
        DeleteCallbacks.Add(deleteCallback);
        AddDeleteCallback(deleteCallback);
        return new ActionDisposable(() => DeleteCallbacks.Remove(deleteCallback));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void DeleteCallback(ulong count, IntPtr keys);
}
