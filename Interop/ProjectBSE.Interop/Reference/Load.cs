using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using CreationEditor.Resources;
using Mutagen.Bethesda.Plugins;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "loadReferences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void LoadReferences_Native(ulong count, ReferenceLoad[] load);
    public static void LoadReferences(ReferenceLoad[] load) => LoadReferences_Native((ulong) load.Length, load);

    [LibraryImport(DllName, EntryPoint = "addLoadCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddLoadCallback(LoadCallback callback);
    private static readonly List<LoadCallback> LoadCallbacks = [];
    public static IDisposable AddLoadCallback(Action<ReferenceLoad[]> callback) {
        unsafe {
            LoadCallback loadCallback = (count, loaded) => {
                callback(InteropUtility.ToArray(loaded, (int) count, ReferenceLoadMarshaller.ConvertToManaged));
            };
            LoadCallbacks.Add(loadCallback);
            AddLoadCallback(loadCallback);
            return new ActionDisposable(() => LoadCallbacks.Remove(loadCallback));
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void LoadCallback(ulong count, ReferenceLoadMarshaller.ReferenceLoadUnmanaged* loaded);

    #region ReferenceLoad
    [NativeMarshalling(typeof(ReferenceLoadMarshaller))]
    public struct ReferenceLoad {
        public FormKey FormKey { get; set; }
        public string Path { get; set; }
        public ReferenceTransform Transform { get; set; }

        public new string ToString() {
            return $"{FormKey} {Path}";
        }
    }

    public static int ReferenceLoadSize => Marshal.SizeOf<ReferenceLoadMarshaller.ReferenceLoadUnmanaged>();

    [CustomMarshaller(typeof(ReferenceLoad), MarshalMode.Default, typeof(ReferenceLoadMarshaller))]
    internal static class ReferenceLoadMarshaller {
        public static ReferenceLoadUnmanaged ConvertToUnmanaged(ReferenceLoad managed) {
            return new ReferenceLoadUnmanaged {
                FormKey = Marshal.StringToCoTaskMemUTF8(managed.FormKey.ToString()),
                Path = Marshal.StringToCoTaskMemUTF8(managed.Path),
                Transform = managed.Transform
            };
        }

        public static ReferenceLoad ConvertToManaged(ReferenceLoadUnmanaged unmanaged) {
            return new ReferenceLoad {
                FormKey = FormKey.Factory(Marshal.PtrToStringUTF8(unmanaged.FormKey) ?? throw new InvalidCastException("FormKey could not be converted to string")),
                Path = Marshal.PtrToStringUTF8(unmanaged.Path) ?? throw new InvalidCastException("Path could not be converted to string"),
                Transform = unmanaged.Transform
            };
        }

        public static void Free(ReferenceLoadUnmanaged unmanaged) {
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.FormKey);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Path);
        }

        internal struct ReferenceLoadUnmanaged {
            public IntPtr FormKey;
            public IntPtr Path;
            public ReferenceTransform Transform;
        }
    }
    #endregion
}
