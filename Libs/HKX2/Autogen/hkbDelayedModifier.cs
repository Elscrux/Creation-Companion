using System.Xml.Linq;
namespace HKX2
{
    // hkbDelayedModifier Signatire: 0x8e101a7a size: 104 flags: FLAGS_NONE

    // m_delaySeconds m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_durationSeconds m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_secondsElapsed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_isActive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 100 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbDelayedModifier : hkbModifierWrapper, IEquatable<hkbDelayedModifier?>
    {
        public float m_delaySeconds { set; get; }
        public float m_durationSeconds { set; get; }
        private float m_secondsElapsed { set; get; }
        private bool m_isActive { set; get; }

        public override uint Signature { set; get; } = 0x8e101a7a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_delaySeconds = br.ReadSingle();
            m_durationSeconds = br.ReadSingle();
            m_secondsElapsed = br.ReadSingle();
            m_isActive = br.ReadBoolean();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_delaySeconds);
            bw.WriteSingle(m_durationSeconds);
            bw.WriteSingle(m_secondsElapsed);
            bw.WriteBoolean(m_isActive);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_delaySeconds = xd.ReadSingle(xe, nameof(m_delaySeconds));
            m_durationSeconds = xd.ReadSingle(xe, nameof(m_durationSeconds));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_delaySeconds), m_delaySeconds);
            xs.WriteFloat(xe, nameof(m_durationSeconds), m_durationSeconds);
            xs.WriteSerializeIgnored(xe, nameof(m_secondsElapsed));
            xs.WriteSerializeIgnored(xe, nameof(m_isActive));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbDelayedModifier);
        }

        public bool Equals(hkbDelayedModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_delaySeconds.Equals(other.m_delaySeconds) &&
                   m_durationSeconds.Equals(other.m_durationSeconds) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_delaySeconds);
            hashcode.Add(m_durationSeconds);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

