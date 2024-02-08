using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
namespace ProjectBSE.Interop;

public static partial class Interop {
    [NativeMarshalling(typeof(TextureSetMarshaller))]
    public struct TextureSet {
        public string? Diffuse { get; set; }
        public string? Normal { get; set; }
        public string? Specular { get; set; }
        public string? EnvironmentMask { get; set; }
        public string? Height { get; set; }
        public string? Environment { get; set; }
        public string? Multilayer { get; set; }
        public string? Emissive { get; set; }
    }

    public static int TextureSetSize => Marshal.SizeOf<TextureSetMarshaller.TextureSetUnmanaged>();

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
}
