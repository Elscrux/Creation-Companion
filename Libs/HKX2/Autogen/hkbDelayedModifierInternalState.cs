using System.Xml.Linq;
namespace HKX2
{
    // hkbDelayedModifierInternalState Signatire: 0x85fb0b80 size: 24 flags: FLAGS_NONE

    // m_secondsElapsed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_isActive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    public partial class hkbDelayedModifierInternalState : hkReferencedObject, IEquatable<hkbDelayedModifierInternalState?>
    {
        public float m_secondsElapsed { set; get; }
        public bool m_isActive { set; get; }

        public override uint Signature { set; get; } = 0x85fb0b80;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_secondsElapsed = br.ReadSingle();
            m_isActive = br.ReadBoolean();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_secondsElapsed);
            bw.WriteBoolean(m_isActive);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_secondsElapsed = xd.ReadSingle(xe, nameof(m_secondsElapsed));
            m_isActive = xd.ReadBoolean(xe, nameof(m_isActive));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_secondsElapsed), m_secondsElapsed);
            xs.WriteBoolean(xe, nameof(m_isActive), m_isActive);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbDelayedModifierInternalState);
        }

        public bool Equals(hkbDelayedModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_secondsElapsed.Equals(other.m_secondsElapsed) &&
                   m_isActive.Equals(other.m_isActive) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_secondsElapsed);
            hashcode.Add(m_isActive);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

