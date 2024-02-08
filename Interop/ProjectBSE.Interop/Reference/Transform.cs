using System.Runtime.InteropServices;
using Noggog;
namespace ProjectBSE.Interop;

public static partial class Interop {
    public static int ReferenceTransformSize => Marshal.SizeOf<ReferenceTransform>();

    public struct ReferenceTransform {
        public P3Float Translation { get; set; }
        public P3Float Scale { get; set; }
        public P3Float Rotations { get; set; }
    }
}
