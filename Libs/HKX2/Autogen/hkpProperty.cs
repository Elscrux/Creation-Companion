using System.Xml.Linq;
namespace HKX2
{
    // hkpProperty Signatire: 0x9ce308e9 size: 16 flags: FLAGS_NONE

    // m_key m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_alignmentPadding m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_value m_class: hkpPropertyValue Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkpProperty : IHavokObject, IEquatable<hkpProperty?>
    {
        public uint m_key { set; get; }
        public uint m_alignmentPadding { set; get; }
        public hkpPropertyValue m_value { set; get; } = new();

        public virtual uint Signature { set; get; } = 0x9ce308e9;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_key = br.ReadUInt32();
            m_alignmentPadding = br.ReadUInt32();
            m_value.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_key);
            bw.WriteUInt32(m_alignmentPadding);
            m_value.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_key = xd.ReadUInt32(xe, nameof(m_key));
            m_alignmentPadding = xd.ReadUInt32(xe, nameof(m_alignmentPadding));
            m_value = xd.ReadClass<hkpPropertyValue>(xe, nameof(m_value));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_key), m_key);
            xs.WriteNumber(xe, nameof(m_alignmentPadding), m_alignmentPadding);
            xs.WriteClass<hkpPropertyValue>(xe, nameof(m_value), m_value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpProperty);
        }

        public bool Equals(hkpProperty? other)
        {
            return other is not null &&
                   m_key.Equals(other.m_key) &&
                   m_alignmentPadding.Equals(other.m_alignmentPadding) &&
                   ((m_value is null && other.m_value is null) || (m_value is not null && other.m_value is not null && m_value.Equals((IHavokObject)other.m_value))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_key);
            hashcode.Add(m_alignmentPadding);
            hashcode.Add(m_value);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

