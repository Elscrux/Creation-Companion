using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxSkinBinding Signatire: 0x5a93f338 size: 128 flags: FLAGS_NONE

    // m_mesh m_class: hkxMesh Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_nodeNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_bindPose m_class:  Type.TYPE_ARRAY Type.TYPE_MATRIX4 arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_initSkinTransform m_class:  Type.TYPE_MATRIX4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkxSkinBinding : hkReferencedObject, IEquatable<hkxSkinBinding?>
    {
        public hkxMesh? m_mesh { set; get; }
        public IList<string> m_nodeNames { set; get; } = Array.Empty<string>();
        public IList<Matrix4x4> m_bindPose { set; get; } = Array.Empty<Matrix4x4>();
        public Matrix4x4 m_initSkinTransform { set; get; }

        public override uint Signature { set; get; } = 0x5a93f338;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_mesh = des.ReadClassPointer<hkxMesh>(br);
            m_nodeNames = des.ReadStringPointerArray(br);
            m_bindPose = des.ReadMatrix4Array(br);
            br.Position += 8;
            m_initSkinTransform = des.ReadMatrix4(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_mesh);
            s.WriteStringPointerArray(bw, m_nodeNames);
            s.WriteMatrix4Array(bw, m_bindPose);
            bw.Position += 8;
            s.WriteMatrix4(bw, m_initSkinTransform);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_mesh = xd.ReadClassPointer<hkxMesh>(xe, nameof(m_mesh));
            m_nodeNames = xd.ReadStringArray(xe, nameof(m_nodeNames));
            m_bindPose = xd.ReadMatrix4Array(xe, nameof(m_bindPose));
            m_initSkinTransform = xd.ReadMatrix4(xe, nameof(m_initSkinTransform));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_mesh), m_mesh);
            xs.WriteStringArray(xe, nameof(m_nodeNames), m_nodeNames);
            xs.WriteMatrix4Array(xe, nameof(m_bindPose), m_bindPose);
            xs.WriteMatrix4(xe, nameof(m_initSkinTransform), m_initSkinTransform);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxSkinBinding);
        }

        public bool Equals(hkxSkinBinding? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_mesh is null && other.m_mesh is null) || (m_mesh is not null && other.m_mesh is not null && m_mesh.Equals((IHavokObject)other.m_mesh))) &&
                   m_nodeNames.SequenceEqual(other.m_nodeNames) &&
                   m_bindPose.SequenceEqual(other.m_bindPose) &&
                   m_initSkinTransform.Equals(other.m_initSkinTransform) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_mesh);
            hashcode.Add(m_nodeNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_bindPose.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_initSkinTransform);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

