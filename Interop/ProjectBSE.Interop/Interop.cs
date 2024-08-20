using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
namespace ProjectBSE.Interop;

public static partial class Interop {
    private const string DllName = "TGInterOp.dll";
    public const int CurrentVersion = 4;

    [LibraryImport(DllName, EntryPoint = "initTGEditor", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int InitTGEditor_Native(InitConfig config, string[] bsaFileNames, ulong count);
    public static int InitTGEditor(InitConfig config, string[] bsaFileNames) {
        return InitTGEditor_Native(config, bsaFileNames, (ulong) bsaFileNames.Length);
    }

    [LibraryImport(DllName, EntryPoint = "getMainWindowHandle")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial IntPtr GetMainWindowHandle_Native();
    public static IntPtr GetMainWindowHandle() => GetMainWindowHandle_Native();

    [LibraryImport(DllName, EntryPoint = "isFinished")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsFinished_Native();
    public static bool IsFinished() => IsFinished_Native();

    [LibraryImport(DllName, EntryPoint = "waitFinishedInit")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void WaitFinishedInit_Native();
    public static void WaitFinishedInit() => WaitFinishedInit_Native();

    #region InitConfig
    [NativeMarshalling(typeof(InitConfigMarshaller))]
    public struct InitConfig() {
        public uint Version { get; set; } = CurrentVersion;
        public string AssetDirectory { get; set; } = string.Empty;
        public IntPtr[] WindowHandles { get; set; } = [];
        public FeatureSet FeatureSet { get; set; } = new();
    }

    public static int InitConfigSize => Marshal.SizeOf<InitConfigMarshaller.InitConfigUnmanaged>();

    [CustomMarshaller(typeof(InitConfig), MarshalMode.Default, typeof(InitConfigMarshaller))]
    internal static class InitConfigMarshaller {
        public static InitConfigUnmanaged ConvertToUnmanaged(InitConfig managed) {
            var unmanaged = new InitConfigUnmanaged {
                Version = managed.Version,
                AssetDirectory = Marshal.StringToCoTaskMemAnsi(managed.AssetDirectory),
                SizeOfWindowHandles = (ulong) (managed.WindowHandles.Length * IntPtr.Size),
                WindowHandles = Marshal.AllocHGlobal(managed.WindowHandles.Length * IntPtr.Size),
            };
            Marshal.Copy(managed.WindowHandles, 0, unmanaged.WindowHandles, managed.WindowHandles.Length);
            return unmanaged;
        }

        public static InitConfig ConvertToManaged(InitConfigUnmanaged unmanaged) {
            var managed = new InitConfig {
                Version = unmanaged.Version,
                AssetDirectory = Marshal.PtrToStringAnsi(unmanaged.AssetDirectory)
                 ?? throw new InvalidCastException("AssetDirectory could not be converted to string"),
                WindowHandles = new IntPtr[unmanaged.SizeOfWindowHandles / (ulong) IntPtr.Size],
                FeatureSet = FeatureSetMarshaller.ConvertToManaged(unmanaged.FeatureSet),
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
            public FeatureSetMarshaller.FeatureSetUnmanaged FeatureSet;
        }
    }
    #endregion

    #region FeatureSet
    [NativeMarshalling(typeof(FeatureSetMarshaller))]
    public struct FeatureSet() {
        public uint WideLines { get; set; } = 0;
        public uint AnisotropicFiltering { get; set; } = uint.MaxValue;
        public uint MipMapLevels { get; set; } = 4;
    }

    public static int FeatureSetSize => Marshal.SizeOf<FeatureSetMarshaller.FeatureSetUnmanaged>();

    [CustomMarshaller(typeof(FeatureSet), MarshalMode.Default, typeof(FeatureSetMarshaller))]
    internal static class FeatureSetMarshaller {
        public static FeatureSetUnmanaged ConvertToUnmanaged(FeatureSet managed) {
            return new FeatureSetUnmanaged {
                WideLines = managed.WideLines,
                AnisotropicFiltering = managed.AnisotropicFiltering,
                MipMapLevels = managed.MipMapLevels,
            };
        }

        public static FeatureSet ConvertToManaged(FeatureSetUnmanaged unmanaged) {
            return new FeatureSet {
                WideLines = unmanaged.WideLines,
                AnisotropicFiltering = unmanaged.AnisotropicFiltering,
                MipMapLevels = unmanaged.MipMapLevels,
            };
        }

        public static void Free(FeatureSetUnmanaged unmanaged) {
            // Nothing to free
        }

        internal struct FeatureSetUnmanaged {
            public uint WideLines;
            public uint AnisotropicFiltering;
            public uint MipMapLevels;
        }
    }
    #endregion

    #region SizeInformation
    [LibraryImport(DllName, EntryPoint = "getSizeInfo")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial SizeInformation GetSizeInfo_Native();
    public static SizeInformation GetSizeInfo() => GetSizeInfo_Native();

    public static int SizeInformationSize => Marshal.SizeOf<SizeInformation>();

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
    #endregion
}
