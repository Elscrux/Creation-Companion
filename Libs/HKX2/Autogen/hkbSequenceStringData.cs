using System.Xml.Linq;
namespace HKX2
{
    // hkbSequenceStringData Signatire: 0x6a5094e3 size: 48 flags: FLAGS_NONE

    // m_eventNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_variableNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkbSequenceStringData : hkReferencedObject, IEquatable<hkbSequenceStringData?>
    {
        public IList<string> m_eventNames { set; get; } = Array.Empty<string>();
        public IList<string> m_variableNames { set; get; } = Array.Empty<string>();

        public override uint Signature { set; get; } = 0x6a5094e3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventNames = des.ReadStringPointerArray(br);
            m_variableNames = des.ReadStringPointerArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointerArray(bw, m_eventNames);
            s.WriteStringPointerArray(bw, m_variableNames);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_eventNames = xd.ReadStringArray(xe, nameof(m_eventNames));
            m_variableNames = xd.ReadStringArray(xe, nameof(m_variableNames));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteStringArray(xe, nameof(m_eventNames), m_eventNames);
            xs.WriteStringArray(xe, nameof(m_variableNames), m_variableNames);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSequenceStringData);
        }

        public bool Equals(hkbSequenceStringData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_eventNames.SequenceEqual(other.m_eventNames) &&
                   m_variableNames.SequenceEqual(other.m_variableNames) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_eventNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_variableNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

