using System.Runtime.InteropServices;
using CreationEditor.Services.Environment;
using Noggog;
namespace CreationEditor.Avalonia.Services.Viewport;

public static class Interop {
    private const string DllName = "TGInterOp.dll";

    public struct ReferenceTransform {
        public P3Float Translation;
        public P3Float Scale;
        public P3Float Rotations;
    }

    public struct ReferenceLoad {
        public string FormKey;
        public string Path;
        public ReferenceTransform Transform;

        public new string ToString() {
            return $"{FormKey} {Path}";
        }
    }

    public enum UpdateType {
        Transform,
        Path,
    }

    public struct ReferenceUpdate {
        public string FormKey;
        public UpdateType Update;
        public string Path;
    }
    
    public struct InitConfig {
        public uint Version;
        public string AssetDirectory;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoadCallback(uint count, ReferenceLoad[] load);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UpdateCallback (uint count, ReferenceUpdate[] load);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HideCallback (uint count, string[] keys, bool hide);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DeleteCallback (uint count, string[] keys, bool hide);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addLoadCallback(LoadCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addUpdateCallback(UpdateCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addHideCallback(HideCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addDeleteCallback(DeleteCallback  callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void loadReferences(uint count, ReferenceLoad[] load);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool updateReferences(uint count, ReferenceUpdate[] keys);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool hideReferences(uint count, string[] formKeys, bool hide);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool deleteReferences(uint count, string[] formKeys);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool initTGEditor(InitConfig count);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool isFinished();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void waitFinishedInit();

    public static async void Start(IEnvironmentContext environmentContext) {
        Task.Run(() => {
            initTGEditor(new InitConfig {
                Version = 1,
                AssetDirectory = @"E:\TES\Skyrim\vanilla-files\"//environmentContext.DataDirectoryProvider.Path
            });
        });
        
        addLoadCallback((callbackCount, callbackLoads) => {
            Console.WriteLine($"CALLBACK: {callbackCount}");
            if (callbackCount == 0) return;
            
            foreach (var load in callbackLoads) {
                Console.WriteLine($"CALLBACK: {load.ToString()}");
            }
        });

        waitFinishedInit();
    }
}
