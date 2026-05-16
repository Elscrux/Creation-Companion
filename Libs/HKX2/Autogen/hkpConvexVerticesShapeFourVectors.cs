using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexVerticesShapeFourVectors Signatire: 0x3d80c5bf size: 48 flags: FLAGS_NONE

    // m_x m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_y m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_z m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpConvexVerticesShapeFourVectors : IHavokObject, IEquatable<hkpConvexVerticesShapeFourVectors?>
    {
        public Vector4 m_x { set; get; }
        public Vector4 m_y { set; get; }
        public Vector4 m_z { set; get; }

        public virtual uint Signature { set; get; } = 0x3d80c5bf;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_x = br.ReadVector4();
            m_y = br.ReadVector4();
            m_z = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_x);
            bw.WriteVector4(m_y);
            bw.WriteVector4(m_z);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_x = xd.ReadVector4(xe, nameof(m_x));
            m_y = xd.ReadVector4(xe, nameof(m_y));
            m_z = xd.ReadVector4(xe, nameof(m_z));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_x), m_x);
            xs.WriteVector4(xe, nameof(m_y), m_y);
            xs.WriteVector4(xe, nameof(m_z), m_z);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexVerticesShapeFourVectors);
        }

        public bool Equals(hkpConvexVerticesShapeFourVectors? other)
        {
            return other is not null &&
                   m_x.Equals(other.m_x) &&
                   m_y.Equals(other.m_y) &&
                   m_z.Equals(other.m_z) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_x);
            hashcode.Add(m_y);
            hashcode.Add(m_z);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

