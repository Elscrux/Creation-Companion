using System.Xml.Linq;
namespace HKX2
{
    // hkPackedVector3 Signatire: 0x9c16df5b size: 8 flags: FLAGS_NONE

    // m_values m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 4 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkPackedVector3 : IHavokObject, IEquatable<hkPackedVector3?>
    {
        public short[] m_values = new short[4];

        public virtual uint Signature { set; get; } = 0x9c16df5b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_values = des.ReadInt16CStyleArray(br, 4);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteInt16CStyleArray(bw, m_values);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_values = xd.ReadInt16CStyleArray(xe, nameof(m_values), 4);
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumberArray(xe, nameof(m_values), m_values);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkPackedVector3);
        }

        public bool Equals(hkPackedVector3? other)
        {
            return other is not null &&
                   m_values.SequenceEqual(other.m_values) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_values.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

