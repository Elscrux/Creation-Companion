using System.Xml.Linq;
namespace HKX2
{
    // hkMemoryResourceHandleExternalLink Signatire: 0x3144d17c size: 16 flags: FLAGS_NONE

    // m_memberName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_externalId m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkMemoryResourceHandleExternalLink : IHavokObject, IEquatable<hkMemoryResourceHandleExternalLink?>
    {
        public string m_memberName { set; get; } = "";
        public string m_externalId { set; get; } = "";

        public virtual uint Signature { set; get; } = 0x3144d17c;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_memberName = des.ReadStringPointer(br);
            m_externalId = des.ReadStringPointer(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_memberName);
            s.WriteStringPointer(bw, m_externalId);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_memberName = xd.ReadString(xe, nameof(m_memberName));
            m_externalId = xd.ReadString(xe, nameof(m_externalId));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_memberName), m_memberName);
            xs.WriteString(xe, nameof(m_externalId), m_externalId);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkMemoryResourceHandleExternalLink);
        }

        public bool Equals(hkMemoryResourceHandleExternalLink? other)
        {
            return other is not null &&
                   (m_memberName is null && other.m_memberName is null || m_memberName == other.m_memberName || m_memberName is null && other.m_memberName == "" || m_memberName == "" && other.m_memberName is null) &&
                   (m_externalId is null && other.m_externalId is null || m_externalId == other.m_externalId || m_externalId is null && other.m_externalId == "" || m_externalId == "" && other.m_externalId is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_memberName);
            hashcode.Add(m_externalId);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

