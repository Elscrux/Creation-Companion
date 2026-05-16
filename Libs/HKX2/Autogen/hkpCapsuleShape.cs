using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpCapsuleShape Signatire: 0xdd0b1fd3 size: 80 flags: FLAGS_NONE

    // m_vertexA m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_vertexB m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpCapsuleShape : hkpConvexShape, IEquatable<hkpCapsuleShape?>
    {
        public Vector4 m_vertexA { set; get; }
        public Vector4 m_vertexB { set; get; }

        public override uint Signature { set; get; } = 0xdd0b1fd3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_vertexA = br.ReadVector4();
            m_vertexB = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            bw.WriteVector4(m_vertexA);
            bw.WriteVector4(m_vertexB);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_vertexA = xd.ReadVector4(xe, nameof(m_vertexA));
            m_vertexB = xd.ReadVector4(xe, nameof(m_vertexB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_vertexA), m_vertexA);
            xs.WriteVector4(xe, nameof(m_vertexB), m_vertexB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCapsuleShape);
        }

        public bool Equals(hkpCapsuleShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_vertexA.Equals(other.m_vertexA) &&
                   m_vertexB.Equals(other.m_vertexB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_vertexA);
            hashcode.Add(m_vertexB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

