using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Noggog;
namespace ProjectBSE.Interop;

public static partial class Interop {
    private const string DllName = "TGInterOp.dll";
    public const int CurrentVersion = 4;

    public struct ReferenceTransform {
        public P3Float Translation;
        public P3Float Scale;
        public P3Float Rotations;
    }

    [NativeMarshalling(typeof(ReferenceLoadMarshaller))]
    public struct ReferenceLoad {
        public string FormKey;
        public string Path;
        public ReferenceTransform Transform;

        public new string ToString() {
            return $"{FormKey} {Path}";
        }
    }

    [CustomMarshaller(typeof(ReferenceLoad), MarshalMode.Default, typeof(ReferenceLoadMarshaller))]
    internal static class ReferenceLoadMarshaller {
        public static ReferenceLoadUnmanaged ConvertToUnmanaged(ReferenceLoad managed) {
            return new ReferenceLoadUnmanaged {
                FormKey = Marshal.StringToCoTaskMemUTF8(managed.FormKey),
                Path = Marshal.StringToCoTaskMemUTF8(managed.Path),
                Transform = managed.Transform
            };
        }

        public static ReferenceLoad ConvertToManaged(ReferenceLoadUnmanaged unmanaged) {
            return new ReferenceLoad {
                FormKey = Marshal.PtrToStringUTF8(unmanaged.FormKey) ?? throw new Exception(),
                Path = Marshal.PtrToStringUTF8(unmanaged.Path) ?? throw new Exception(),
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

    public enum UpdateType {
        Transform,
        Path,
    }

    [NativeMarshalling(typeof(ReferenceUpdateMarshaller))]
    public struct ReferenceUpdate {
        public string FormKey;
        public UpdateType Update;
        public ReferenceTransform Transform;
        public string Path;
    }

    [CustomMarshaller(typeof(ReferenceUpdate), MarshalMode.Default, typeof(ReferenceUpdateMarshaller))]
    internal static class ReferenceUpdateMarshaller {
        public static ReferenceUpdateUnmanaged ConvertToUnmanaged(ReferenceUpdate managed) {
            return new ReferenceUpdateUnmanaged {
                FormKey = Marshal.StringToCoTaskMemUTF8(managed.FormKey),
                Update = managed.Update,
                Transform = managed.Transform,
                Path = Marshal.StringToCoTaskMemUTF8(managed.Path)
            };
        }

        public static ReferenceUpdate ConvertToManaged(ReferenceUpdateUnmanaged unmanaged) {
            return new ReferenceUpdate {
                FormKey = Marshal.PtrToStringUTF8(unmanaged.FormKey) ?? throw new Exception(),
                Update = unmanaged.Update,
                Transform = unmanaged.Transform,
                Path = Marshal.PtrToStringUTF8(unmanaged.Path) ?? throw new Exception()
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

    [NativeMarshalling(typeof(TextureSetMarshaller))]
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

    [CustomMarshaller(typeof(TextureSet), MarshalMode.Default, typeof(TextureSetMarshaller))]
    internal static class TextureSetMarshaller {
        public static TextureSetUnmanaged ConvertToUnmanaged(TextureSet managed)
            => new() {
                Diffuse = Marshal.StringToCoTaskMemUTF8(managed.Diffuse),
                Normal = Marshal.StringToCoTaskMemUTF8(managed.Normal),
                Specular = Marshal.StringToCoTaskMemUTF8(managed.Specular),
                EnvironmentMask = Marshal.StringToCoTaskMemUTF8(managed.EnvironmentMask),
                Height = Marshal.StringToCoTaskMemUTF8(managed.Height),
                Environment = Marshal.StringToCoTaskMemUTF8(managed.Environment),
                Multilayer = Marshal.StringToCoTaskMemUTF8(managed.Multilayer),
                Emissive = Marshal.StringToCoTaskMemUTF8(managed.Emissive),
            };

        public static TextureSet ConvertToManaged(TextureSetUnmanaged unmanaged)
            => new() {
                Diffuse = Marshal.PtrToStringUTF8(unmanaged.Diffuse),
                Normal = Marshal.PtrToStringUTF8(unmanaged.Normal),
                Specular = Marshal.PtrToStringUTF8(unmanaged.Specular),
                EnvironmentMask = Marshal.PtrToStringUTF8(unmanaged.EnvironmentMask),
                Height = Marshal.PtrToStringUTF8(unmanaged.Height),
                Environment = Marshal.PtrToStringUTF8(unmanaged.Environment),
                Multilayer = Marshal.PtrToStringUTF8(unmanaged.Multilayer),
                Emissive = Marshal.PtrToStringUTF8(unmanaged.Emissive),
            };

        public static void Free(TextureSetUnmanaged unmanaged) {
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Diffuse);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Normal);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Specular);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.EnvironmentMask);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Height);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Environment);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Multilayer);
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.Emissive);
        }

        internal struct TextureSetUnmanaged {
            public IntPtr Diffuse;
            public IntPtr Normal;
            public IntPtr Specular;
            public IntPtr EnvironmentMask;
            public IntPtr Height;
            public IntPtr Environment;
            public IntPtr Multilayer;
            public IntPtr Emissive;
        }
    }

    [NativeMarshalling(typeof(CornerSetsMarshaller))]
    public struct CornerSets {
        public Quadrant TopRight;
        public Quadrant BottomRight;
        public Quadrant TopLeft;
        public Quadrant BottomLeft;
    }

    [CustomMarshaller(typeof(CornerSets), MarshalMode.Default, typeof(CornerSetsMarshaller))]
    internal static class CornerSetsMarshaller {
        public static CornerSetsUnmanaged ConvertToUnmanaged(CornerSets managed) {
            return new CornerSetsUnmanaged {
                TopRight = QuadrantMarshaller.ConvertToUnmanaged(managed.TopRight),
                BottomRight = QuadrantMarshaller.ConvertToUnmanaged(managed.BottomRight),
                TopLeft = QuadrantMarshaller.ConvertToUnmanaged(managed.TopLeft),
                BottomLeft = QuadrantMarshaller.ConvertToUnmanaged(managed.BottomLeft)
            };
        }

        public static CornerSets ConvertToManaged(CornerSetsUnmanaged unmanaged) {
            return new CornerSets {
                TopRight = QuadrantMarshaller.ConvertToManaged(unmanaged.TopRight),
                BottomRight = QuadrantMarshaller.ConvertToManaged(unmanaged.BottomRight),
                TopLeft = QuadrantMarshaller.ConvertToManaged(unmanaged.TopLeft),
                BottomLeft = QuadrantMarshaller.ConvertToManaged(unmanaged.BottomLeft)
            };
        }

        public static void Free(CornerSetsUnmanaged unmanaged) {
            QuadrantMarshaller.Free(unmanaged.TopRight);
            QuadrantMarshaller.Free(unmanaged.BottomRight);
            QuadrantMarshaller.Free(unmanaged.TopLeft);
            QuadrantMarshaller.Free(unmanaged.BottomLeft);
        }

        internal struct CornerSetsUnmanaged {
            public QuadrantMarshaller.QuadrantUnmanaged TopRight;
            public QuadrantMarshaller.QuadrantUnmanaged BottomRight;
            public QuadrantMarshaller.QuadrantUnmanaged TopLeft;
            public QuadrantMarshaller.QuadrantUnmanaged BottomLeft;
        }
    }

    [NativeMarshalling(typeof(BaseLayerMarshaller))]
    public struct BaseLayer {
        public TextureSet TextureSet;
    }

    [CustomMarshaller(typeof(BaseLayer), MarshalMode.Default, typeof(BaseLayerMarshaller))]
    internal static class BaseLayerMarshaller {
        public static BaseLayerUnmanaged ConvertToUnmanaged(BaseLayer managed) {
            return new BaseLayerUnmanaged {
                TextureSet = TextureSetMarshaller.ConvertToUnmanaged(managed.TextureSet)
            };
        }

        public static BaseLayer ConvertToManaged(BaseLayerUnmanaged unmanaged) {
            return new BaseLayer {
                TextureSet = TextureSetMarshaller.ConvertToManaged(unmanaged.TextureSet)
            };
        }

        public static void Free(BaseLayerUnmanaged unmanaged) {
            TextureSetMarshaller.Free(unmanaged.TextureSet);
        }

        internal struct BaseLayerUnmanaged {
            public TextureSetMarshaller.TextureSetUnmanaged TextureSet;
        }
    }

    public struct AlphaData {
        public float Opacity;
        public ushort Position;
    }

    [NativeMarshalling(typeof(AlphaLayerMarshaller))]
    public struct AlphaLayer {
        public TextureSet TextureSet;
        public AlphaData[] Data; // max 289 (pass by pointer) 
        public ushort DataLength;
    }

    internal struct AlphaLayerUnmanaged {
        public TextureSetMarshaller.TextureSetUnmanaged TextureSet;
        public IntPtr Data;
        public ushort DataLength;
    }

    [CustomMarshaller(typeof(AlphaLayer), MarshalMode.Default, typeof(AlphaLayerMarshaller))]
    internal static class AlphaLayerMarshaller {
        public static AlphaLayerUnmanaged ConvertToUnmanaged(AlphaLayer managed) {
            return new AlphaLayerUnmanaged {
                TextureSet = TextureSetMarshaller.ConvertToUnmanaged(managed.TextureSet),
                Data = managed.Data.ToUnmanagedMemory(),
                DataLength = managed.DataLength
            };
        }

        public static AlphaLayer ConvertToManaged(AlphaLayerUnmanaged unmanaged) {
            return new AlphaLayer {
                TextureSet = TextureSetMarshaller.ConvertToManaged(unmanaged.TextureSet),
                Data = unmanaged.Data.ToArray<AlphaData>(unmanaged.DataLength),
                DataLength = unmanaged.DataLength
            };
        }

        public static void Free(AlphaLayerUnmanaged unmanaged) {
            Marshal.FreeCoTaskMem(unmanaged.Data);
            TextureSetMarshaller.Free(unmanaged.TextureSet);
        }
    }

    [NativeMarshalling(typeof(QuadrantMarshaller))]
    public struct Quadrant {
        public BaseLayer BaseLayer;
        public AlphaLayer[] AlphaLayers; // max 8 layers (pass by pointer)
        public byte AlphaLayersLength;
    }

    [CustomMarshaller(typeof(Quadrant), MarshalMode.Default, typeof(QuadrantMarshaller))]
    internal static class QuadrantMarshaller {
        public static unsafe QuadrantUnmanaged ConvertToUnmanaged(Quadrant managed) {
            return new QuadrantUnmanaged {
                BaseLayer = BaseLayerMarshaller.ConvertToUnmanaged(managed.BaseLayer),
                AlphaLayers = ArrayMarshaller<AlphaLayer, AlphaLayerUnmanaged>.AllocateContainerForUnmanagedElements(managed.AlphaLayers, out _),
                AlphaLayersLength = 0
            };
        }

        public static unsafe Quadrant ConvertToManaged(QuadrantUnmanaged unmanaged) {
            return new Quadrant {
                BaseLayer = BaseLayerMarshaller.ConvertToManaged(unmanaged.BaseLayer),
                AlphaLayers = ArrayMarshaller<AlphaLayer, AlphaLayerUnmanaged>.AllocateContainerForManagedElements(unmanaged.AlphaLayers, unmanaged.AlphaLayersLength) ?? throw new Exception(),
                AlphaLayersLength = unmanaged.AlphaLayersLength
            };
        }

        public static unsafe void Free(QuadrantUnmanaged unmanaged) {
            ArrayMarshaller<AlphaLayer, AlphaLayerUnmanaged>.Free(unmanaged.AlphaLayers);
        }

        internal struct QuadrantUnmanaged {
            public BaseLayerMarshaller.BaseLayerUnmanaged BaseLayer;
            public unsafe AlphaLayerUnmanaged* AlphaLayers; // max 8 layers (pass by pointer)
            public byte AlphaLayersLength;
        }
    }

    [NativeMarshalling(typeof(TerrainInfoMarshaller))]
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

    [CustomMarshaller(typeof(TerrainInfo), MarshalMode.Default, typeof(TerrainInfoMarshaller))]
    internal static class TerrainInfoMarshaller {
        public static TerrainInfoUnmanaged ConvertToUnmanaged(TerrainInfo managed) {
            return new TerrainInfoUnmanaged {
                X = managed.X,
                Y = managed.Y,
                PointSize = managed.PointSize,
                PositionBegin = managed.PositionBegin,
                NormalBegin = managed.NormalBegin,
                ColorBegin = managed.ColorBegin,
                CornerSets = CornerSetsMarshaller.ConvertToUnmanaged(managed.CornerSets)
            };
        }

        public static TerrainInfo ConvertToManaged(TerrainInfoUnmanaged unmanaged) {
            return new TerrainInfo {
                X = unmanaged.X,
                Y = unmanaged.Y,
                PointSize = unmanaged.PointSize,
                PositionBegin = unmanaged.PositionBegin,
                NormalBegin = unmanaged.NormalBegin,
                ColorBegin = unmanaged.ColorBegin,
                CornerSets = CornerSetsMarshaller.ConvertToManaged(unmanaged.CornerSets)
            };
        }

        public static void Free(TerrainInfoUnmanaged unmanaged) {
            CornerSetsMarshaller.Free(unmanaged.CornerSets);
        }

        internal struct TerrainInfoUnmanaged {
            public float X;
            public float Y;
            public ulong PointSize;

            public ulong PositionBegin;
            public ulong NormalBegin;
            public ulong ColorBegin;

            public CornerSetsMarshaller.CornerSetsUnmanaged CornerSets;
        }
    }

    [NativeMarshalling(typeof(InitConfigMarshaller))]
    public struct InitConfig() {
        public uint Version = CurrentVersion;
        public string AssetDirectory = string.Empty;
        public ulong SizeOfWindowHandles = 0;
        public IntPtr[] WindowHandles = Array.Empty<nint>();
        public FeatureSet FeatureSet = new();
    }

    [CustomMarshaller(typeof(InitConfig), MarshalMode.Default, typeof(InitConfigMarshaller))]
    internal static class InitConfigMarshaller {
        public static InitConfigUnmanaged ConvertToUnmanaged(InitConfig managed) {
            var unmanaged = new InitConfigUnmanaged {
                Version = managed.Version,
                AssetDirectory = Marshal.StringToCoTaskMemAnsi(managed.AssetDirectory),
                SizeOfWindowHandles = (ulong) (managed.WindowHandles.Length * IntPtr.Size),
                WindowHandles = Marshal.AllocHGlobal(managed.WindowHandles.Length * IntPtr.Size)
            };
            Marshal.Copy(managed.WindowHandles, 0, unmanaged.WindowHandles, managed.WindowHandles.Length);
            return unmanaged;
        }

        public static InitConfig ConvertToManaged(InitConfigUnmanaged unmanaged) {
            var managed = new InitConfig {
                Version = unmanaged.Version,
                AssetDirectory = Marshal.PtrToStringAnsi(unmanaged.AssetDirectory) ?? throw new Exception(),
                SizeOfWindowHandles = unmanaged.SizeOfWindowHandles,
                WindowHandles = new IntPtr[unmanaged.SizeOfWindowHandles / (ulong) IntPtr.Size]
            };
            Marshal.Copy(unmanaged.WindowHandles, managed.WindowHandles, 0, managed.WindowHandles.Length);
            return managed;
        }

        public static void Free(InitConfigUnmanaged unmanaged) {
            Marshal.ZeroFreeCoTaskMemUTF8(unmanaged.AssetDirectory);
            Marshal.FreeCoTaskMem(unmanaged.WindowHandles);
        }

        internal struct InitConfigUnmanaged {
            public uint Version;
            public IntPtr AssetDirectory;
            public ulong SizeOfWindowHandles;
            public IntPtr WindowHandles;
        }
    }

    [NativeMarshalling(typeof(FeatureSetMarshaller))]
    public struct FeatureSet() {
        public bool WideLines = false;
        public int AnisotropicFiltering = int.MaxValue;
        public uint MipMapLevels = 4;
    }

    [CustomMarshaller(typeof(FeatureSet), MarshalMode.Default, typeof(FeatureSetMarshaller))]
    internal static class FeatureSetMarshaller {
        public static FeatureSetUnmanaged ConvertToUnmanaged(FeatureSet managed) {
            return new FeatureSetUnmanaged {
                WideLines = managed.WideLines,
                AnisotropicFiltering = managed.AnisotropicFiltering,
                MipMapLevels = managed.MipMapLevels
            };
        }

        public static FeatureSet ConvertToManaged(FeatureSetUnmanaged unmanaged) {
            return new FeatureSet {
                WideLines = unmanaged.WideLines,
                AnisotropicFiltering = unmanaged.AnisotropicFiltering,
                MipMapLevels = unmanaged.MipMapLevels
            };
        }

        public static void Free(FeatureSetUnmanaged unmanaged) {
        }

        internal struct FeatureSetUnmanaged {
            public bool WideLines;
            public int AnisotropicFiltering;
            public uint MipMapLevels;
        }
    }
    public struct SizeInformation {
        public long SizeInformationStruct;
        public long InitConfigStruct;
        public long ReferenceTransformStruct;
        public long ReferenceLoadStruct;
        public long ReferenceUpdateStruct;
        public long TextureSetStruct;
        public long AlphaDataStruct;
        public long AlphaLayerStruct;
        public long QuadrantStruct;
        public long CornerSetsStruct;
        public long TerrainInfoStruct;
        public long FeatureSetStruct;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LoadCallback(ulong count, [MarshalAs(UnmanagedType.Struct, MarshalTypeRef = typeof(ReferenceLoadMarshaller))] ReferenceLoad[] load);

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

    [LibraryImport(DllName, EntryPoint = "getSizeInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial SizeInformation GetSizeInfo();

    [LibraryImport(DllName, EntryPoint = "addLoadCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AddLoadCallback(LoadCallback callback);

    [LibraryImport(DllName, EntryPoint = "addUpdateCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AddUpdateCallback(UpdateCallback callback);

    [LibraryImport(DllName, EntryPoint = "addHideCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AddHideCallback(HideCallback callback);

    [LibraryImport(DllName, EntryPoint = "addDeleteCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AddDeleteCallback(DeleteCallback callback);

    [LibraryImport(DllName, EntryPoint = "addSelectCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AddSelectCallback(SelectCallback callback);

    [LibraryImport(DllName, EntryPoint = "addTerrainCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool AddTerrainCallback(TerrainAddCallback callback);

    [LibraryImport(DllName, EntryPoint = "loadReferences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void LoadReferences(ulong count, ReferenceLoad[] load);

    [LibraryImport(DllName, EntryPoint = "updateReferences")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UpdateReferences(ulong count, ReferenceUpdate[] keys);

    [LibraryImport(DllName, EntryPoint = "hideReferences", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool HideReferences(ulong count, string[] formKeys, [MarshalAs(UnmanagedType.Bool)] bool hide);

    [LibraryImport(DllName, EntryPoint = "deleteReferences", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DeleteReferences(ulong count, string[] formKeys);

    [LibraryImport(DllName, EntryPoint = "loadTerrain")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool LoadTerrain(uint count, TerrainInfo[] info, float[] buffer);

    [LibraryImport(DllName, EntryPoint = "initTGEditor", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int InitTGEditor(InitConfig config, string[] formKeys, ulong count);

    [LibraryImport(DllName, EntryPoint = "getMainWindowHandle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial IntPtr GetMainWindowHandle();

    [LibraryImport(DllName, EntryPoint = "isFinished")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsFinished();

    [LibraryImport(DllName, EntryPoint = "waitFinishedInit")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void WaitFinishedInit();
}
