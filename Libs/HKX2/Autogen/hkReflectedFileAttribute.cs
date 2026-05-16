using System.Xml.Linq;
namespace HKX2
{
    // hkReflectedFileAttribute Signatire: 0xedb6b8f7 size: 8 flags: FLAGS_NONE

    // m_value m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkReflectedFileAttribute : IHavokObject, IEquatable<hkReflectedFileAttribute?>
    {
        public string m_value { set; get; } = "";

        public virtual uint Signature { set; get; } = 0xedb6b8f7;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = des.ReadCString(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteCString(bw, m_value);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_value = xd.ReadString(xe, nameof(m_value));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_value), m_value);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkReflectedFileAttribute);
        }

        public bool Equals(hkReflectedFileAttribute? other)
        {
            return other is not null &&
                   (m_value is null && other.m_value is null || m_value == other.m_value || m_value is null && other.m_value == "" || m_value == "" && other.m_value is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_value);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

