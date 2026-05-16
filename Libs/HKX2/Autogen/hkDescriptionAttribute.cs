using System.Xml.Linq;
namespace HKX2
{
    // hkDescriptionAttribute Signatire: 0xe9f9578a size: 8 flags: FLAGS_NONE

    // m_string m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkDescriptionAttribute : IHavokObject, IEquatable<hkDescriptionAttribute?>
    {
        public string m_string { set; get; } = "";

        public virtual uint Signature { set; get; } = 0xe9f9578a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_string = des.ReadCString(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteCString(bw, m_string);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_string = xd.ReadString(xe, nameof(m_string));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_string), m_string);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkDescriptionAttribute);
        }

        public bool Equals(hkDescriptionAttribute? other)
        {
            return other is not null &&
                   (m_string is null && other.m_string is null || m_string == other.m_string || m_string is null && other.m_string == "" || m_string == "" && other.m_string is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_string);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

