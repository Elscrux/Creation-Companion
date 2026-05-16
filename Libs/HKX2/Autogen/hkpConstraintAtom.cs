using System.Xml.Linq;
namespace HKX2
{
    // hkpConstraintAtom Signatire: 0x59d67ef6 size: 2 flags: FLAGS_NONE

    // m_type m_class:  Type.TYPE_ENUM Type.TYPE_UINT16 arrSize: 0 offset: 0 flags: FLAGS_NONE enum: AtomType
    public partial class hkpConstraintAtom : IHavokObject, IEquatable<hkpConstraintAtom?>
    {
        public ushort m_type { set; get; }

        public virtual uint Signature { set; get; } = 0x59d67ef6;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_type = br.ReadUInt16();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_type);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_type = xd.ReadFlag<AtomType, ushort>(xe, nameof(m_type));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteEnum<AtomType, ushort>(xe, nameof(m_type), m_type);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConstraintAtom);
        }

        public bool Equals(hkpConstraintAtom? other)
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

