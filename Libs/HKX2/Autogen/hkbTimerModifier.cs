using System.Xml.Linq;
namespace HKX2
{
    // hkbTimerModifier Signatire: 0x338b4879 size: 112 flags: FLAGS_NONE

    // m_alarmTimeSeconds m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_alarmEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_secondsElapsed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbTimerModifier : hkbModifier, IEquatable<hkbTimerModifier?>
    {
        public float m_alarmTimeSeconds { set; get; }
        public hkbEventProperty m_alarmEvent { set; get; } = new();
        private float m_secondsElapsed { set; get; }

        public override uint Signature { set; get; } = 0x338b4879;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_alarmTimeSeconds = br.ReadSingle();
            br.Position += 4;
            m_alarmEvent.Read(des, br);
            m_secondsElapsed = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_alarmTimeSeconds);
            bw.Position += 4;
            m_alarmEvent.Write(s, bw);
            bw.WriteSingle(m_secondsElapsed);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_alarmTimeSeconds = xd.ReadSingle(xe, nameof(m_alarmTimeSeconds));
            m_alarmEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_alarmEvent));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_alarmTimeSeconds), m_alarmTimeSeconds);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_alarmEvent), m_alarmEvent);
            xs.WriteSerializeIgnored(xe, nameof(m_secondsElapsed));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbTimerModifier);
        }

        public bool Equals(hkbTimerModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_alarmTimeSeconds.Equals(other.m_alarmTimeSeconds) &&
                   ((m_alarmEvent is null && other.m_alarmEvent is null) || (m_alarmEvent is not null && other.m_alarmEvent is not null && m_alarmEvent.Equals((IHavokObject)other.m_alarmEvent))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_alarmTimeSeconds);
            hashcode.Add(m_alarmEvent);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

