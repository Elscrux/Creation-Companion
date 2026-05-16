using System.Xml.Linq;
namespace HKX2
{
    // hkxAttribute Signatire: 0x7375cae3 size: 16 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_value m_class: hkReferencedObject Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkxAttribute : IHavokObject, IEquatable<hkxAttribute?>
    {
        public string m_name { set; get; } = "";
        public hkReferencedObject? m_value { set; get; }

        public virtual uint Signature { set; get; } = 0x7375cae3;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_value = des.ReadClassPointer<hkReferencedObject>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_name);
            s.WriteClassPointer(bw, m_value);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_name = xd.ReadString(xe, nameof(m_name));
            m_value = xd.ReadClassPointer<hkReferencedObject>(xe, nameof(m_value));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteClassPointer(xe, nameof(m_value), m_value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAttribute);
        }

        public bool Equals(hkxAttribute? other)
        {
            return other is not null &&
                   m_name == other.m_name &&
                   ((m_value is null && other.m_value is null) || (m_value is not null && other.m_value is not null && m_value.Equals((IHavokObject)other.m_value))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_name);
            hashcode.Add(m_value);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

