using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using CreationEditor.Resources;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "updateReferences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UpdateReferences_Native(ulong count, ReferenceUpdate[] keys);
    public static void UpdateReferences(ReferenceUpdate[] keys) => UpdateReferences_Native((ulong) keys.Length, keys);

    [LibraryImport(DllName, EntryPoint = "addUpdateCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddUpdateCallback(UpdateCallback callback);
    private static readonly List<UpdateCallback> UpdateCallbacks = [];
    public static IDisposable AddUpdateCallback(Action<ReferenceUpdate[]> callback) {
        unsafe {
            UpdateCallback updateCallback = (count, updates) => {
                callback(InteropUtility.ToArray(updates, (int) count, ReferenceUpdateMarshaller.ConvertToManaged));
            };
            UpdateCallbacks.Add(updateCallback);
            AddUpdateCallback(updateCallback);
            return new ActionDisposable(() => UpdateCallbacks.Remove(updateCallback));
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void UpdateCallback(ulong count, ReferenceUpdateMarshaller.ReferenceUpdateUnmanaged* load);

    public enum UpdateType {
        Transform,
        Path,
    }

    #region ReferenceUpdate
    [NativeMarshalling(typeof(ReferenceUpdateMarshaller))]
    public struct ReferenceUpdate {
        public FormKey FormKey { get; set; }
        public UpdateType Update { get; set; }
        public ReferenceTransform Transform { get; set; }
        public string Path { get; set; }
    }

    public static int ReferenceUpdateSize => Marshal.SizeOf<ReferenceUpdateMarshaller.ReferenceUpdateUnmanaged>();

    [CustomMarshaller(typeof(ReferenceUpdate), MarshalMode.Default, typeof(ReferenceUpdateMarshaller))]
    internal static class ReferenceUpdateMarshaller {
        public static ReferenceUpdateUnmanaged ConvertToUnmanaged(ReferenceUpdate managed) {
            return new ReferenceUpdateUnmanaged {
                FormKey = Marshal.StringToCoTaskMemUTF8(managed.FormKey.ToString()),
                Update = managed.Update,
                Transform = managed.Transform,
                Path = Marshal.StringToCoTaskMemUTF8(managed.Path)
            };
        }

        public static ReferenceUpdate ConvertToManaged(ReferenceUpdateUnmanaged unmanaged) {
            return new ReferenceUpdate {
                FormKey = FormKey.Factory(Marshal.PtrToStringUTF8(unmanaged.FormKey) ?? throw new InvalidCastException("FormKey could not be converted to string")),
                Update = unmanaged.Update,
                Transform = unmanaged.Transform,
                Path = Marshal.PtrToStringUTF8(unmanaged.Path) ?? throw new InvalidCastException("Path could not be converted to string")
            };
        }

        public static void Free(ReferenceUpdateUnmanaged unmanaged) {
            Marshal.FreeCoTaskMem(unmanaged.FormKey);
            Marshal.FreeCoTaskMem(unmanaged.Path);
        }

        internal struct ReferenceUpdateUnmanaged {
            public IntPtr FormKey;
            public UpdateType Update;
            public ReferenceTransform Transform;
            public IntPtr Path;
        }
    }
    #endregion
}
