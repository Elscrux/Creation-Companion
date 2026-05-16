using System.Xml.Linq;
namespace HKX2
{
    // hkxMaterialProperty Signatire: 0xd295234d size: 8 flags: FLAGS_NONE

    // m_key m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_value m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    public partial class hkxMaterialProperty : IHavokObject, IEquatable<hkxMaterialProperty?>
    {
        public uint m_key { set; get; }
        public uint m_value { set; get; }

        public virtual uint Signature { set; get; } = 0xd295234d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_key = br.ReadUInt32();
            m_value = br.ReadUInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_key);
            bw.WriteUInt32(m_value);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_key = xd.ReadUInt32(xe, nameof(m_key));
            m_value = xd.ReadUInt32(xe, nameof(m_value));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_key), m_key);
            xs.WriteNumber(xe, nameof(m_value), m_value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxMaterialProperty);
        }

        public bool Equals(hkxMaterialProperty? other)
        {
            return other is not null &&
                   m_key.Equals(other.m_key) &&
                   m_value.Equals(other.m_value) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_key);
            hashcode.Add(m_value);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

