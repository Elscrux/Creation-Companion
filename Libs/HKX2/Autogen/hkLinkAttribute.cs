using System.Xml.Linq;
namespace HKX2
{
    // hkLinkAttribute Signatire: 0x255d8164 size: 1 flags: FLAGS_NONE

    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 0 flags: FLAGS_NONE enum: Link
    public partial class hkLinkAttribute : IHavokObject, IEquatable<hkLinkAttribute?>
    {
        public sbyte m_type { set; get; }

        public virtual uint Signature { set; get; } = 0x255d8164;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_type = br.ReadSByte();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSByte(m_type);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_type = xd.ReadFlag<Link, sbyte>(xe, nameof(m_type));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteEnum<Link, sbyte>(xe, nameof(m_type), m_type);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkLinkAttribute);
        }

        public bool Equals(hkLinkAttribute? other)
        {
            return other is not null &&
                   m_type.Equals(other.m_type) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_type);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

