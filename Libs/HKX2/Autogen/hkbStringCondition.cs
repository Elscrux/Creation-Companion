using System.Xml.Linq;
namespace HKX2
{
    // hkbStringCondition Signatire: 0x5ab50487 size: 24 flags: FLAGS_NONE

    // m_conditionString m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbStringCondition : hkbCondition, IEquatable<hkbStringCondition?>
    {
        public string m_conditionString { set; get; } = "";

        public override uint Signature { set; get; } = 0x5ab50487;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_conditionString = des.ReadStringPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_conditionString);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_conditionString = xd.ReadString(xe, nameof(m_conditionString));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_conditionString), m_conditionString);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStringCondition);
        }

        public bool Equals(hkbStringCondition? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_conditionString is null && other.m_conditionString is null || m_conditionString == other.m_conditionString || m_conditionString is null && other.m_conditionString == "" || m_conditionString == "" && other.m_conditionString is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_conditionString);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

