using System.Xml.Linq;
namespace HKX2
{
    // hkbAttributeModifierAssignment Signatire: 0x48b8ad52 size: 8 flags: FLAGS_NONE

    // m_attributeIndex m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_attributeValue m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkbAttributeModifierAssignment : IHavokObject, IEquatable<hkbAttributeModifierAssignment?>
    {
        public int m_attributeIndex { set; get; }
        public float m_attributeValue { set; get; }

        public virtual uint Signature { set; get; } = 0x48b8ad52;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_attributeIndex = br.ReadInt32();
            m_attributeValue = br.ReadSingle();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_attributeIndex);
            bw.WriteSingle(m_attributeValue);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_attributeIndex = xd.ReadInt32(xe, nameof(m_attributeIndex));
            m_attributeValue = xd.ReadSingle(xe, nameof(m_attributeValue));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_attributeIndex), m_attributeIndex);
            xs.WriteFloat(xe, nameof(m_attributeValue), m_attributeValue);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbAttributeModifierAssignment);
        }

        public bool Equals(hkbAttributeModifierAssignment? other)
        {
            return other is not null &&
                   m_attributeIndex.Equals(other.m_attributeIndex) &&
                   m_attributeValue.Equals(other.m_attributeValue) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_attributeIndex);
            hashcode.Add(m_attributeValue);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

