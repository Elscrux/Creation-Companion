using System.Runtime.InteropServices;
using Noggog;
namespace CreationEditor.Avalonia.Services.Viewport.BSE;

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

    public struct TextureSet {
        public string? Diffuse;
        public string? Normal;
        public string? Specular;
        public string? EnvironmentMask;
        public string? Height;
        public string? Environment;
        public string? Multilayer;
        public string? Emissive;
    }

    public struct CornerSets {
        public Quadrant TopRight;
        public Quadrant BottomRight;
        public Quadrant TopLeft;
        public Quadrant BottomLeft;
    }

    public struct BaseLayer {
        public TextureSet TextureSet;
    }

    public struct AlphaData {
        public float Opacity;
        public ushort Position;
    }

    public struct AlphaLayer {
        public TextureSet TextureSet;
        public IntPtr Data; // max 289 (pass by pointer) 
        public ushort DataLength;
    }

    public struct Quadrant {
        public BaseLayer BaseLayer;
        public IntPtr AlphaLayers; // max 8 layers (pass by pointer)
        public byte AlphaLayersLength;
    }

    public struct TerrainInfo {
        public float X; // Local editor space offset
        public float Y; // Local editor space offset
        public ulong PointSize; // Length of one cell = 33

        public ulong PositionBegin; // first index of the positional data in buffer of this cell
        // normal data and color data is passed as 3 floats per point to build the corresponding vec
        public ulong NormalBegin; // first index of the normal data in buffer of this cell
        public ulong ColorBegin; // first index of the color data in buffer of this cell

        public CornerSets CornerSets;
    }

    public struct InitConfig {
        public uint Version;
        public string AssetDirectory;
        public ulong SizeOfWindowHandles;
        public IntPtr[] WindowHandles;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoadCallback(ulong count, ReferenceLoad[] load);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UpdateCallback(ulong count, ReferenceUpdate[] load);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void HideCallback(ulong count, string[] keys, bool hide);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DeleteCallback(ulong count, string[] keys);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SelectCallback(ulong count, IntPtr keys);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TerrainAddCallback(uint count, TerrainInfo[] info, float[] buffer);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addLoadCallback(LoadCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addUpdateCallback(UpdateCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addHideCallback(HideCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addDeleteCallback(DeleteCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addSelectCallback(SelectCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool addTerrainCallback(TerrainAddCallback callback);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void loadReferences(ulong count, ReferenceLoad[] load);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool updateReferences(ulong count, ReferenceUpdate[] keys);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool hideReferences(ulong count, string[] formKeys, bool hide);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool deleteReferences(ulong count, string[] formKeys);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool loadTerrain(uint count, TerrainInfo[] info, float[] buffer);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int initTGEditor(InitConfig config, string[] formKeys, ulong count);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool isFinished();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void waitFinishedInit();
}
