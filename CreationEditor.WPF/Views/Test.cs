namespace CreationEditor.WPF.Views;

public class Test {
    public struct vec3 {
        public float x;
        public float y;
        public float z;
    }

    public struct ReferenceTransform {
        public vec3 translation;
        public vec3 scale;
        public vec3 rotations;
    }

    public struct ReferenceLoad {
        public string formKey;
        public string path;
        public ReferenceTransform transform;
    }

    public enum UpdateType {
        TRANSFORM,
        PATH
    }

    public struct ReferenceUpdate {
        public UIntPtr formKey;
        public UpdateType update;
        public string path;
    }

    [System.Runtime.InteropServices.DllImport("TGInterOp.dll")]
    public static extern UIntPtr loadReferences(uint count, ReferenceLoad load);

    [System.Runtime.InteropServices.DllImport("TGInterOp.dll")]
    public static extern bool updateReferences(uint count, ReferenceUpdate keys);

    [System.Runtime.InteropServices.DllImport("TGInterOp.dll")]
    public static extern bool hideReferences(uint count, UIntPtr keys);

    [System.Runtime.InteropServices.DllImport("TGInterOp.dll")]
    public static extern bool deleteReferences(uint count, UIntPtr keys);

    public Test() {
        Console.WriteLine("Test123: " + loadReferences(123, new ReferenceLoad { formKey = "test", path = "test.nif", transform = new ReferenceTransform { translation = new vec3(), scale = new vec3(), rotations = new vec3() } }));
        Console.WriteLine("Test123: " + updateReferences(123, new ReferenceUpdate {formKey = new UIntPtr(), path = "test.nif", update = UpdateType.PATH}));
        Console.WriteLine("Test123: " + hideReferences(123, UIntPtr.MinValue));
        Console.WriteLine("Test123: " + deleteReferences(123, UIntPtr.MinValue));
    }
}
