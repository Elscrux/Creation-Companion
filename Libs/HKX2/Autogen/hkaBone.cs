using System.Xml.Linq;
namespace HKX2
{
    // hkaBone Signatire: 0x35912f8a size: 16 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_lockTranslation m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkaBone : IHavokObject, IEquatable<hkaBone?>
    {
        public string m_name { set; get; } = "";
        public bool m_lockTranslation { set; get; }

        public virtual uint Signature { set; get; } = 0x35912f8a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_lockTranslation = br.ReadBoolean();
            br.Position += 7;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStringPointer(bw, m_name);
            bw.WriteBoolean(m_lockTranslation);
            bw.Position += 7;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_name = xd.ReadString(xe, nameof(m_name));
            m_lockTranslation = xd.ReadBoolean(xe, nameof(m_lockTranslation));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_name), m_name);
            xs.WriteBoolean(xe, nameof(m_lockTranslation), m_lockTranslation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaBone);
        }

        public bool Equals(hkaBone? other)
        {
            return other is not null &&
                   m_name == other.m_name &&
                   m_lockTranslation.Equals(other.m_lockTranslation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_name);
            hashcode.Add(m_lockTranslation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

