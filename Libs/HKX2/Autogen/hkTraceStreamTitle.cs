using System.Xml.Linq;
namespace HKX2
{
    // hkTraceStreamTitle Signatire: 0x6a4ca82c size: 32 flags: FLAGS_NOT_SERIALIZABLE

    // m_value m_class:  Type.TYPE_CHAR Type.TYPE_VOID arrSize: 32 offset: 0 flags: FLAGS_NONE enum: 
    public partial class hkTraceStreamTitle : IHavokObject, IEquatable<hkTraceStreamTitle?>
    {
        public string m_value { set; get; } = "";

        public virtual uint Signature { set; get; } = 0x6a4ca82c;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_value = br.ReadASCII(32);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteASCII(m_value);
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
            return Equals(obj as hkTraceStreamTitle);
        }

        public bool Equals(hkTraceStreamTitle? other)
        {
            return other is not null &&
                   m_value.SequenceEqual(other.m_value) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_value.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

