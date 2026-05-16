using System.Xml.Linq;
namespace HKX2
{
    // hkbEventsFromRangeModifier Signatire: 0xbc561b6e size: 112 flags: FLAGS_NONE

    // m_inputValue m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_lowerBound m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_eventRanges m_class: hkbEventRangeDataArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_wasActiveInPreviousFrame m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbEventsFromRangeModifier : hkbModifier, IEquatable<hkbEventsFromRangeModifier?>
    {
        public float m_inputValue { set; get; }
        public float m_lowerBound { set; get; }
        public hkbEventRangeDataArray? m_eventRanges { set; get; }
        public IList<object> m_wasActiveInPreviousFrame { set; get; } = Array.Empty<object>();

        public override uint Signature { set; get; } = 0xbc561b6e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_inputValue = br.ReadSingle();
            m_lowerBound = br.ReadSingle();
            m_eventRanges = des.ReadClassPointer<hkbEventRangeDataArray>(br);
            des.ReadEmptyArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_inputValue);
            bw.WriteSingle(m_lowerBound);
            s.WriteClassPointer(bw, m_eventRanges);
            s.WriteVoidArray(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_inputValue = xd.ReadSingle(xe, nameof(m_inputValue));
            m_lowerBound = xd.ReadSingle(xe, nameof(m_lowerBound));
            m_eventRanges = xd.ReadClassPointer<hkbEventRangeDataArray>(xe, nameof(m_eventRanges));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_inputValue), m_inputValue);
            xs.WriteFloat(xe, nameof(m_lowerBound), m_lowerBound);
            xs.WriteClassPointer(xe, nameof(m_eventRanges), m_eventRanges);
            xs.WriteSerializeIgnored(xe, nameof(m_wasActiveInPreviousFrame));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventsFromRangeModifier);
        }

        public bool Equals(hkbEventsFromRangeModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_inputValue.Equals(other.m_inputValue) &&
                   m_lowerBound.Equals(other.m_lowerBound) &&
                   ((m_eventRanges is null && other.m_eventRanges is null) || (m_eventRanges is not null && other.m_eventRanges is not null && m_eventRanges.Equals((IHavokObject)other.m_eventRanges))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_inputValue);
            hashcode.Add(m_lowerBound);
            hashcode.Add(m_eventRanges);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

