using System.Xml.Linq;
namespace HKX2
{
    // hkbSequence Signatire: 0x43182ca3 size: 248 flags: FLAGS_NONE

    // m_eventSequencedData m_class: hkbEventSequencedData Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_realVariableSequencedData m_class: hkbRealVariableSequencedData Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_boolVariableSequencedData m_class: hkbBoolVariableSequencedData Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_intVariableSequencedData m_class: hkbIntVariableSequencedData Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_enableEventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_disableEventId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 148 flags: FLAGS_NONE enum: 
    // m_stringData m_class: hkbSequenceStringData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_variableIdMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 160 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_eventIdMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 168 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_nextSampleEvents m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 176 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_nextSampleReals m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 192 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_nextSampleBools m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 208 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_nextSampleInts m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 224 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_time m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 240 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_isEnabled m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 244 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbSequence : hkbModifier, IEquatable<hkbSequence?>
    {
        public IList<hkbEventSequencedData> m_eventSequencedData { set; get; } = Array.Empty<hkbEventSequencedData>();
        public IList<hkbRealVariableSequencedData> m_realVariableSequencedData { set; get; } = Array.Empty<hkbRealVariableSequencedData>();
        public IList<hkbBoolVariableSequencedData> m_boolVariableSequencedData { set; get; } = Array.Empty<hkbBoolVariableSequencedData>();
        public IList<hkbIntVariableSequencedData> m_intVariableSequencedData { set; get; } = Array.Empty<hkbIntVariableSequencedData>();
        public int m_enableEventId { set; get; }
        public int m_disableEventId { set; get; }
        public hkbSequenceStringData? m_stringData { set; get; }
        private object? m_variableIdMap { set; get; }
        private object? m_eventIdMap { set; get; }
        public IList<object> m_nextSampleEvents { set; get; } = Array.Empty<object>();
        public IList<object> m_nextSampleReals { set; get; } = Array.Empty<object>();
        public IList<object> m_nextSampleBools { set; get; } = Array.Empty<object>();
        public IList<object> m_nextSampleInts { set; get; } = Array.Empty<object>();
        private float m_time { set; get; }
        private bool m_isEnabled { set; get; }

        public override uint Signature { set; get; } = 0x43182ca3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventSequencedData = des.ReadClassPointerArray<hkbEventSequencedData>(br);
            m_realVariableSequencedData = des.ReadClassPointerArray<hkbRealVariableSequencedData>(br);
            m_boolVariableSequencedData = des.ReadClassPointerArray<hkbBoolVariableSequencedData>(br);
            m_intVariableSequencedData = des.ReadClassPointerArray<hkbIntVariableSequencedData>(br);
            m_enableEventId = br.ReadInt32();
            m_disableEventId = br.ReadInt32();
            m_stringData = des.ReadClassPointer<hkbSequenceStringData>(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            m_time = br.ReadSingle();
            m_isEnabled = br.ReadBoolean();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_eventSequencedData);
            s.WriteClassPointerArray(bw, m_realVariableSequencedData);
            s.WriteClassPointerArray(bw, m_boolVariableSequencedData);
            s.WriteClassPointerArray(bw, m_intVariableSequencedData);
            bw.WriteInt32(m_enableEventId);
            bw.WriteInt32(m_disableEventId);
            s.WriteClassPointer(bw, m_stringData);
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            bw.WriteSingle(m_time);
            bw.WriteBoolean(m_isEnabled);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_eventSequencedData = xd.ReadClassPointerArray<hkbEventSequencedData>(xe, nameof(m_eventSequencedData));
            m_realVariableSequencedData = xd.ReadClassPointerArray<hkbRealVariableSequencedData>(xe, nameof(m_realVariableSequencedData));
            m_boolVariableSequencedData = xd.ReadClassPointerArray<hkbBoolVariableSequencedData>(xe, nameof(m_boolVariableSequencedData));
            m_intVariableSequencedData = xd.ReadClassPointerArray<hkbIntVariableSequencedData>(xe, nameof(m_intVariableSequencedData));
            m_enableEventId = xd.ReadInt32(xe, nameof(m_enableEventId));
            m_disableEventId = xd.ReadInt32(xe, nameof(m_disableEventId));
            m_stringData = xd.ReadClassPointer<hkbSequenceStringData>(xe, nameof(m_stringData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_eventSequencedData), m_eventSequencedData);
            xs.WriteClassPointerArray(xe, nameof(m_realVariableSequencedData), m_realVariableSequencedData);
            xs.WriteClassPointerArray(xe, nameof(m_boolVariableSequencedData), m_boolVariableSequencedData);
            xs.WriteClassPointerArray(xe, nameof(m_intVariableSequencedData), m_intVariableSequencedData);
            xs.WriteNumber(xe, nameof(m_enableEventId), m_enableEventId);
            xs.WriteNumber(xe, nameof(m_disableEventId), m_disableEventId);
            xs.WriteClassPointer(xe, nameof(m_stringData), m_stringData);
            xs.WriteSerializeIgnored(xe, nameof(m_variableIdMap));
            xs.WriteSerializeIgnored(xe, nameof(m_eventIdMap));
            xs.WriteSerializeIgnored(xe, nameof(m_nextSampleEvents));
            xs.WriteSerializeIgnored(xe, nameof(m_nextSampleReals));
            xs.WriteSerializeIgnored(xe, nameof(m_nextSampleBools));
            xs.WriteSerializeIgnored(xe, nameof(m_nextSampleInts));
            xs.WriteSerializeIgnored(xe, nameof(m_time));
            xs.WriteSerializeIgnored(xe, nameof(m_isEnabled));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSequence);
        }

        public bool Equals(hkbSequence? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_eventSequencedData.SequenceEqual(other.m_eventSequencedData) &&
                   m_realVariableSequencedData.SequenceEqual(other.m_realVariableSequencedData) &&
                   m_boolVariableSequencedData.SequenceEqual(other.m_boolVariableSequencedData) &&
                   m_intVariableSequencedData.SequenceEqual(other.m_intVariableSequencedData) &&
                   m_enableEventId.Equals(other.m_enableEventId) &&
                   m_disableEventId.Equals(other.m_disableEventId) &&
                   ((m_stringData is null && other.m_stringData is null) || (m_stringData is not null && other.m_stringData is not null && m_stringData.Equals((IHavokObject)other.m_stringData))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_eventSequencedData.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_realVariableSequencedData.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_boolVariableSequencedData.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_intVariableSequencedData.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_enableEventId);
            hashcode.Add(m_disableEventId);
            hashcode.Add(m_stringData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

