using System.Xml.Linq;
namespace HKX2
{
    // hkpPropertyValue Signatire: 0xc75925aa size: 8 flags: FLAGS_NONE

    // m_data m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkpPropertyValue : IHavokObject, IEquatable<hkpPropertyValue?>
    {
        public ulong m_data { set; get; }

        public virtual uint Signature { set; get; } = 0xc75925aa;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_data = br.ReadUInt64();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt64(m_data);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_data = xd.ReadUInt64(xe, nameof(m_data));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPropertyValue);
        }

        public bool Equals(hkpPropertyValue? other)
        {
            return other is not null &&
                   m_data.Equals(other.m_data) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_data);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

