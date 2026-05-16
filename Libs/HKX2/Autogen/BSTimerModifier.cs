using System.Xml.Linq;
namespace HKX2
{
    // BSTimerModifier Signatire: 0x531f3292 size: 112 flags: FLAGS_NONE

    // m_alarmTimeSeconds m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_alarmEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_resetAlarm m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_secondsElapsed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 108 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSTimerModifier : hkbModifier, IEquatable<BSTimerModifier?>
    {
        public float m_alarmTimeSeconds { set; get; }
        public hkbEventProperty m_alarmEvent { set; get; } = new();
        public bool m_resetAlarm { set; get; }
        private float m_secondsElapsed { set; get; }

        public override uint Signature { set; get; } = 0x531f3292;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_alarmTimeSeconds = br.ReadSingle();
            br.Position += 4;
            m_alarmEvent.Read(des, br);
            m_resetAlarm = br.ReadBoolean();
            br.Position += 3;
            m_secondsElapsed = br.ReadSingle();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_alarmTimeSeconds);
            bw.Position += 4;
            m_alarmEvent.Write(s, bw);
            bw.WriteBoolean(m_resetAlarm);
            bw.Position += 3;
            bw.WriteSingle(m_secondsElapsed);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_alarmTimeSeconds = xd.ReadSingle(xe, nameof(m_alarmTimeSeconds));
            m_alarmEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_alarmEvent));
            m_resetAlarm = xd.ReadBoolean(xe, nameof(m_resetAlarm));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_alarmTimeSeconds), m_alarmTimeSeconds);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_alarmEvent), m_alarmEvent);
            xs.WriteBoolean(xe, nameof(m_resetAlarm), m_resetAlarm);
            xs.WriteSerializeIgnored(xe, nameof(m_secondsElapsed));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSTimerModifier);
        }

        public bool Equals(BSTimerModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_alarmTimeSeconds.Equals(other.m_alarmTimeSeconds) &&
                   ((m_alarmEvent is null && other.m_alarmEvent is null) || (m_alarmEvent is not null && other.m_alarmEvent is not null && m_alarmEvent.Equals((IHavokObject)other.m_alarmEvent))) &&
                   m_resetAlarm.Equals(other.m_resetAlarm) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_alarmTimeSeconds);
            hashcode.Add(m_alarmEvent);
            hashcode.Add(m_resetAlarm);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

