using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkaMeshBinding Signatire: 0x81d9950b size: 72 flags: FLAGS_NONE

    // m_mesh m_class: hkxMesh Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_originalSkeletonName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_skeleton m_class: hkaSkeleton Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_mappings m_class: hkaMeshBindingMapping Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_boneFromSkinMeshTransforms m_class:  Type.TYPE_ARRAY Type.TYPE_TRANSFORM arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    public partial class hkaMeshBinding : hkReferencedObject, IEquatable<hkaMeshBinding?>
    {
        public hkxMesh? m_mesh { set; get; }
        public string m_originalSkeletonName { set; get; } = "";
        public hkaSkeleton? m_skeleton { set; get; }
        public IList<hkaMeshBindingMapping> m_mappings { set; get; } = Array.Empty<hkaMeshBindingMapping>();
        public IList<Matrix4x4> m_boneFromSkinMeshTransforms { set; get; } = Array.Empty<Matrix4x4>();

        public override uint Signature { set; get; } = 0x81d9950b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_mesh = des.ReadClassPointer<hkxMesh>(br);
            m_originalSkeletonName = des.ReadStringPointer(br);
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
            m_mappings = des.ReadClassArray<hkaMeshBindingMapping>(br);
            m_boneFromSkinMeshTransforms = des.ReadTransformArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_mesh);
            s.WriteStringPointer(bw, m_originalSkeletonName);
            s.WriteClassPointer(bw, m_skeleton);
            s.WriteClassArray(bw, m_mappings);
            s.WriteTransformArray(bw, m_boneFromSkinMeshTransforms);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_mesh = xd.ReadClassPointer<hkxMesh>(xe, nameof(m_mesh));
            m_originalSkeletonName = xd.ReadString(xe, nameof(m_originalSkeletonName));
            m_skeleton = xd.ReadClassPointer<hkaSkeleton>(xe, nameof(m_skeleton));
            m_mappings = xd.ReadClassArray<hkaMeshBindingMapping>(xe, nameof(m_mappings));
            m_boneFromSkinMeshTransforms = xd.ReadTransformArray(xe, nameof(m_boneFromSkinMeshTransforms));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_mesh), m_mesh);
            xs.WriteString(xe, nameof(m_originalSkeletonName), m_originalSkeletonName);
            xs.WriteClassPointer(xe, nameof(m_skeleton), m_skeleton);
            xs.WriteClassArray(xe, nameof(m_mappings), m_mappings);
            xs.WriteTransformArray(xe, nameof(m_boneFromSkinMeshTransforms), m_boneFromSkinMeshTransforms);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaMeshBinding);
        }

        public bool Equals(hkaMeshBinding? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_mesh is null && other.m_mesh is null) || (m_mesh is not null && other.m_mesh is not null && m_mesh.Equals((IHavokObject)other.m_mesh))) &&
                   (m_originalSkeletonName is null && other.m_originalSkeletonName is null || m_originalSkeletonName == other.m_originalSkeletonName || m_originalSkeletonName is null && other.m_originalSkeletonName == "" || m_originalSkeletonName == "" && other.m_originalSkeletonName is null) &&
                   ((m_skeleton is null && other.m_skeleton is null) || (m_skeleton is not null && other.m_skeleton is not null && m_skeleton.Equals((IHavokObject)other.m_skeleton))) &&
                   m_mappings.SequenceEqual(other.m_mappings) &&
                   m_boneFromSkinMeshTransforms.SequenceEqual(other.m_boneFromSkinMeshTransforms) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_mesh);
            hashcode.Add(m_originalSkeletonName);
            hashcode.Add(m_skeleton);
            hashcode.Add(m_mappings.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_boneFromSkinMeshTransforms.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

