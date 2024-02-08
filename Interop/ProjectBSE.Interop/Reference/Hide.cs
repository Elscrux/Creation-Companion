using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CreationEditor.Resources;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "hideReferences", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool HideReferences_Native(ulong count, string[] formKeys, [MarshalAs(UnmanagedType.Bool)] bool hide);
    public static void HideReferences(FormKey[] formKeys) => HideReferences_Native((ulong) formKeys.Length, formKeys.ToStringArray(), true);
    public static void ShowReferences(FormKey[] formKeys) => HideReferences_Native((ulong) formKeys.Length, formKeys.ToStringArray(), false);

    [LibraryImport(DllName, EntryPoint = "addHideCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddHideCallback(HideCallback callback);
    private static readonly List<HideCallback> HideCallbacks = [];
    public static IDisposable AddHideCallback(Action<FormKey[]> callback) {
        HideCallback hideCallback = (count, keys, hide) => {
            if (hide) callback(keys.ToFormKeyArray((int) count));
        };
        HideCallbacks.Add(hideCallback);
        AddHideCallback(hideCallback);
        return new ActionDisposable(() => HideCallbacks.Remove(hideCallback));
    }
    public static IDisposable AddShowCallback(Action<FormKey[]> callback) {
        HideCallback hideCallback = (count, keys, hide) => {
            if (!hide) callback(keys.ToFormKeyArray((int) count));
        };
        HideCallbacks.Add(hideCallback);
        AddHideCallback(hideCallback);
        return new ActionDisposable(() => HideCallbacks.Remove(hideCallback));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void HideCallback(ulong count, IntPtr keys, bool hide);
}
