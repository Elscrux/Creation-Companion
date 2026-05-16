using System.Xml.Linq;
namespace HKX2
{
    // hkbGetUpModifierInternalState Signatire: 0xd84cad4a size: 32 flags: FLAGS_NONE

    // m_timeSinceBegin m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_timeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_initNextModify m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    public partial class hkbGetUpModifierInternalState : hkReferencedObject, IEquatable<hkbGetUpModifierInternalState?>
    {
        public float m_timeSinceBegin { set; get; }
        public float m_timeStep { set; get; }
        public bool m_initNextModify { set; get; }

        public override uint Signature { set; get; } = 0xd84cad4a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_timeSinceBegin = br.ReadSingle();
            m_timeStep = br.ReadSingle();
            m_initNextModify = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_timeSinceBegin);
            bw.WriteSingle(m_timeStep);
            bw.WriteBoolean(m_initNextModify);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_timeSinceBegin = xd.ReadSingle(xe, nameof(m_timeSinceBegin));
            m_timeStep = xd.ReadSingle(xe, nameof(m_timeStep));
            m_initNextModify = xd.ReadBoolean(xe, nameof(m_initNextModify));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_timeSinceBegin), m_timeSinceBegin);
            xs.WriteFloat(xe, nameof(m_timeStep), m_timeStep);
            xs.WriteBoolean(xe, nameof(m_initNextModify), m_initNextModify);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbGetUpModifierInternalState);
        }

        public bool Equals(hkbGetUpModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_timeSinceBegin.Equals(other.m_timeSinceBegin) &&
                   m_timeStep.Equals(other.m_timeStep) &&
                   m_initNextModify.Equals(other.m_initNextModify) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_timeSinceBegin);
            hashcode.Add(m_timeStep);
            hashcode.Add(m_initNextModify);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

