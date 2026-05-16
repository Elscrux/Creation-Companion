using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpPlaneShape Signatire: 0xc36bbd30 size: 80 flags: FLAGS_NONE

    // m_plane m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_aabbCenter m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_aabbHalfExtents m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpPlaneShape : hkpHeightFieldShape, IEquatable<hkpPlaneShape?>
    {
        public Vector4 m_plane { set; get; }
        public Vector4 m_aabbCenter { set; get; }
        public Vector4 m_aabbHalfExtents { set; get; }

        public override uint Signature { set; get; } = 0xc36bbd30;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_plane = br.ReadVector4();
            m_aabbCenter = br.ReadVector4();
            m_aabbHalfExtents = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_plane);
            bw.WriteVector4(m_aabbCenter);
            bw.WriteVector4(m_aabbHalfExtents);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_plane = xd.ReadVector4(xe, nameof(m_plane));
            m_aabbCenter = xd.ReadVector4(xe, nameof(m_aabbCenter));
            m_aabbHalfExtents = xd.ReadVector4(xe, nameof(m_aabbHalfExtents));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_plane), m_plane);
            xs.WriteVector4(xe, nameof(m_aabbCenter), m_aabbCenter);
            xs.WriteVector4(xe, nameof(m_aabbHalfExtents), m_aabbHalfExtents);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPlaneShape);
        }

        public bool Equals(hkpPlaneShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_plane.Equals(other.m_plane) &&
                   m_aabbCenter.Equals(other.m_aabbCenter) &&
                   m_aabbHalfExtents.Equals(other.m_aabbHalfExtents) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_plane);
            hashcode.Add(m_aabbCenter);
            hashcode.Add(m_aabbHalfExtents);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

