using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxScene Signatire: 0x5f673ddd size: 224 flags: FLAGS_NONE

    // m_modeller m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_asset m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_sceneLength m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_rootNode m_class: hkxNode Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_selectionSets m_class: hkxNodeSelectionSet Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_cameras m_class: hkxCamera Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_lights m_class: hkxLight Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_meshes m_class: hkxMesh Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_materials m_class: hkxMaterial Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_inplaceTextures m_class: hkxTextureInplace Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_externalTextures m_class: hkxTextureFile Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_skinBindings m_class: hkxSkinBinding Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_appliedTransform m_class:  Type.TYPE_MATRIX3 Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    public partial class hkxScene : hkReferencedObject, IEquatable<hkxScene?>
    {
        public string m_modeller { set; get; } = "";
        public string m_asset { set; get; } = "";
        public float m_sceneLength { set; get; }
        public hkxNode? m_rootNode { set; get; }
        public IList<hkxNodeSelectionSet> m_selectionSets { set; get; } = Array.Empty<hkxNodeSelectionSet>();
        public IList<hkxCamera> m_cameras { set; get; } = Array.Empty<hkxCamera>();
        public IList<hkxLight> m_lights { set; get; } = Array.Empty<hkxLight>();
        public IList<hkxMesh> m_meshes { set; get; } = Array.Empty<hkxMesh>();
        public IList<hkxMaterial> m_materials { set; get; } = Array.Empty<hkxMaterial>();
        public IList<hkxTextureInplace> m_inplaceTextures { set; get; } = Array.Empty<hkxTextureInplace>();
        public IList<hkxTextureFile> m_externalTextures { set; get; } = Array.Empty<hkxTextureFile>();
        public IList<hkxSkinBinding> m_skinBindings { set; get; } = Array.Empty<hkxSkinBinding>();
        public Matrix4x4 m_appliedTransform { set; get; }

        public override uint Signature { set; get; } = 0x5f673ddd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_modeller = des.ReadStringPointer(br);
            m_asset = des.ReadStringPointer(br);
            m_sceneLength = br.ReadSingle();
            br.Position += 4;
            m_rootNode = des.ReadClassPointer<hkxNode>(br);
            m_selectionSets = des.ReadClassPointerArray<hkxNodeSelectionSet>(br);
            m_cameras = des.ReadClassPointerArray<hkxCamera>(br);
            m_lights = des.ReadClassPointerArray<hkxLight>(br);
            m_meshes = des.ReadClassPointerArray<hkxMesh>(br);
            m_materials = des.ReadClassPointerArray<hkxMaterial>(br);
            m_inplaceTextures = des.ReadClassPointerArray<hkxTextureInplace>(br);
            m_externalTextures = des.ReadClassPointerArray<hkxTextureFile>(br);
            m_skinBindings = des.ReadClassPointerArray<hkxSkinBinding>(br);
            m_appliedTransform = des.ReadMatrix3(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_modeller);
            s.WriteStringPointer(bw, m_asset);
            bw.WriteSingle(m_sceneLength);
            bw.Position += 4;
            s.WriteClassPointer(bw, m_rootNode);
            s.WriteClassPointerArray(bw, m_selectionSets);
            s.WriteClassPointerArray(bw, m_cameras);
            s.WriteClassPointerArray(bw, m_lights);
            s.WriteClassPointerArray(bw, m_meshes);
            s.WriteClassPointerArray(bw, m_materials);
            s.WriteClassPointerArray(bw, m_inplaceTextures);
            s.WriteClassPointerArray(bw, m_externalTextures);
            s.WriteClassPointerArray(bw, m_skinBindings);
            s.WriteMatrix3(bw, m_appliedTransform);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_modeller = xd.ReadString(xe, nameof(m_modeller));
            m_asset = xd.ReadString(xe, nameof(m_asset));
            m_sceneLength = xd.ReadSingle(xe, nameof(m_sceneLength));
            m_rootNode = xd.ReadClassPointer<hkxNode>(xe, nameof(m_rootNode));
            m_selectionSets = xd.ReadClassPointerArray<hkxNodeSelectionSet>(xe, nameof(m_selectionSets));
            m_cameras = xd.ReadClassPointerArray<hkxCamera>(xe, nameof(m_cameras));
            m_lights = xd.ReadClassPointerArray<hkxLight>(xe, nameof(m_lights));
            m_meshes = xd.ReadClassPointerArray<hkxMesh>(xe, nameof(m_meshes));
            m_materials = xd.ReadClassPointerArray<hkxMaterial>(xe, nameof(m_materials));
            m_inplaceTextures = xd.ReadClassPointerArray<hkxTextureInplace>(xe, nameof(m_inplaceTextures));
            m_externalTextures = xd.ReadClassPointerArray<hkxTextureFile>(xe, nameof(m_externalTextures));
            m_skinBindings = xd.ReadClassPointerArray<hkxSkinBinding>(xe, nameof(m_skinBindings));
            m_appliedTransform = xd.ReadMatrix3(xe, nameof(m_appliedTransform));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_modeller), m_modeller);
            xs.WriteString(xe, nameof(m_asset), m_asset);
            xs.WriteFloat(xe, nameof(m_sceneLength), m_sceneLength);
            xs.WriteClassPointer(xe, nameof(m_rootNode), m_rootNode);
            xs.WriteClassPointerArray(xe, nameof(m_selectionSets), m_selectionSets);
            xs.WriteClassPointerArray(xe, nameof(m_cameras), m_cameras);
            xs.WriteClassPointerArray(xe, nameof(m_lights), m_lights);
            xs.WriteClassPointerArray(xe, nameof(m_meshes), m_meshes);
            xs.WriteClassPointerArray(xe, nameof(m_materials), m_materials);
            xs.WriteClassPointerArray(xe, nameof(m_inplaceTextures), m_inplaceTextures);
            xs.WriteClassPointerArray(xe, nameof(m_externalTextures), m_externalTextures);
            xs.WriteClassPointerArray(xe, nameof(m_skinBindings), m_skinBindings);
            xs.WriteMatrix3(xe, nameof(m_appliedTransform), m_appliedTransform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxScene);
        }

        public bool Equals(hkxScene? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_modeller == other.m_modeller) &&
                   (m_asset == other.m_asset) &&
                   m_sceneLength.Equals(other.m_sceneLength) &&
                   ((m_rootNode is null && other.m_rootNode is null) || (m_rootNode is not null && other.m_rootNode is not null && m_rootNode.Equals((IHavokObject)other.m_rootNode))) &&
                   m_selectionSets.SequenceEqual(other.m_selectionSets) &&
                   m_cameras.SequenceEqual(other.m_cameras) &&
                   m_lights.SequenceEqual(other.m_lights) &&
                   m_meshes.SequenceEqual(other.m_meshes) &&
                   m_materials.SequenceEqual(other.m_materials) &&
                   m_inplaceTextures.SequenceEqual(other.m_inplaceTextures) &&
                   m_externalTextures.SequenceEqual(other.m_externalTextures) &&
                   m_skinBindings.SequenceEqual(other.m_skinBindings) &&
                   m_appliedTransform.Equals(other.m_appliedTransform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_modeller);
            hashcode.Add(m_asset);
            hashcode.Add(m_sceneLength);
            hashcode.Add(m_rootNode);
            hashcode.Add(m_selectionSets.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_cameras.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_lights.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_meshes.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_materials.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_inplaceTextures.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_externalTextures.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_skinBindings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_appliedTransform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

