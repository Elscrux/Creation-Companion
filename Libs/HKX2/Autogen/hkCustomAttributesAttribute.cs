using System.Xml.Linq;
namespace HKX2
{
    // hkCustomAttributesAttribute Signatire: 0x1388d601 size: 24 flags: FLAGS_NONE

    // m_name m_class:  Type.TYPE_CSTRING Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_value m_class:  Type.TYPE_VARIANT Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    public partial class hkCustomAttributesAttribute : IHavokObject, IEquatable<hkCustomAttributesAttribute?>
    {
        public string m_name { set; get; } = "";
        public object? m_value { set; get; }

        public virtual uint Signature { set; get; } = 0x1388d601;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadCString(br);
            throw new NotImplementedException("TPYE_VARIANT");
            br.Position += 8;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteCString(bw, m_name);
            throw new NotImplementedException("TPYE_VARIANT");
            bw.Position += 8;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_name = xd.ReadString(xe, nameof(m_name));
            throw new NotImplementedException("TPYE_VARIANT");
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteString(xe, nameof(m_name), m_name);
            throw new NotImplementedException("TPYE_VARIANT");
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkCustomAttributesAttribute);
        }

        public bool Equals(hkCustomAttributesAttribute? other)
        {
            return other is not null &&
                   m_name == other.m_name &&
                   Equals(m_value, other.m_value) &&
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

