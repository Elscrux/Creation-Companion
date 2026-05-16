using System.Xml.Linq;
namespace HKX2
{
    // hkModelerNodeTypeAttribute Signatire: 0x338c092f size: 1 flags: FLAGS_NONE

    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 0 flags: FLAGS_NONE enum: ModelerType
    public partial class hkModelerNodeTypeAttribute : IHavokObject, IEquatable<hkModelerNodeTypeAttribute?>
    {
        public sbyte m_type { set; get; }

        public virtual uint Signature { set; get; } = 0x338c092f;

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
            m_type = xd.ReadFlag<ModelerType, sbyte>(xe, nameof(m_type));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteEnum<ModelerType, sbyte>(xe, nameof(m_type), m_type);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkModelerNodeTypeAttribute);
        }

        public bool Equals(hkModelerNodeTypeAttribute? other)
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

