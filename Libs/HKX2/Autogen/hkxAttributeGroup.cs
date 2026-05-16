using System.Xml.Linq;
namespace HKX2
{
    // hkxAttributeGroup Signatire: 0x345ca95d size: 24 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_attributes m_class: hkxAttribute Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkxAttributeGroup : IHavokObject, IEquatable<hkxAttributeGroup?>
    {
        public string m_name { set; get; } = "";
        public IList<hkxAttribute> m_attributes { set; get; } = Array.Empty<hkxAttribute>();

        public virtual uint Signature { set; get; } = 0x345ca95d;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_attributes = des.ReadClassArray<hkxAttribute>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_name);
            s.WriteClassArray(bw, m_attributes);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_name = xd.ReadString(xe, nameof(m_name));
            m_attributes = xd.ReadClassArray<hkxAttribute>(xe, nameof(m_attributes));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteClassArray(xe, nameof(m_attributes), m_attributes);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAttributeGroup);
        }

        public bool Equals(hkxAttributeGroup? other)
        {
            return other is not null &&
                   m_name == other.m_name &&
                   m_attributes.SequenceEqual(other.m_attributes) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_name);
            hashcode.Add(m_attributes.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

