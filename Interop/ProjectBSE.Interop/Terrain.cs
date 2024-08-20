using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using CreationEditor.Resources;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [LibraryImport(DllName, EntryPoint = "loadTerrain")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool LoadTerrain_Native(uint count, TerrainInfo[] info, float[] buffer);
    public static void LoadTerrain(TerrainInfo[] info, float[] buffer) => LoadTerrain_Native((uint) info.Length, info, buffer);

    [LibraryImport(DllName, EntryPoint = "addTerrainCallback")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool AddTerrainCallback(TerrainAddCallback callback);
    private static readonly List<TerrainAddCallback> TerrainAddCallbacks = [];
    public static IDisposable AddTerrainCallback(Action<TerrainInfo[], float[]> callback, int bufferSize) {
        unsafe {
            TerrainAddCallback terrainAddCallback = (count, terrain, buffer) => {
                callback(
                    InteropUtility.ToArray(terrain, (int) count, TerrainInfoMarshaller.ConvertToManaged),
                    InteropUtility.ToArray(buffer, bufferSize));
            };
            TerrainAddCallbacks.Add(terrainAddCallback);
            AddTerrainCallback(terrainAddCallback);
            return new ActionDisposable(() => TerrainAddCallbacks.Remove(terrainAddCallback));
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private unsafe delegate void TerrainAddCallback(uint count, TerrainInfoMarshaller.TerrainInfoUnmanaged* info, float* buffer);

    #region CornerSets
    [NativeMarshalling(typeof(CornerSetsMarshaller))]
    public struct CornerSets {
        public Quadrant TopRight;
        public Quadrant BottomRight;
        public Quadrant TopLeft;
        public Quadrant BottomLeft;
    }

    public static int CornerSetsSize => Marshal.SizeOf<CornerSetsMarshaller.CornerSetsUnmanaged>();

    [CustomMarshaller(typeof(CornerSets), MarshalMode.Default, typeof(CornerSetsMarshaller))]
    internal static class CornerSetsMarshaller {
        public static CornerSetsUnmanaged ConvertToUnmanaged(CornerSets managed) {
            return new CornerSetsUnmanaged {
                TopRight = QuadrantMarshaller.ConvertToUnmanaged(managed.TopRight),
                BottomRight = QuadrantMarshaller.ConvertToUnmanaged(managed.BottomRight),
                TopLeft = QuadrantMarshaller.ConvertToUnmanaged(managed.TopLeft),
                BottomLeft = QuadrantMarshaller.ConvertToUnmanaged(managed.BottomLeft),
            };
        }

        public static CornerSets ConvertToManaged(CornerSetsUnmanaged unmanaged) {
            return new CornerSets {
                TopRight = QuadrantMarshaller.ConvertToManaged(unmanaged.TopRight),
                BottomRight = QuadrantMarshaller.ConvertToManaged(unmanaged.BottomRight),
                TopLeft = QuadrantMarshaller.ConvertToManaged(unmanaged.TopLeft),
                BottomLeft = QuadrantMarshaller.ConvertToManaged(unmanaged.BottomLeft),
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
    #endregion

    #region BaseLayer
    [NativeMarshalling(typeof(BaseLayerMarshaller))]
    public struct BaseLayer {
        public TextureSet TextureSet { get; set; }
    }

    [CustomMarshaller(typeof(BaseLayer), MarshalMode.Default, typeof(BaseLayerMarshaller))]
    internal static class BaseLayerMarshaller {
        public static BaseLayerUnmanaged ConvertToUnmanaged(BaseLayer managed) {
            return new BaseLayerUnmanaged {
                TextureSet = TextureSetMarshaller.ConvertToUnmanaged(managed.TextureSet),
            };
        }

        public static BaseLayer ConvertToManaged(BaseLayerUnmanaged unmanaged) {
            return new BaseLayer {
                TextureSet = TextureSetMarshaller.ConvertToManaged(unmanaged.TextureSet),
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
        public float Opacity { get; set; }
        public ushort Position { get; set; }
    }

    public static int AlphaDataSize => Marshal.SizeOf<AlphaData>();
    #endregion

    #region AlphaLayer
    [NativeMarshalling(typeof(AlphaLayerMarshaller))]
    public struct AlphaLayer {
        public TextureSet TextureSet { get; set; }
        public AlphaData[] Data { get; set; } // max 289 (pass by pointer) 
        public ushort DataLength { get; set; }
    }

    public static int AlphaLayerSize => Marshal.SizeOf<AlphaLayerMarshaller.AlphaLayerUnmanaged>();

    [CustomMarshaller(typeof(AlphaLayer), MarshalMode.Default, typeof(AlphaLayerMarshaller))]
    internal static class AlphaLayerMarshaller {
        public static AlphaLayerUnmanaged ConvertToUnmanaged(AlphaLayer managed) {
            return new AlphaLayerUnmanaged {
                TextureSet = TextureSetMarshaller.ConvertToUnmanaged(managed.TextureSet),
                Data = managed.Data.ToUnmanagedMemory(),
                DataLength = managed.DataLength,
            };
        }

        public static AlphaLayer ConvertToManaged(AlphaLayerUnmanaged unmanaged) {
            return new AlphaLayer {
                TextureSet = TextureSetMarshaller.ConvertToManaged(unmanaged.TextureSet),
                Data = unmanaged.Data.ToArray<AlphaData>(unmanaged.DataLength),
                DataLength = unmanaged.DataLength,
            };
        }

        public static void Free(AlphaLayerUnmanaged unmanaged) {
            Marshal.FreeCoTaskMem(unmanaged.Data);
            TextureSetMarshaller.Free(unmanaged.TextureSet);
        }

        internal struct AlphaLayerUnmanaged {
            public TextureSetMarshaller.TextureSetUnmanaged TextureSet;
            public IntPtr Data;
            public ushort DataLength;
        }
    }
    #endregion

    #region Quadrant
    [NativeMarshalling(typeof(QuadrantMarshaller))]
    public struct Quadrant {
        public BaseLayer BaseLayer { get; set; }
        public AlphaLayer[] AlphaLayers { get; set; } // max 8 layers (pass by pointer)
    }

    public static int QuadrantSize => Marshal.SizeOf<QuadrantMarshaller.QuadrantUnmanaged>();

    [CustomMarshaller(typeof(Quadrant), MarshalMode.Default, typeof(QuadrantMarshaller))]
    internal static class QuadrantMarshaller {
        public static unsafe QuadrantUnmanaged ConvertToUnmanaged(Quadrant managed) {
            return new QuadrantUnmanaged {
                BaseLayer = BaseLayerMarshaller.ConvertToUnmanaged(managed.BaseLayer),
                AlphaLayers = ArrayMarshaller<AlphaLayer, AlphaLayerMarshaller.AlphaLayerUnmanaged>.AllocateContainerForUnmanagedElements(
                    managed.AlphaLayers,
                    out _),
                AlphaLayersLength = (byte) managed.AlphaLayers.Length,
            };
        }

        public static unsafe Quadrant ConvertToManaged(QuadrantUnmanaged unmanaged) {
            return new Quadrant {
                BaseLayer = BaseLayerMarshaller.ConvertToManaged(unmanaged.BaseLayer),
                AlphaLayers =
                    ArrayMarshaller<AlphaLayer, AlphaLayerMarshaller.AlphaLayerUnmanaged>.AllocateContainerForManagedElements(
                        unmanaged.AlphaLayers,
                        unmanaged.AlphaLayersLength) ?? throw new ArgumentException("AlphaLayers is null"),
            };
        }

        public static unsafe void Free(QuadrantUnmanaged unmanaged) {
            ArrayMarshaller<AlphaLayer, AlphaLayerMarshaller.AlphaLayerUnmanaged>.Free(unmanaged.AlphaLayers);
        }

        internal struct QuadrantUnmanaged {
            public BaseLayerMarshaller.BaseLayerUnmanaged BaseLayer;
            public unsafe AlphaLayerMarshaller.AlphaLayerUnmanaged* AlphaLayers; // max 8 layers (pass by pointer)
            public byte AlphaLayersLength;
        }
    }
    #endregion

    #region TerrainInfo
    [NativeMarshalling(typeof(TerrainInfoMarshaller))]
    public struct TerrainInfo {
        public float X { get; set; } // Local editor space offset
        public float Y { get; set; } // Local editor space offset
        public ulong PointSize { get; set; } // Length of one cell = 33

        public ulong PositionBegin { get; set; } // first index of the positional data in buffer of this cell
        // normal data and color data is passed as 3 floats per point to build the corresponding vec
        public ulong NormalBegin { get; set; } // first index of the normal data in buffer of this cell
        public ulong ColorBegin { get; set; } // first index of the color data in buffer of this cell

        public CornerSets CornerSets { get; set; }
    }

    public static int TerrainInfoSize => Marshal.SizeOf<TerrainInfoMarshaller.TerrainInfoUnmanaged>();

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
                CornerSets = CornerSetsMarshaller.ConvertToUnmanaged(managed.CornerSets),
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
                CornerSets = CornerSetsMarshaller.ConvertToManaged(unmanaged.CornerSets),
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
    #endregion
}
