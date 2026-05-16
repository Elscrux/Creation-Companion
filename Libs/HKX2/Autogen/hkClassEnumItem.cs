using System.Xml.Linq;
namespace HKX2
{
    // hkClassEnumItem Signatire: 0xce6f8a6c size: 16 flags: FLAGS_NONE

    // m_value m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_name m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkClassEnumItem : IHavokObject, IEquatable<hkClassEnumItem?>
    {
        public int m_value { set; get; }
        public string m_name { set; get; } = "";

        public virtual uint Signature { set; get; } = 0xce6f8a6c;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = br.ReadInt32();
            br.Position += 4;
            m_name = des.ReadCString(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt32(m_value);
            bw.Position += 4;
            s.WriteCString(bw, m_name);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_value = xd.ReadInt32(xe, nameof(m_value));
            m_name = xd.ReadString(xe, nameof(m_name));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_value), m_value);
            xs.WriteString(xe, nameof(m_name), m_name);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkClassEnumItem);
        }

        public bool Equals(hkClassEnumItem? other)
        {
            return other is not null &&
                   m_value.Equals(other.m_value) &&
                   m_name == other.m_name &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_value);
            hashcode.Add(m_name);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

