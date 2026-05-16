using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMultiRayShapeRay Signatire: 0xffdc0b65 size: 32 flags: FLAGS_NONE

    // m_start m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_end m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpMultiRayShapeRay : IHavokObject, IEquatable<hkpMultiRayShapeRay?>
    {
        public Vector4 m_start { set; get; }
        public Vector4 m_end { set; get; }

        public virtual uint Signature { set; get; } = 0xffdc0b65;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_start = br.ReadVector4();
            m_end = br.ReadVector4();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_start);
            bw.WriteVector4(m_end);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_start = xd.ReadVector4(xe, nameof(m_start));
            m_end = xd.ReadVector4(xe, nameof(m_end));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_start), m_start);
            xs.WriteVector4(xe, nameof(m_end), m_end);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMultiRayShapeRay);
        }

        public bool Equals(hkpMultiRayShapeRay? other)
        {
            return other is not null &&
                   m_start.Equals(other.m_start) &&
                   m_end.Equals(other.m_end) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_start);
            hashcode.Add(m_end);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

