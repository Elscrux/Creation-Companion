using System.Xml.Linq;
namespace HKX2
{
    // hkDocumentationAttribute Signatire: 0x630edd9e size: 8 flags: FLAGS_NONE

    // m_docsSectionTag m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkDocumentationAttribute : IHavokObject, IEquatable<hkDocumentationAttribute?>
    {
        public string m_docsSectionTag { set; get; } = "";

        public virtual uint Signature { set; get; } = 0x630edd9e;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_docsSectionTag = des.ReadCString(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteCString(bw, m_docsSectionTag);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_docsSectionTag = xd.ReadString(xe, nameof(m_docsSectionTag));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_docsSectionTag), m_docsSectionTag);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkDocumentationAttribute);
        }

        public bool Equals(hkDocumentationAttribute? other)
        {
            return other is not null &&
                   (m_docsSectionTag is null && other.m_docsSectionTag is null || m_docsSectionTag == other.m_docsSectionTag || m_docsSectionTag is null && other.m_docsSectionTag == "" || m_docsSectionTag == "" && other.m_docsSectionTag is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_docsSectionTag);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

