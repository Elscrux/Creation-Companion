using System.Xml.Linq;
namespace HKX2
{
    // hkHalf8 Signatire: 0x7684dc80 size: 16 flags: FLAGS_NONE

    // m_quad m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 8 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    public partial class hkHalf8 : IHavokObject, IEquatable<hkHalf8?>
    {
        public Half[] m_quad = new Half[8];

        public virtual uint Signature { set; get; } = 0x7684dc80;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_quad = des.ReadHalfCStyleArray(br, 8);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteHalfCStyleArray(bw, m_quad);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_quad = xd.ReadHalfCStyleArray(xe, nameof(m_quad), 8);
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloatArray(xe, nameof(m_quad), m_quad);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkHalf8);
        }

        public bool Equals(hkHalf8? other)
        {
            return other is not null &&
                   m_quad.SequenceEqual(other.m_quad) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_quad.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

