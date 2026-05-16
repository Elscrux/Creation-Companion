using System.Xml.Linq;
namespace HKX2
{
    // hkDataObjectTypeAttribute Signatire: 0x1e3857bb size: 8 flags: FLAGS_NONE

    // m_typeName m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkDataObjectTypeAttribute : IHavokObject, IEquatable<hkDataObjectTypeAttribute?>
    {
        public string m_typeName { set; get; } = "";

        public virtual uint Signature { set; get; } = 0x1e3857bb;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_typeName = des.ReadCString(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteCString(bw, m_typeName);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_typeName = xd.ReadString(xe, nameof(m_typeName));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_typeName), m_typeName);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkDataObjectTypeAttribute);
        }

        public bool Equals(hkDataObjectTypeAttribute? other)
        {
            return other is not null &&
                   (m_typeName is null && other.m_typeName is null || m_typeName == other.m_typeName || m_typeName is null && other.m_typeName == "" || m_typeName == "" && other.m_typeName is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_typeName);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

