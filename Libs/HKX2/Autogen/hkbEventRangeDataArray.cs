using System.Xml.Linq;
namespace HKX2
{
    // hkbEventRangeDataArray Signatire: 0x330a56ee size: 32 flags: FLAGS_NONE

    // m_eventData m_class: hkbEventRangeData Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbEventRangeDataArray : hkReferencedObject, IEquatable<hkbEventRangeDataArray?>
    {
        public IList<hkbEventRangeData> m_eventData { set; get; } = Array.Empty<hkbEventRangeData>();

        public override uint Signature { set; get; } = 0x330a56ee;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventData = des.ReadClassArray<hkbEventRangeData>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_eventData);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_eventData = xd.ReadClassArray<hkbEventRangeData>(xe, nameof(m_eventData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_eventData), m_eventData);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventRangeDataArray);
        }

        public bool Equals(hkbEventRangeDataArray? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_eventData.SequenceEqual(other.m_eventData) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_eventData.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

